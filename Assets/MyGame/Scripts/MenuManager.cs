using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    // Singleton instance
    public static MenuManager Instance { get; private set; }

    private const string PlayerNameKey = "PlayerName";

    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private GameObject roomPanel;

    [Header("Room List UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject roomListItem;
    
    [Header("Player List UI")]
    [SerializeField] private Transform playerLobbyListContent;
    [SerializeField] private GameObject playerListItem;


    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI infoStatus;
    [SerializeField] private TextMeshProUGUI infoPlayerLobby;
    [SerializeField] private TextMeshProUGUI infoPlayerGame;

    private string infoPlayerLobbyTxt = "Players in lobby: ";
    private string infoPlayerGameTxt = "Players in game: ";

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListEntries = new Dictionary<string, GameObject>();


    private void Awake()
    {
        // Implementing the Singleton pattern:
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optional: If you want the MenuManager to persist across scenes:
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            playerNameInput.text = PlayerPrefs.GetString(PlayerNameKey);
        }
        EnableCanvas(loginPanel.name);
    }

    private void FixedUpdate()
    {
        infoStatus.text = PhotonNetwork.NetworkClientState.ToString();
        infoPlayerLobby.text = infoPlayerLobbyTxt + PhotonNetwork.CountOfPlayersOnMaster.ToString();
        infoPlayerGame.text = infoPlayerGameTxt + PhotonNetwork.CountOfPlayersInRooms.ToString();
    }

    private void EnableCanvas(string activePanel)
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));
        roomPanel.SetActive(activePanel.Equals(roomPanel.name));
    }

    public void SignIn()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.Log("Enter name player");
            return;
        }

        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString(PlayerNameKey, playerNameInput.text);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ShowRoomListPanel()
    {
        EnableCanvas(roomListPanel.name);
    }

    public void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public void OnJoinedRoom()
    {
        EnableCanvas(roomPanel.name);

    }

    public void UpdatePlayerList(Player[] players)
    {
        foreach (Transform child in playerLobbyListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in players)
        {
            GameObject listItem = Instantiate(playerListItem, playerLobbyListContent);
            listItem.GetComponent<PlayerListItem>().Initialize(player);
        }
    }


    #region Room List Management

    public void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry);
        }
        roomListEntries.Clear();
    }

    public void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                continue;
            }

            GameObject objRoomListItem = Instantiate(roomListItem, content);
            objRoomListItem.transform.localPosition = Vector3.one;
            objRoomListItem.GetComponent<RoomListItem>().Inicialize(info.Name, info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, objRoomListItem);
        }
    }

    public void UpdateCachedRoomList(List<RoomInfo> list)
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

    #endregion
}
