using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SpriteGroupName
{
    None,
    Chest,
    WoodenSword
}

public abstract class List_Sprite
{
    public static List<SpriteGroup_SO> AllSpritesList;

    public static void InitialiseSprites()
    {
        AllSpritesList = new();
        AllSpritesList.Add(Resources.Load<SpriteGroup_SO>("Resources_Sprite/Chest"));
    }

    public static Thing GetSprite(SpriteGroupName groupName, string thingName)
    {
        foreach (SpriteGroup_SO spriteGroup in AllSpritesList)
        {
            if (spriteGroup.GroupName == groupName)
            {
                foreach (Thing thing in spriteGroup.Things)
                {
                    if (thing.ThingName == thingName)
                    {
                        return thing;
                    }
                }
            }
        }

        return null;
    }
}