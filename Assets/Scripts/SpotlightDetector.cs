using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightDetector : MonoBehaviour
{
    //FOV
    public float radius;
    [Range(0,360)]
    public float angle;

    //Target
    public GameObject playerRef;
    public Transform playerPos;
    public GameObject rotateRef;
    public Transform rotateTarget;

    //Masks
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    //Checker
    public bool canSeePlayer;

    // Start is called before the first frame update
    private void Start()
    {
        rotateRef = GameObject.Find("RotateTarget");
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());

        canSeePlayer = false;
        RotateSpotlight();
    }

    private IEnumerator FOVRoutine()
    {
        
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        
        while (true)
        {
            yield return wait;

            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (!canSeePlayer)
        {
            RotateSpotlight();
        }

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    if (canSeePlayer)
                    {
                        ShootPlayer();
                    }
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if(canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void ShootPlayer()
    {
        transform.LookAt(playerPos);
    }

    private void RotateSpotlight()
    {
        transform.LookAt(rotateTarget);
    }
}
