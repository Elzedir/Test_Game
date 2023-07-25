using UnityEngine;

public class SO_List : MonoBehaviour
{
    public static SO_List Instance;

    //public GameObject characterPrefab;

    public List_Item itemList;

    public List_Sprites[] weaponSprites;
    public List_Sprites[] armourSprites;
    public List_Sprites[] uiSprites;
    public List_Sprites[] chestSprites;

    public List_AnimatorControllers[] animatorControllers;
}