using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseEntity
{
    public int idClient = 0;


    void FixedUpdate()
    {
        if (move)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, lerp);
            if (transform.position == newPosition)
            {
                move = false;
            }
        }

    }
}
