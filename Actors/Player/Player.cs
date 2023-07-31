using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    private Actor_Base _player;

    private void FixedUpdate()
    {
        if (_player == null || _player.gameObject != this.gameObject)
        {
            _player = GetComponent<Actor_Base>();
        }

        if (_player.dead)
        {
            Death();
        }
    }
    
    public void PlayerRespawn()
    {
        GameManager.Instance.PlayerDead = false;
        _player.StatManager.RestoreHealth(_player.StatManager.maxHealth);
        _player.PushDirection = Vector3.zero;
    }

    private void Death()
    {
        GameManager.Instance.PlayerDead = true;
        GameManager.Instance.deathMenuAnimator.SetTrigger("Show");
    }
    
}
