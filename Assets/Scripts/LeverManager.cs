using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverManager : MonoBehaviour
{
    public LeverSystem[] leverSys;
    public GameObject gate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(leverSys[0].pull == true && leverSys[1].pull == false && leverSys[2].pull == true && leverSys[3].pull == true)
        {
            Destroy(gate);
        }
    }
}
