using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTargetMovement : MonoBehaviour
{
    public Transform[] pos;
    public float speed;

    int nextPosIndex;
    public Transform nextPos;
    // Start is called before the first frame update
    void Start()
    {
        nextPos = pos[0];
    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        if (transform.position == nextPos.position) 
        {
            nextPosIndex++;
            if(nextPosIndex >= pos.Length)
            {
                nextPosIndex = 0;
            }
            nextPos = pos[nextPosIndex];
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos.position, speed * Time.deltaTime);
        }

    }


}
