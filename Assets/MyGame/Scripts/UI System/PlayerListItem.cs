using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private GameObject kickoutButton;

    private Player player;

    public void Initialize(Player playerData)
    {
        player = playerData;
        playerNameTxt.text = player.NickName;

        // Apenas o Master Client pode ver o botão de expulsar
        kickoutButton.SetActive(PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.UserId == playerData.UserId)
        {
            kickoutButton.SetActive(false);
        }
    }

    public void KickPlayer()
    {
        GameObject.Find(PhotonNetwork.LocalPlayer.UserId).GetComponent<PlayerController>().KickPlayer(player);
    }



}
