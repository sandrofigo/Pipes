using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    public Player[] playerObjects;
    public Image lifeText;
    public Image[] lifeObjects;
    public Pipe[] pipes;


    [SerializeField]
    float startUpDelay = 2f;

    [System.Serializable]
    public struct Player
    {
        public Image image;
        public Image[] wrenches;
    }

    [System.Serializable]
    public struct Pipe
    {
        public Image[] steamImages;
        public Image[] valves;
    }

    GameState gameState;

    enum GameState
    {
        StartUp = 0,
        Playing = 1,
        GameOver = 2,
        ValveAnimationPlaying = 3
    }

    [SerializeField]
    int playerStart = 1;

    int currentPlayer;
    int currentWrench = 0;

    [SerializeField]
    int lifes = 3;

    float[] valvePressures = new float[3];

    [SerializeField]
    float minValvePressure = 0.2f;
    [SerializeField]
    float maxValvePressure = 0.7f;
    [SerializeField]
    float wrenchFixValue = 0.1f;
    [SerializeField]
    float wrenchTurnDelay = 0.5f;
    float wrenchTime;
    [SerializeField]
    float walkDelay = 0.5f;
    float walkTime;

    [SerializeField]
    float valvePenalty = -0.5f;

    [SerializeField]
    AnimationCurve difficultyCurve;
    float difficulty;

    bool canWalk;
    bool canFix;

    [SerializeField]
    AudioSource walkSound;
    [SerializeField]
    AudioSource fixSound;
    [SerializeField]
    AudioSource valvePopSound;

    // Use this for initialization
    void Start()
    {
        currentPlayer = playerStart;
        gameState = GameState.StartUp;

        Invoke("StartGame", startUpDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Playing)
        {
            if (walkTime <= 0 && canWalk)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    currentPlayer--;
                    currentPlayer = Mathf.Clamp(currentPlayer, 0, playerObjects.Length - 1);

                    ShowPlayer(currentPlayer);
                    walkTime = walkDelay;

                    walkSound.Play();
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    currentPlayer++;
                    currentPlayer = Mathf.Clamp(currentPlayer, 0, playerObjects.Length - 1);

                    ShowPlayer(currentPlayer);
                    walkTime = walkDelay;

                    walkSound.Play();
                }
            }
            walkTime = Mathf.Clamp(walkTime -= Time.deltaTime, 0, walkDelay);

            if (Input.GetKeyDown(KeyCode.Space) && wrenchTime <= 0 && canFix)
            {
                if (currentWrench == 0) currentWrench++;
                else currentWrench--;
                ShowWrench(currentPlayer, currentWrench);

                if (valvePressures[currentPlayer] > 0) valvePressures[currentPlayer] = Mathf.Clamp(valvePressures[currentPlayer] - wrenchFixValue, 0, 1);
                wrenchTime = wrenchTurnDelay;

                difficulty = Mathf.Clamp01(difficulty + 0.01f);

                fixSound.Play();
            }
            wrenchTime = Mathf.Clamp(wrenchTime -= Time.deltaTime, 0, wrenchTurnDelay);

            for (int i = 0; i < valvePressures.Length; i++)
            {
                valvePressures[i] += Time.deltaTime * Random.Range(0f, 0.04f * (1 + difficultyCurve.Evaluate(difficulty)));

                if (valvePressures[i] > minValvePressure) EnableSteam(i, 0);
                if (valvePressures[i] > maxValvePressure) EnableSteam(i, 1);
                if (valvePressures[i] < minValvePressure) DisableSteam(i);

                if (valvePressures[i] > 1f)
                {
                    gameState = GameState.ValveAnimationPlaying;
                    canWalk = false;
                    canFix = false;

                    lifes--;
                    ShowLifes();

                    for (int j = 0; j < pipes.Length; j++)
                    {
                        if (j != i) DisableSteam(j);
                    }

                    valvePopSound.Play();

                    StartCoroutine(ValveAnimation(i));
                    break;
                }
            }

        }
    }

    void StartGame()
    {
        HideAll();

        ShowPlayer(currentPlayer);

        ShowLifes();

        for (int i = 0; i < pipes.Length; i++)
        {
            EnableValve(i, 0);
        }

        for (int i = 0; i < valvePressures.Length; i++)
        {
            valvePressures[i] = Random.Range(0, 0.5f);
        }

        canWalk = true;
        canFix = true;

        gameState = GameState.Playing;
    }

    void EnableSteam(int pipeIndex)
    {
        EnableSteam(pipeIndex, Random.Range(0, pipes[pipeIndex].steamImages.Length));
    }

    void EnableSteam(int pipeIndex, int steamIndex)
    {
        pipes[pipeIndex].steamImages[steamIndex].gameObject.SetActive(true);
        pipes[pipeIndex].steamImages[steamIndex].GetComponent<Steam>().blink = true;
    }

    void DisableSteam()
    {
        for (int i = 0; i < pipes.Length; i++)
        {
            DisableSteam(i);
        }
    }

    void DisableSteam(int pipeIndex)
    {
        for (int i = 0; i < pipes[pipeIndex].steamImages.Length; i++)
        {
            pipes[pipeIndex].steamImages[i].gameObject.SetActive(false);
            pipes[pipeIndex].steamImages[i].GetComponent<Steam>().blink = false;
        }
    }

    void EnableValve(int pipeIndex, int valveIndex)
    {
        for (int i = 0; i < pipes[pipeIndex].valves.Length; i++)
        {
            pipes[pipeIndex].valves[i].gameObject.SetActive(i == valveIndex ? true : false);
        }
    }

    void ShowPlayer(int playerIndex)
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].image.gameObject.SetActive(i == playerIndex ? true : false);
        }
        ShowWrench(playerIndex, 0);
        currentWrench = 0;
    }

    void ShowWrench(int playerIndex, int wrenchIndex)
    {
        for (int i = 0; i < playerObjects[playerIndex].wrenches.Length; i++)
        {
            playerObjects[playerIndex].wrenches[i].gameObject.SetActive(i == wrenchIndex ? true : false);
        }
    }

    void ShowLifes()
    {
        lifeText.gameObject.SetActive(true);
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            lifeObjects[i].gameObject.SetActive(i < lifes ? true : false);
        }
    }

    void HideAll()
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].image.gameObject.SetActive(false);
            for (int j = 0; j < playerObjects[i].wrenches.Length; j++)
            {
                playerObjects[i].wrenches[j].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < pipes.Length; i++)
        {
            for (int j = 0; j < pipes[i].steamImages.Length; j++)
            {
                pipes[i].steamImages[j].gameObject.SetActive(false);
            }
            for (int j = 0; j < pipes[i].valves.Length; j++)
            {
                pipes[i].valves[j].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            lifeObjects[i].gameObject.SetActive(false);
        }
        lifeText.gameObject.SetActive(false);
    }

    IEnumerator ValveAnimation(int pipeIndex)
    {
        int currentValve = 0;

        while (currentValve < pipes[pipeIndex].valves.Length)
        {
            EnableValve(pipeIndex, currentValve);
            currentValve++;
            yield return new WaitForSeconds(0.25f);
        }

        valvePressures[pipeIndex] = valvePenalty;
        EnableValve(pipeIndex, 0);

        gameState = GameState.Playing;
        canWalk = true;
        canFix = true;
    }
}
