using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using Unity.VisualScripting;

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

    public bool InAction {  get; set; }
    public bool IsHanging {  get; set; }

    Vector3 desiredMoveDir;
    Vector3 moveDir;
    Vector3 velocity;

    public bool IsOnLedge {  get; set; }
    public LedgeData LedgeData { get; set; }

    float ySpeed;
    float moveAmount;

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


        moveAmount = Mathf.Clamp01(MathF.Abs(h) + MathF.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;
         
        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;

        if (!hasControl) return;

        if(IsHanging) return; 
        
        velocity = Vector3.zero;

        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded)
        {
            ySpeed = -0.5f;
            velocity = desiredMoveDir * moveSpeed;

            IsOnLedge = environmentScanner.ObstacleLedgeCheck(desiredMoveDir, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }

            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);

            if (moveAmount > 0f && moveDir.magnitude > 0.2f)
            {
                targetRotation = Quaternion.LookRotation(moveDir);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime; 

            velocity = transform.forward * moveSpeed / 2;
        }

        velocity.y = ySpeed;
        characterController.Move( velocity * Time.deltaTime);
        
    }

    private void GroundCheck() 
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void LedgeMovement() 
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
        float angle = MathF.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDir, transform.forward) >= 80)
        {
            velocity = Vector3.zero;
            return;
        }


        if (angle < 60) 
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
        else if (angle < 90)
        {
            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var dir = left * MathF.Sign(signedAngle);

            velocity = velocity.magnitude * dir;
            moveDir = dir;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams, Quaternion targetRotation, bool rotate=false, float postDelay=0f, bool mirror=false)
    {
        InAction = true;
        //playerController.SetControl(false, 0.21f);

        animator.SetBool("mirrorAction", mirror);
        animator.CrossFade(animName, 0.20f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(animName))
            Debug.LogError("The parkour animation is wrong");

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (rotate)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (matchParams != null)
                MatchTarget(matchParams);

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }
       /* if (postDelay == 0)
            SetControl(true, 0f);*/
        yield return new WaitForSeconds(postDelay);
        /*if (postDelay > 0)
            SetControl(true, 0f);*/
        InAction = false;

    }

    private void MatchTarget(MatchTargetParams mp)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart, new MatchTargetWeightMask(mp.posWeight, 0), mp.startTime, mp.targetTime);
    }

    public void SetControl(bool hasControl) 
    {

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }

        this.hasControl = hasControl;
        characterController.enabled = hasControl;
    }

    public bool HasControl { get => hasControl; set => hasControl = value; }

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
public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}
