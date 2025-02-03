using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject roomListItem;

    [SerializeField] private TextMeshProUGUI infoStatus;
    [SerializeField] private TextMeshProUGUI infoPlayerLobby;
    [SerializeField] private TextMeshProUGUI infoPlayerGame;

    private string infoPlayerLobbyTxt = "Players in lobby: ";
    private string infoPlayerGameTxt = "Players in game: ";

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListEntries = new Dictionary<string, GameObject>();

    private void FixedUpdate()
    {
        infoStatus.text = PhotonNetwork.NetworkClientState.ToString();
        infoPlayerLobby.text = infoPlayerLobbyTxt + PhotonNetwork.CountOfPlayersOnMaster.ToString();
        infoPlayerGame.text = infoPlayerGameTxt + PhotonNetwork.CountOfPlayersInRooms.ToString();
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.InLobby)
        {
            return;
        }
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
        if (!PhotonNetwork.InRoom)
        {
            return;
        }
        PhotonNetwork.LeaveRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.JoinLobby();

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.IsOpen == false && PhotonNetwork.CurrentRoom.IsVisible == false)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    private void ClearRoomListView() 
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry);
        }
        roomListEntries.Clear();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                continue;
            }

            GameObject objRoomListItem = Instantiate(roomListItem);
            objRoomListItem.transform.parent = content;
            objRoomListItem.transform.position = Vector3.one;
            objRoomListItem.GetComponent<RoomListItem>().Inicialize(info.Name, info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, objRoomListItem);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> list)
    {
        foreach (RoomInfo info in list)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }
}