using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    public Player[] playerObjects;
    public Image lifeText;
    public Image[] lifeObjects;
    public Image[] steamObjects;


    [SerializeField]
    float startUpDelay = 2f;

    [System.Serializable]
    public struct Player
    {
        public Image image;
        public Image[] wrenches;
    }

    [SerializeField]
    int playerStart = 1;

    int currentPlayer;

	// Use this for initialization
	void Start () {
        currentPlayer = playerStart;
	}

    float sUD = 0;

	// Update is called once per frame
	void Update () {

        if(sUD >= startUpDelay)
        {
            StartGame();
        }
        else
        {
            sUD += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {

        }

    }

    void StartGame()
    {
        HideAll();

        ShowPlayer(currentPlayer);

    }

    
    void ShowPlayer(int playerIndex)
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].image.gameObject.SetActive((i == playerIndex) ? true : false);
        }
        ShowWrench(playerIndex, 0);
    }

    void ShowWrench(int playerIndex, int wrenchIndex)
    {
        for (int i = 0; i < playerObjects[playerIndex].wrenches.Length; i++)
        {
            playerObjects[playerIndex].wrenches[i].gameObject.SetActive((i == wrenchIndex) ? true : false);
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
        for (int i = 0; i < steamObjects.Length; i++)
        {
            steamObjects[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            lifeObjects[i].gameObject.SetActive(false);
        }
        lifeText.gameObject.SetActive(false);
    }
}
