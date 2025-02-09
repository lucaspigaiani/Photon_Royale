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
    private EnvironmentScanner environmentScanner;

    Quaternion targetRotation;

    bool isGrounded;
    bool hasControl = true;

    Vector3 desiredMoveDir;
    Vector3 moveDir;
    Vector3 velocity;

    public bool IsOnLedge {  get; set; }
    public LedgeData LedgeData { get; set; }

    float ySpeed;

    public float RotationSpeed => rotationSpeed;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");


        float moveAmount = Mathf.Clamp01(MathF.Abs(h) + MathF.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;
         
        desiredMoveDir = cameraController.planarRotation * moveInput;
        moveDir = desiredMoveDir;

        if (!hasControl)
            return;

        velocity = Vector3.zero;

        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded)
        {
            ySpeed = -0.5f;
            velocity = desiredMoveDir * moveSpeed;

            IsOnLedge = environmentScanner.LedgeCheck(desiredMoveDir, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
                Debug.Log("IsOnLedge");
            }

            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);

        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime; 

            velocity = transform.forward * moveSpeed / 2;
        }

        velocity.y = ySpeed;
        characterController.Move( velocity * Time.deltaTime);
        if (moveAmount > 0f && moveDir.magnitude > 0.2f)
        { 
            targetRotation = Quaternion.LookRotation(moveDir);
        }
            
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void GroundCheck() 
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void LedgeMovement() 
    {
        float angle = Vector3.Angle(LedgeData.surfaceHit.normal, desiredMoveDir);

        if (angle < 90) 
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
    }

    public void SetControl(bool hasControl, float moveAmount) 
    {

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", moveAmount);
            targetRotation = transform.rotation;
        }

        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        
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
