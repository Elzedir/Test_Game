using UnityEngine;

public class SO_List : MonoBehaviour
{
    private static SO_List _instance;

    //public GameObject characterPrefab;

    public List_Item itemList;

    public List_Sprites[] WeaponMeleeSprites;
    public List_Sprites[] WeaponRangedSprites;
    public List_Sprites[] ArmourSprites;
    public List_Sprites[] UiSprites;
    public List_Sprites[] ChestSprites;
    public GameObject[] Prefabs;

    public List_AnimatorControllers[] animatorControllers;

    public static SO_List Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SO_List>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SO_List).Name);
                    _instance = singletonObject.AddComponent<SO_List>();
                }
            }

            return _instance;
        }
    }
}