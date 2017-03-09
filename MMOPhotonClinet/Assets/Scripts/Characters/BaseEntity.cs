using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public float hp { get; set; }

    public float lerp = 10f;
    public float stepSize = 1f;
    public float rotateSpeed = 7f;
    protected Vector3 newPosition = new Vector3();
    protected Quaternion newRotation = new Quaternion();

    
    public void MoveEtentity(Vector3 newPos)
    {
        newPosition = newPos;
     }

    public void RotateEtentuty(Quaternion newRot)
    {
        newRotation = newRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }


    public void RotateEtentuty()
    {
        newRotation = Quaternion.LookRotation(newPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }

    public void DestroyEntity()
    {
        Destroy(gameObject);
    }

    public void UpdateRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }

    public void UpdatePostion()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, lerp);
    }
}
