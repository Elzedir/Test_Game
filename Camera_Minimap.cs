using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Minimap : MonoBehaviour
{
    public Transform target; // The game object to track (e.g., player, enemy)
    public RectTransform icon; // The UI element representing the target on the minimap

    void Update()
    {
        Vector2 newPosition = Camera.main.WorldToViewportPoint(target.position);
        icon.anchoredPosition = new Vector2(newPosition.x * Screen.width, newPosition.y * Screen.height);
    }
}
