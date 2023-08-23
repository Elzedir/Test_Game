using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "New Patrol Data", menuName = "Character/Patrol Data")]
public class Actor_PatrolData_SO : ScriptableObject
{
    public Vector2[] PatrolPoints;
    public float PatrolSpeed = 1.0f;
    public float WaitTimeAtPoint = 1.0f;
}