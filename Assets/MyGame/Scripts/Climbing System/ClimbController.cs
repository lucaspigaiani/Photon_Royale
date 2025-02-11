using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    ClimbPoint currentPoint;

    EnvironmentScanner envScanner;
    PlayerController playerController;

    private void Awake()
    {
        envScanner = GetComponent<EnvironmentScanner>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!playerController.InAction)
        {
            if (Input.GetButton("Jump"))
            {
                if (envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("IdleToHang", ledgeHit.transform, 0.41f, 0.54f));
                }
            }
        }
        else 
        {
            // ledge to ledge jump
            float h = MathF.Round(Input.GetAxisRaw("Horizontal"));
            float v = MathF.Round(Input.GetAxisRaw("Vertical"));
            var inputDir = new Vector2(h, v);

            if (playerController.InAction || inputDir == Vector2.zero) return;

            var neighbour = currentPoint.GetNeighbour(inputDir);
            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentPoint = neighbour.point;

                if (neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge("HangHopUp", currentPoint.transform, 0.35f, 0.65f));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f));
            }


        }
    }

    IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    {
        var matchParams = new MatchTargetParams()
        {
            pos = GetHandPos(ledge),
            bodyPart = AvatarTarget.RightHand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one
        };

        var targetRot = Quaternion.LookRotation(-ledge.forward);
        yield return playerController.DoAction(anim, matchParams, targetRot, true);

        playerController.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge)
    {
        return ledge.position + ledge.forward * 0.1f + Vector3.up * 0.1f - ledge.right * 0.25f;
    }
}
