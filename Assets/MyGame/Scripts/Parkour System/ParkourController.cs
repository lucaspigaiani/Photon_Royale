using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    private EnvironmentScanner environmentScanner;
    private Animator animator;

    bool inAction;
    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if (Input.GetButton("Jump") && !inAction)
        {
            var hitData = environmentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                animator.CrossFade("StepUp", 0.2f);
            }
        }
        

    }
}
