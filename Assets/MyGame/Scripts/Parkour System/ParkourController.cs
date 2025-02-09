using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> parkourActions;
    [SerializeField] private ParkourAction jumpDownAction;
   
    bool inAction = false;

    private PlayerController playerController;
    private EnvironmentScanner environmentScanner;
    private Animator animator;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (Input.GetButton("Jump") && !inAction)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
        if (playerController.IsOnLedge && inAction == false && hitData.forwardHitFound == false && Input.GetButton("Jump"))
        {
            if (playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }
    }

    private IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        playerController.SetControl(false, 0.21f);

        animator.SetBool("mirrorAction", action.Mirror);
        animator.CrossFade(action.AnimName, 0.20f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.AnimName))
            Debug.LogError("The parkour animation is wrong");

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (action.RotateToObstacle)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);

            if(action.EnableTargetMatching)
                MatchTarget(action);

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }
        if(action.PostActionDelay == 0)
            playerController.SetControl(true, 0f);
        yield return new WaitForSeconds(action.PostActionDelay);
        if (action.PostActionDelay > 0)
            playerController.SetControl(true, 0f);
        inAction = false;

    }

    private void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, new MatchTargetWeightMask(action.MatchPosWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
