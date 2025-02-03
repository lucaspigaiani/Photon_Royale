using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameTxt;
    [SerializeField] private TextMeshProUGUI roomPlayersCountTxt;

    public void Inicialize(string roomName, int roomPlayers, int maxRoomPlayers) 
    {
        roomNameTxt.text = roomName;
        roomPlayersCountTxt.text = $"{roomPlayers}/{maxRoomPlayers}";
    }

    public void Join()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRoom(roomNameTxt.text);
        }
    }
}
