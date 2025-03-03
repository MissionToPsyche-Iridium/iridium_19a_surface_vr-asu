using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustToGround : MonoBehaviour
{

    [SerializeField] private float groundRaycastDistance = 2f;
    [SerializeField] private float rotationChangeSpeed = 2f;
    // Update is called once per frame
    void Update()
    {
        RaycastHit belowHit;
        Quaternion slopeRotationFactor = Quaternion.FromToRotation(Vector3.up, transform.up);
        Vector3 groundOriginOffset = new Vector3(0, 0, 0);
        if (Physics.Raycast(transform.position + (slopeRotationFactor * groundOriginOffset), -transform.up, out belowHit, groundRaycastDistance))
        {
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, belowHit.normal), belowHit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationChangeSpeed * Time.deltaTime);
        }
    }
}
