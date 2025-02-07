using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CanvasCharacter : MonoBehaviour, IPointerClickHandler
{
    [Header("Character Properties")]
    public int CharacterID = 0; // Unique ID for the character
    [SerializeField] private bool _isSelected; // Whether the character is currently selected
    [SerializeField] private string _playerOwnerName = ""; // Name of the player who selected this character
    [SerializeField] private int _playerOwnerID = 0; // ID of the player who selected this character

    [Header("UI Components")]
    private TextMeshProUGUI _descriptionText; // Text to display the player's name
    private Button _characterButton; // Button component for the character

    private void Start()
    {
        InitializeCharacter();
    }

    /// <summary>
    /// Initializes the character's state and UI components.
    /// </summary>
    private void InitializeCharacter()
    {
        _isSelected = true;
        _descriptionText = GetComponentInChildren<TextMeshProUGUI>();
        _characterButton = GetComponent<Button>();
    }

    /// <summary>
    /// Sets the character's unique ID.
    /// </summary>
    /// <param name="id">The ID to assign to the character.</param>
    public void SetCharacterID(int id)
    {
        CharacterID = id;
    }

    /// <summary>
    /// Handles the click event on the character button.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Character ID: " + CharacterID);

        if (!_isSelected)
        {
            return; // Do nothing if the character is already selected
        }

        // Update the player's custom properties to select this character
        Hashtable playerProperties = new Hashtable { { "Character", CharacterID } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    /// <summary>
    /// Selects the character for a specific player.
    /// </summary>
    /// <param name="playerName">The name of the player selecting the character.</param>
    /// <param name="playerID">The ID of the player selecting the character.</param>
    public void SelectCharacter(string playerName, int playerID)
    {
        ResetPreviouslySelectedCharacter(playerID); // Reset the player's previous selection

        // Update the character's state and UI
        _isSelected = false;
        _playerOwnerName = playerName;
        _playerOwnerID = playerID;
        _descriptionText.text = playerName;
        _descriptionText.color = GetPlayerColor(playerID);
        _characterButton.interactable = false;
    }

    /// <summary>
    /// Resets the character's state and UI to default.
    /// </summary>
    public void ResetCharacter()
    {
        _isSelected = true;
        _playerOwnerName = "";
        _playerOwnerID = 0;
        _descriptionText.text = "";
        _descriptionText.color = GetPlayerColor(0); // Default color
        _characterButton.interactable = true;
    }

    /// <summary>
    /// Resets the character previously selected by the player.
    /// </summary>
    /// <param name="playerID">The ID of the player whose previous selection should be reset.</param>
    private void ResetPreviouslySelectedCharacter(int playerID)
    {
        Transform buttonsParent = transform.parent;

        foreach (Transform child in buttonsParent)
        {
            CanvasCharacter character = child.GetComponent<CanvasCharacter>();
            if (character != null && character._playerOwnerID == playerID)
            {
                character.ResetCharacter(); // Reset the character
                break; // Exit after resetting the correct character
            }
        }
    }

    /// <summary>
    /// Returns a color based on the player's ID.
    /// </summary>
    /// <param name="playerID">The ID of the player.</param>
    /// <returns>The color associated with the player ID.</returns>
    private Color GetPlayerColor(int playerID)
    {
        switch (playerID)
        {
            case 1: return Color.red;
            case 2: return Color.blue;
            case 3: return Color.green;
            case 4: return Color.yellow;
            default: return Color.white;
        }
    }
}