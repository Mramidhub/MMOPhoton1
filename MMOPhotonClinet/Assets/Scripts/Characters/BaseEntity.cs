using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public float hp { get; set; }

    public float lerp = 1f;
    public float stepSize = 1f;
    protected Vector3 newPosition = new Vector3();
    protected bool move;

    
    public void MoveEtentity(Vector3 newPos)
    {
        newPosition = newPos;
        move = true;
    }


    public void DestroyEntity()
    {
        Destroy(gameObject);
    }

}
