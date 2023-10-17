using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sprite List", menuName = "Resources/SpriteList")]
public class SpriteGroup_SO : ScriptableObject
{
    public SpriteGroupName GroupName;
    public Thing[] Things;
}

[Serializable]
public class Thing
{
    public string ThingName;
    public Sprite[] ThingSprite;
}
