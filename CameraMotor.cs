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
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    protected Vector3 _originalPosition;
    protected Vector3 _lastPos;
    protected Vector3 _nextPos;
    protected float _lastFoV;
    protected float _nextFoV;
    protected float _shakeTime;
    public bool DeltaMovement = true;
    public Vector3 Amount = new Vector3(1f, 1f, 0);
    public float Duration;
    public float Speed;
    protected Camera _camera;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _camera = GetComponent<Camera>();
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
        
        if (_shakeTime > 0)
        {
            _shakeTime -= Time.deltaTime;

            if (_shakeTime > 0)
            {
                _nextPos = (Mathf.PerlinNoise(_shakeTime * Speed, _shakeTime * Speed * 2) - 0.5f) * Amount.x * transform.right * Curve.Evaluate(1f - _shakeTime / Duration) +
                              (Mathf.PerlinNoise(_shakeTime * Speed * 2, _shakeTime * Speed) - 0.5f) * Amount.y * transform.up * Curve.Evaluate(1f - _shakeTime / Duration);

                _nextFoV = (Mathf.PerlinNoise(_shakeTime * Speed * 2, _shakeTime * Speed * 2) - 0.5f) * Amount.z * Curve.Evaluate(1f - _shakeTime / Duration);

                _camera.fieldOfView += (_nextFoV - _lastFoV);
                transform.Translate(DeltaMovement ? (_nextPos - _lastPos) : _nextPos);
                transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

                _lastPos = _nextPos;
                _lastFoV = _nextFoV;
            }
            else
            {
                ResetCameraShake();
            }
        }
    }

    public void ManualMove(Vector2 direction)
    {
        float moveSpeed = 5.0f;
        Vector3 move = new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.unscaledDeltaTime;
        transform.position += move;
    }

    public void ShakeOnce(float duration = 0.5f, float speed = 5f, Vector3? amount = null, Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
    {
        _originalPosition = transform.position;
        Duration = duration;
        Speed = speed;
        if (amount != null)
            Amount = (Vector3)amount;
        if (curve != null)
            Curve = curve;
        DeltaMovement = deltaMovement;

        ResetCameraShake();
        _shakeTime = Duration;
    }

    private void ResetCameraShake()
    {
        transform.Translate(DeltaMovement ? _lastPos : _originalPosition);
        _camera.fieldOfView -= _lastFoV;

        _lastPos = _nextPos = _originalPosition;
        _lastFoV = _nextFoV = 0f;
    }
}
