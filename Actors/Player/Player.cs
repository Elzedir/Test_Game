using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    private Actor_Base _player;
    public float currentYSpeed = 1.0f;
    public float currentXSpeed = 1.0f;
    private RaycastHit2D _hit;

    private void FixedUpdate()
    {
        if (_player == null || _player.gameObject != this.gameObject)
        {
            _player = GetComponent<Actor_Base>();
        }

        if (_player.ActorStates.Dead)
        {
            Death();
        }
    }
    
    public void PlayerRespawn()
    {
        GameManager.Instance.PlayerDead = false;
        _player.ActorScripts.StatManager.RestoreHealth(_player.ActorScripts.StatManager.maxHealth);
        _player.PushDirection = Vector3.zero;
    }

    private void Death()
    {
        GameManager.Instance.PlayerDead = true;
        GameManager.Instance.deathMenuAnimator.SetTrigger("Show");
    }

    public virtual void PlayerMove()
    {
        if (!_player.ActorStates.Dead)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            if (!_player.ActorStates.Jumping)
            {
                Vector3 move = new Vector3(x * currentXSpeed, y * currentYSpeed, 0);
                transform.localScale = new Vector3(_player.OriginalSize.z * Mathf.Sign(direction.x), _player.OriginalSize.y, _player.OriginalSize.z);
                _player.ActorScripts.Actor_VFX.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

                move += _player.PushDirection;

                _player.PushDirection = Vector3.Lerp(_player.PushDirection, Vector3.zero, _player.ActorData.pushRecoverySpeed);

                _hit = Physics2D.BoxCast(transform.position, _player.ActorComponents.ActorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));
                if (_hit.collider == null)
                {
                    transform.Translate(0, move.y * Time.deltaTime, 0);
                    _player.SetMovementSpeed(move.magnitude);
                }
                _hit = Physics2D.BoxCast(transform.position, _player.ActorComponents.ActorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));
                if (_hit.collider == null)
                {
                    transform.Translate(move.x * Time.deltaTime, 0, 0);
                    _player.SetMovementSpeed(move.magnitude);
                }
            }
        }
    }
}
