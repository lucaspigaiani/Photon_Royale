using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playersParent;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        MenuManager.Instance.ShowRoomListPanel();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        MenuManager.Instance.ClearRoomListView();
        MenuManager.Instance.UpdateCachedRoomList(roomList);
        MenuManager.Instance.UpdateRoomListView();
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.InLobby) return;

        string tempRoomName = "Room" + Random.Range(1, 1000);
        byte maxRoomPlayers = (byte)Random.Range(2, 3);

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxRoomPlayers,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(tempRoomName, roomOptions);
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom) return;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OnJoinedRoom();
        UpdatePlayerList();
        GameObject newPlayer = PhotonNetwork.Instantiate(player.name, this.transform.position, this.transform.rotation);
        newPlayer.transform.parent = playersParent.transform;
        newPlayer.name = PhotonNetwork.LocalPlayer.UserId;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        UpdatePlayerList(); // Atualiza a UI dos jogadores
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.CurrentRoom.IsOpen && !PhotonNetwork.CurrentRoom.IsVisible)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        UpdatePlayerList(); // Atualiza a UI dos jogadores
    }

    public override void OnLeftLobby()
    {
        MenuManager.Instance.OnLeftLobby();
    }

    private void UpdatePlayerList()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.UpdatePlayerList(PhotonNetwork.PlayerList);
        }
    }
}
