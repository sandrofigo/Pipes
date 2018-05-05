using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


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
    }

    [SerializeField]
    int playerStart = 1;

    int currentPlayer;
    int currentWrench = 0;

    [SerializeField]
    int lifes = 3;

	// Use this for initialization
	void Start () {
        currentPlayer = playerStart;

        Invoke("StartGame", startUpDelay);
	}

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentPlayer--;
            currentPlayer = Mathf.Clamp(currentPlayer, 0, playerObjects.Length-1);

            ShowPlayer(currentPlayer);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentPlayer++;
            currentPlayer = Mathf.Clamp(currentPlayer, 0, playerObjects.Length-1);

            ShowPlayer(currentPlayer);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentWrench == 0) currentWrench++;
            else currentWrench--;
            ShowWrench(currentPlayer, currentWrench);
        }

    }

    void StartGame()
    {
        HideAll();

        ShowPlayer(currentPlayer);

        ShowLifes();

        EnableSteam(0);
    }

    void EnableSteam(int pipeIndex)
    {
        int rndSteam = Random.Range(0, pipes[pipeIndex].steamImages.Length);
        pipes[pipeIndex].steamImages[rndSteam].gameObject.SetActive(true);
        pipes[pipeIndex].steamImages[rndSteam].GetComponent<Steam>().blink = true;
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
        }
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            lifeObjects[i].gameObject.SetActive(false);
        }
        lifeText.gameObject.SetActive(false);
    }
}
