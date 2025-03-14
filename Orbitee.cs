using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Orbitee : Actor_Base
{
    protected LayerMask orbitersCanAttack;

    public Transform orbitee;
    public Transform[] orbiters;

    public float[] orbitSpeeds;
    public float[] orbitRadii;
    public float[] orbitOffsets;

    protected override BoxCollider2D Coll => Coll;

    

    protected override void Start()
    {
        base.Start();
        
        for (int i = 0; i < orbiters.Length; i++)
        {
            float timeOffset = orbitOffsets[i] * (100f / orbitSpeeds[i]);
            orbiters[i].GetComponent<Orbiter>().startTime = Time.time + timeOffset;
        }
    }
    
    // put a while statement that the orbiters will only orbit when the player is within checklength range

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < orbiters.Length; i++)
        {
            float time = Time.time - orbiters[i].GetComponent<Orbiter>().startTime;
            float x = Mathf.Cos((time * orbitSpeeds[i]) * orbitRadii[i]);
            float y = Mathf.Sin((time * orbitSpeeds[i]) * orbitRadii[i]);
            Vector3 orbit = new Vector3(x, y, transform.position.z) * orbitRadii[i];
            orbiters[i].transform.position = orbitee.position + orbit;
        }
    }
}
