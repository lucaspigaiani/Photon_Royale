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

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    private Animator animator;
    private CharacterController characterController;

    Quaternion targetRotation;
    bool isGrounded;
    float ySpeed;

    
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");


        float moveAmount = Mathf.Clamp01(MathF.Abs(h) + MathF.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;
        var moveDir = cameraController.planarRotation * moveInput;

        GroundCheck();
        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime; 
        }


        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;
        characterController.Move( velocity * Time.deltaTime);

        if (moveAmount > 0.2)
        {
            
            targetRotation = Quaternion.LookRotation(moveDir);
           
        }

        transform.rotation =  Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        animator.SetFloat("moveAmount", moveAmount);
    }

    private void GroundCheck() 
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
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
