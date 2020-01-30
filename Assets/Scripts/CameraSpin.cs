using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public int spinSpeed = 50;
    public Transform target;

    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
        //transform.RotateAround(target.position, Vector3.up, spinSpeed * Time.deltaTime);
    }
}
