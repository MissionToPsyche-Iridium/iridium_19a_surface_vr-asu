using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

public class spin : MonoBehaviour
{
    public int xSpin;
    public int ySpin;
    public int zSpin;

    private float xCount;
    private float yCount;
    private float zCount;
    private float xAngle;
    private float yAngle;
    private float zAngle;

    void Start()
    {
        // Keep the dimension from rotating if the spin is set to 0
        xCount = 0;
        yCount = 0;
        zCount = 0;
        xAngle = transform.rotation.x;
        yAngle = transform.rotation.y;
        zAngle = transform.rotation.z;
    }

    void Update()
    {
        // Change each dimension's rotation in the same speed as the spin set
        if(xSpin != 0) {
            xCount++;
            if(xCount == xSpin)
                xCount = 0;
            xAngle = (xCount/xSpin)*360;
        }
        if(ySpin != 0) {
            yCount++;
            if(yCount == ySpin)
                yCount = 0;
            yAngle = (yCount/ySpin)*360;
        }
        if(zSpin != 0) {
            zCount++;
            if(zCount == zSpin)
                zCount = 0;
            zAngle = (zCount/zSpin)*360;
        }
        transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
    }
}
