using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
        // if player presses D key
        if (Input.GetKey("D"))
        {
            // then set the isWalking boolean to be true
            animator.SetBool("isWalking", true);
        }
    }
}
