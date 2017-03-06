using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public float hp { get; set; }

    public float lerp = 1f;
    public float stepSize = 1f;

    public void MoveEtentity(Vector3 newPosition)
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, lerp);
    }

}
