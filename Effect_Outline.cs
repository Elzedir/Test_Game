using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect_Outline : MonoBehaviour
{
    Material material;
    SpriteRenderer spriteRenderer;

    bool canInteract = false;
    bool isOutlined = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    private void Update()
    {
        if (canInteract)
        {
            if (!isOutlined)
            {
                //Use the scriptable object
                //material = Resources.Load<Material>("Shader_Graphs/Outline");

                isOutlined = true;
            }            
        }
        else
        {
            isOutlined = false;
        }
    }

    public void Outline()
    {
        // call the outline effect from somewhere else
        canInteract = true;
    }
}
