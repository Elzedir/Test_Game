using UnityEngine;

public class Game_Settings : MonoBehaviour
{
    private static Game_Settings instance;

    //public GameObject characterPrefab;

    public List_Item itemList;
    
    public List_Sprites[] sprites;

    public List_AnimatorControllers[] animatorControllers;

    public static Game_Settings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Game_Settings>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(Game_Settings).Name);
                    instance = singletonObject.AddComponent<Game_Settings>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
}