using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustToGround : MonoBehaviour
{
    


    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0f))
        {
            Debug.Log(hit.normal);
        }

    }
}
