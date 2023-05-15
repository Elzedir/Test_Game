using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Dissolve : MonoBehaviour
{
    Material material;

    bool isDissolving = false;
    float fade = 1f;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        if (isDissolving)
        {
            fade -= Time.deltaTime;

            if (fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
            }
            material.SetFloat("_Fade", fade);
        }
    }

    public void Dissolve()
    {
        // call the dissolve effect from somewhere else
        isDissolving = true;
    }
}
