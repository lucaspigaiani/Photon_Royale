using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CanvasCharacterList : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        AssignCharacterIDs();
    }

    /// <summary>
    /// Assigns unique IDs to all character buttons.
    /// </summary>
    private void AssignCharacterIDs()
    {
        int idCounter = 1; // Start IDs from 1
        foreach (Transform characterTransform in transform)
        {
            CanvasCharacter character = characterTransform.GetComponent<CanvasCharacter>();
            if (character != null)
            {
                character.SetCharacterID(idCounter);
                idCounter++;
            }
        }
    }

    /// <summary>
    /// Called when a player's custom properties are updated.
    /// </summary>
    /// <param name="targetPlayer">The player whose properties were updated.</param>
    /// <param name="changedProps">The properties that were changed.</param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate called.");

        if (changedProps.TryGetValue("Character", out object characterObjID))
        {
            int characterID = (int)characterObjID;
            if (characterID > 0)
            {
                SelectCharacterForPlayer(targetPlayer, characterID);
            }
        }
    }

    /// <summary>
    /// Selects the character for the specified player based on the character ID.
    /// </summary>
    /// <param name="targetPlayer">The player who selected the character.</param>
    /// <param name="characterID">The ID of the selected character.</param>
    private void SelectCharacterForPlayer(Player targetPlayer, int characterID)
    {
        foreach (Transform characterTransform in transform)
        {
            CanvasCharacter character = characterTransform.GetComponent<CanvasCharacter>();
            if (character != null && character.CharacterID == characterID)
            {
                character.SelectCharacter(targetPlayer.NickName, targetPlayer.ActorNumber);
                break; // Exit after selecting the correct character
            }
        }
    }
}