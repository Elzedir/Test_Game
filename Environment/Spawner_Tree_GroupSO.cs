using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TreeGroup", menuName = "Environment/Tree Group")]
public class Spawner_Tree_GroupSO : ScriptableObject
{
    public List<Tree> Trees;
}

[Serializable]
public class Tree
{
    public List<Sprite> TreeSprites;
}
