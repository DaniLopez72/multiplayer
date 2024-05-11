using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform target;
     float currentHeight, finalHeight;
     float currentRotation, finalRotation;

    public float height, distance;
    public float speedMove, speedRotation;

    public void SetTarget(Transform target)
    {
        this.target = target; 
    }
    

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        currentRotation= transform.eulerAngles.y;
        finalRotation = target.eulerAngles.y;

        currentHeight=transform.position.y;
        finalHeight=target.position.y+height;

        currentHeight = Mathf.Lerp(currentHeight, finalHeight, speedMove*Time.deltaTime);
        currentRotation = Mathf.LerpAngle(currentRotation,finalRotation, speedRotation*Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);

        transform.position = target.position;
        transform.position -= rotation * Vector3.forward * distance;

        Vector3 position = transform.position;
        position.y = currentHeight;
        transform.position = position;

        transform.LookAt(target);
    }
}
