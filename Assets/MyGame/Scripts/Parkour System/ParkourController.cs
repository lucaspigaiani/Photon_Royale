using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> parkourActions;
    [SerializeField] private ParkourAction jumpDownAction;
    [SerializeField] private float autoDropHeightLimit = 1f;
   
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

        if (Input.GetButton("Jump") && !playerController.InAction && !playerController.IsHanging)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        playerController.InAction = true;
                        break;
                    }
                }
            }
        }
        if (playerController.IsOnLedge && !playerController.InAction && hitData.forwardHitFound == false && Input.GetButton("Jump"))
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
        playerController.SetControl(false);

        MatchTargetParams matchparams = null;
        if (action.EnableTargetMatching) 
        {
            matchparams = new MatchTargetParams()
            {
                pos = action.MatchPos,
                bodyPart = action.MatchBodyPart,
                posWeight = action.MatchPosWeight,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime
            };

        }

        yield return playerController.DoAction(action.AnimName, matchparams, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.Mirror);
        playerController.SetControl(true);

    }
}
