using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public static CameraMotor Instance;
    private Transform lookAt;
    public Player _player;
    public float boundX = 0.15f;
    public float boundY = 0.05f;
    public bool PlayerCameraEnabled = true;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void LateUpdate()
    {
        if (_player == null || _player.gameObject != GameManager.Instance.Player.gameObject)
        {
            _player = GameManager.Instance.Player;
            lookAt = _player.transform;
        }

        if (lookAt != null && PlayerCameraEnabled)
        {
            Vector3 delta = Vector3.zero;

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

    public void ManualMove(Vector2 direction)
    {
        float moveSpeed = 5.0f;
        Vector3 move = new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.unscaledDeltaTime;
        transform.position += move;
    }
}
