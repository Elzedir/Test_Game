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
    private Actor_Base _playerActor;
    public Actor_Base PlayerActor { get { return _playerActor; } }

    public float currentYSpeed = 1.0f;
    public float currentXSpeed = 1.0f;
    private RaycastHit2D _hit;

    private void FixedUpdate()
    {
        if (_playerActor == null || _playerActor.gameObject != this.gameObject)
        {
            _playerActor = GetComponent<Actor_Base>();
        }

        if (_playerActor.ActorStates.Dead)
        {
            Death();
        }
    }
    
    public void PlayerRespawn()
    {
        GameManager.Instance.PlayerDead = false;
        _playerActor.ActorScripts.StatManager.RestoreHealth(_playerActor.ActorScripts.StatManager.maxHealth);
        _playerActor.PushDirection = Vector3.zero;
    }

    private void Death()
    {
        GameManager.Instance.PlayerDead = true;
        GameManager.Instance.deathMenuAnimator.SetTrigger("Show");
    }

    public virtual void PlayerMove()
    {
        if (!_playerActor.ActorStates.Dead)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            if (!_playerActor.ActorStates.Jumping)
            {
                Vector3 move = new Vector3(x * currentXSpeed, y * currentYSpeed, 0);
                transform.localScale = new Vector3(_playerActor.OriginalSize.z * Mathf.Sign(direction.x), _playerActor.OriginalSize.y, _playerActor.OriginalSize.z);
                _playerActor.ActorScripts.Actor_VFX.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

                move += _playerActor.PushDirection;

                _playerActor.PushDirection = Vector3.Lerp(_playerActor.PushDirection, Vector3.zero, _playerActor.ActorData.pushRecoverySpeed);

                _hit = Physics2D.BoxCast(transform.position, _playerActor.ActorComponents.ActorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));
                if (_hit.collider == null)
                {
                    transform.Translate(0, move.y * Time.deltaTime, 0);
                    _playerActor.SetMovementSpeed(move.magnitude);
                }
                _hit = Physics2D.BoxCast(transform.position, _playerActor.ActorComponents.ActorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));
                if (_hit.collider == null)
                {
                    transform.Translate(move.x * Time.deltaTime, 0, 0);
                    _playerActor.SetMovementSpeed(move.magnitude);
                }
            }
        }
    }

    public void PlayerAttack()
    {
        List<Equipment_Slot> equippedWeapons = _playerActor.ActorScripts.EquipmentManager.WeaponEquipped();

        if (equippedWeapons.Count > 0)
        {
            foreach (var weapon in equippedWeapons)
            {
                weapon.Attack();
            }
        }
        else
        {
            _playerActor.MainHand.UnarmedAttack();
        }
    }
}
