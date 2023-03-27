using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public bool isCollided;
    public GameObject door;

    public int e;
    public float doorRotation;

    private void Update()
    {
        if (e == 1)
        {

            if (doorRotation >= -0.25)
            {
                doorRotation -= (0.05f * 0.1f);
                door.transform.Rotate(0, 0, -1 * 1.7f);
            }
        }
        else
        {

            door.transform.Rotate(0, 0, 0);

        }
    }
    private void OnCollisionStay(Collision other)
    {

        if (other.gameObject.tag == "Switch1")
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                e = 1;
                
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Switch1")
        {

        }
       
    }
}
