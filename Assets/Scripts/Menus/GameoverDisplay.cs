using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameoverDisplay : MonoBehaviour
{
    [SerializeField] GameObject gameoverDisplayObject = null;
    [SerializeField] private TMP_Text winnerNameText = null;
    private void Start()
    {
        GameoverHandler.ClientOnGameOver += ClientHandleGaveover;
    }

    private void OnDestroy()
    {
        GameoverHandler.ClientOnGameOver -= ClientHandleGaveover;
    }

    public void LeaveGame()
    {
        // When the game is over we stop hosting if we are a host and stop clietn
        // if we are aclient. After we stop hosting, we automatically send us back
        // to the offline scene (home screen) that we specified in the NetworkManager
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGaveover(string winner)
    {
        winnerNameText.text = $"{winner} has won!";
        gameoverDisplayObject.SetActive(true);
    }
}
