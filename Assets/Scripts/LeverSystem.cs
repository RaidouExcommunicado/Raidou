using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverSystem : MonoBehaviour
{
    public Collider switchCol;
    public bool pull;

    private void Awake()
    {
        pull = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!pull)
        {
            if (other.tag == "Player")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    pull = true;
                    
                }
            }
        }    
        else if (pull)
        {
            if (other.tag == "Player")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    pull = false;
                    
                }
            }
        }  
    }
}
