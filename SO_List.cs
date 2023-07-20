using UnityEngine;

public class SO_List : MonoBehaviour
{
    private static SO_List instance;

    //public GameObject characterPrefab;

    public List_Item itemList;
    
    public List_Sprites[] weaponSprites;
    public List_Sprites[] armourSprites;
    public List_Sprites[] uiSprites;

    public List_AnimatorControllers[] animatorControllers;

    public static SO_List Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SO_List>();

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