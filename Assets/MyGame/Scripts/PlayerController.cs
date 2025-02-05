using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    Quaternion targetRotation;
    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");

        float moveAmount = MathF.Abs(h) + MathF.Abs(v);

        var moveInput = new Vector3(h, 0, v).normalized;
        var moveDir = cameraController.planarRotation * moveInput;
       
        if (moveAmount > 0)
        {
            
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        if (moveAmount > 0.5f)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        transform.rotation =  Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    #region Photon
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

    #endregion
}
