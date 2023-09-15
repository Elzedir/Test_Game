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
        _playerActor.ActorScripts.StatManager.RestoreHealth(_playerActor.ActorScripts.StatManager.CurrentCombatStats.Health);
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
            float x = 0;
            float y = 0;

            if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Move_Up])) y = 1;
            if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Move_Down])) x = -1;
            if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Move_Left])) y = -1;
            if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Move_Right])) x = 1;

            var mouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            if (!_playerActor.ActorStates.Jumping)
            {
                Vector3 moveDirection = new Vector3(x, y, 0).normalized;
                Vector3 move = moveDirection * _playerActor.ActorScripts.StatManager.CurrentCombatStats.MoveSpeed;
                float speed = move.magnitude / Time.deltaTime;

                transform.localScale = new Vector3(_playerActor.OriginalSize.z * Mathf.Sign(mouseDirection.x), _playerActor.OriginalSize.y, _playerActor.OriginalSize.z);
                _playerActor.ActorScripts.Actor_VFX.transform.localScale = new Vector3(Mathf.Sign(mouseDirection.x), 1, 1);

                move += _playerActor.PushDirection;

                _playerActor.PushDirection = Vector3.Lerp(_playerActor.PushDirection, Vector3.zero, _playerActor.ActorData.ActorStats.CombatStats.PushRecovery);

                _hit = Physics2D.BoxCast(transform.position, _playerActor.ActorComponents.ActorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));

                if (_hit.collider == null)
                {
                    transform.Translate(0, move.y * Time.deltaTime, 0);
                }
                _hit = Physics2D.BoxCast(transform.position, _playerActor.ActorComponents.ActorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));

                if (_hit.collider == null)
                {
                    transform.Translate(move.x * Time.deltaTime, 0, 0);
                }

                _playerActor.ActorAnimator.SetFloat("Speed", speed);
            }
        }
    }

    public void ExecutePlayerAttack(float chargeTime)
    {
        if (!_playerActor.ActorStates.Attacking)
        {
            List<Equipment_Slot> equippedWeapons = Equipment_Manager.WeaponEquipped(PlayerActor);

            if (equippedWeapons.Count > 0)
            {
                foreach (var weapon in equippedWeapons)
                {
                    weapon.Attack(weapon, chargeTime);
                }
            }
            else if (equippedWeapons.Count == 0)
            {
                _playerActor.MainHand.Attack();
            }
        }
    }

    public void PlayerDodge()
    {
        _playerActor.Dodge();
    }
}
