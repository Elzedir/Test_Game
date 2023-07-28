using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class Interactable_Manager : MonoBehaviour
{
    public static Interactable_Manager Instance;
    public List<GameObject> InteractableObjects = new();
    public GameObject _interactedObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        Player player = GameManager.Instance.player;

        if (player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        foreach (GameObject interactable in InteractableObjects)
        {
            if (interactable != null)
            {
                float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
                float maxDistance = 10;

                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    _interactedObject = interactable;
                }
            }
        }

        // Somehow then call the interact function through whatever script is attached to the item. Find a way to make it replaceable. Maybe generic?
    }

    public void ChangeInteractedObject<T>() where T : MonoBehaviour
    {
        // A generic function for left or right where you can Sort the interactable by distance and then go up or down the list by calling the function in a certain direction
    }
}
