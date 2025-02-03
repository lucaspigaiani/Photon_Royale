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
        
        // Verifica se o jogador atual é o MasterClient e se o alvo é válido
        if (PhotonNetwork.IsMasterClient && player != null && !player.IsLocal)
        {
            // Envia um RPC para o jogador alvo, forçando-o a sair
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
        Debug.Log("Você foi expulso da sala!");
        PhotonNetwork.LeaveRoom(); // Força o jogador a sair
    }
}
