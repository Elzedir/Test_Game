using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    public Player player;
    public float boundX = 0.15f;
    public float boundY = 0.05f;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        if (player != null)
        {
            lookAt = player.transform;
        }
    }

    private void LateUpdate()
    {
        
        // Need to replace this with an event or delegate which will change the camara focus on playerscript change.
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            lookAt = player.transform;
        }

        if (lookAt != null)
        {
            Vector3 delta = Vector3.zero;

            // This is to check if we're inside the bounds on the X axis.
            float deltaX = lookAt.position.x - transform.position.x;
            if (deltaX > boundX || deltaX < -boundX)
            {
                if (transform.position.x < lookAt.position.x)
                {
                    delta.x = deltaX - boundX;
                }
                else
                {
                    delta.x = deltaX + boundX;
                }
            }

            // This is to check if we're inside the bounds on the Y axis.
            float deltaY = lookAt.position.y - transform.position.y;
            if (deltaY > boundY || deltaY < -boundY)
            {
                if (transform.position.y < lookAt.position.y)
                {
                    delta.y = deltaY - boundY;
                }
                else
                {
                    delta.y = deltaY + boundY;
                }
            }

            transform.position += new Vector3(delta.x, delta.y, 0);

        }
    }
}
