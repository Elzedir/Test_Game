using UnityEngine;

public class SO_List : MonoBehaviour
{
    private static SO_List instance;

    //public GameObject characterPrefab;

    public List_Item itemList;
    
    public List_Sprites[] sprites;

    public List_AnimatorControllers[] animatorControllers;

    public static SO_List Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SO_List>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SO_List).Name);
                    instance = singletonObject.AddComponent<SO_List>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
}