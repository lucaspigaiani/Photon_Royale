using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;

     
    public void KickPlayer(Player player)
    {
        
        // Verifica se o jogador atual � o MasterClient e se o alvo � v�lido
        if (PhotonNetwork.IsMasterClient && player != null && !player.IsLocal)
        {
            // Envia um RPC para o jogador alvo, for�ando-o a sair
            photonView.RPC(nameof(RPC_KickPlayer), player);
            Debug.Log($"Player {player.NickName} foi expulso!");
        }
        else
        {
            Debug.LogWarning("Apenas o MasterClient pode expulsar outros jogadores!");
        }
    }

    [PunRPC]
    private void RPC_KickPlayer()
    {
        Debug.Log("Voc� foi expulso da sala!");
        PhotonNetwork.LeaveRoom(); // For�a o jogador a sair
    }
}
