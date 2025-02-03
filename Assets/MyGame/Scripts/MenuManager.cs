using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class MenuManager : MonoBehaviourPunCallbacks
{
    private const string PlayerName = "PlayerName";
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject roomListPanel;

    void Start()
    {
        if (PlayerPrefs.HasKey(PlayerName)) 
        {
            playerNameInput.text = PlayerPrefs.GetString(PlayerName);
        }

        EnableCanvas(loginPanel.name);
    }

    private void EnableCanvas(string activePanel) 
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));
    }

    public void SignIn() 
    {
        if (playerNameInput.text == "")
        {
            Debug.Log("Enter name player");
            return;
        }

        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString(PlayerName, playerNameInput.text);
        PhotonNetwork.ConnectUsingSettings();
        

    }

    public override void OnConnectedToMaster()
    {
        EnableCanvas(roomListPanel.name);
    }

}
