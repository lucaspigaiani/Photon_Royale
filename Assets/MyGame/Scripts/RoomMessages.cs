using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomMessages : MonoBehaviourPunCallbacks
{
    [Header("Message Components")]
    [SerializeField] private TextMeshProUGUI _messageText; // UI element to display messages
    [SerializeField] private float _messageDisplayDuration = 3f; // Duration to display the message

    [Header("Screen Messages")]
    private const string EnterMessageSuffix = " entered the room."; // Message when a player enters
    private const string LeaveMessageSuffix = " left the room."; // Message when a player leaves

    private void Awake()
    {
        // Ensure the message text is hidden initially
        _messageText.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Assign a random nickname to the player
        PhotonNetwork.NickName = "Player" + Random.Range(1, 1000);
    }

    /// <summary>
    /// Displays a message on the screen for a set duration.
    /// </summary>
    /// <param name="message">The message to display.</param>
    private void DisplayMessage(string message)
    {
        _messageText.text = message;
        _messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay());
    }

    /// <summary>
    /// Hides the message after a delay.
    /// </summary>
    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(_messageDisplayDuration);
        _messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when a player enters the room.
    /// </summary>
    /// <param name="newPlayer">The player who entered the room.</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayMessage(newPlayer.NickName + EnterMessageSuffix);
    }

    /// <summary>
    /// Called when a player leaves the room.
    /// </summary>
    /// <param name="otherPlayer">The player who left the room.</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisplayMessage(otherPlayer.NickName + LeaveMessageSuffix);
    }
}