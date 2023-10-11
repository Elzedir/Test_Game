using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using static Unity.VisualScripting.Metadata;
using UnityEngine.Tilemaps;
using TreeEditor;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class Spawner_Tree : MonoBehaviour
{
    Transform _treeParent;
    public List<Transform> AllTrees = new();

    void Start()
    {
        _treeParent = GameObject.Find("TreeParent").transform;

        Tilemap forestTilemap = GameObject.Find("ForestTilemap").GetComponent<Tilemap>();
        Spawner_Tree_GroupSO forestGroup = Resources.Load<Spawner_Tree_GroupSO>("Resources_Environment/ForestTreeGroup");
        
        SpawnTrees(forestTilemap, forestGroup);

        SortAndSetTrees();
    }

    void SpawnTrees(Tilemap tilemap, Spawner_Tree_GroupSO treeGroup)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        Vector2 boundarySize = tilemap.cellSize;

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile == null) continue;

                Vector3Int cellPosition = new Vector3Int(x + bounds.x, y + bounds.y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
                int spawnedTrees = 0;

                float stepX = boundarySize.x / Mathf.Sqrt(treeGroup.TreeDensity);
                float stepY = boundarySize.y / Mathf.Sqrt(treeGroup.TreeDensity);

                Vector2 startPos = (Vector2)worldPosition - boundarySize / 2f;

                for (float xCoord = startPos.x; xCoord <= startPos.x + boundarySize.x; xCoord += stepX)
                {
                    for (float yCoord = startPos.y; yCoord <= startPos.y + boundarySize.y; yCoord += stepY)
                    {
                        if (spawnedTrees >= treeGroup.TreeDensity) break;

                        int attemptsX = 0;
                        float noiseFactorX;
                        float newX;

                        do
                        {
                            noiseFactorX = (Mathf.PerlinNoise(xCoord + (UnityEngine.Random.Range(0f, 1f) * treeGroup.TreePerlinMultiplier), yCoord) - 0.5f);
                            newX = xCoord + noiseFactorX;
                            attemptsX++;
                        }
                        while ((newX < startPos.x || newX > startPos.x + boundarySize.x) && attemptsX < 5);

                        int attemptsY = 0;
                        float noiseFactorY;
                        float newY;

                        do
                        {
                            noiseFactorY = (Mathf.PerlinNoise(yCoord + (UnityEngine.Random.Range(0f, 1f) * treeGroup.TreePerlinMultiplier), xCoord) - 0.5f);
                            newY = yCoord + noiseFactorY;
                            attemptsY++;
                        }
                        while ((newY < startPos.y || newY > startPos.y + boundarySize.y) && attemptsY < 5);

                        Tree selectedSprite = treeGroup.Trees[UnityEngine.Random.Range(0, treeGroup.Trees.Count)];

                        GameObject treeTop = new GameObject();
                        treeTop.transform.parent = _treeParent;
                        treeTop.transform.position = new Vector3(newX, newY, 0);

                        SpriteRenderer treeTopSprite = treeTop.AddComponent<SpriteRenderer>();
                        treeTopSprite.sprite = selectedSprite.TreeSprites[0];
                        treeTopSprite.sortingLayerName = "Trees_Top";

                        float halfHeightOfTop = treeTopSprite.sprite.bounds.size.y / 2;

                        GameObject treeBottom = new GameObject();
                        treeBottom.transform.parent = treeTop.transform;
                        treeBottom.transform.position = new Vector3(newX, newY - halfHeightOfTop, 0);

                        SpriteRenderer treeBottomSprite = treeBottom.AddComponent<SpriteRenderer>();
                        treeBottomSprite.sprite = selectedSprite.TreeSprites[1];
                        treeBottomSprite.sortingLayerName = "Trees_Bottom";

                        AllTrees.Add(treeTop.transform);
                        AllTrees.Add(treeBottom.transform);
                        spawnedTrees++;
                    }
                }

                tilemap.SetTile(cellPosition, null);
            }
        }
    }

    void SortAndSetTrees()
    {
        var sortedTrees = AllTrees.OrderByDescending(t => t.position.y).ThenByDescending(t => t.position.x).ToList();

        for (int i = 0; i < sortedTrees.Count; i++)
        {
            SpriteRenderer sr = sortedTrees[i].GetComponent<SpriteRenderer>();

            if (sr == null) continue;

            if (sr.sortingLayerName == "Trees_Top")
            {
                sortedTrees[i].name = $"Tree_Top_{i}";
            }
            else
            {
                sortedTrees[i].name = $"Tree_Bottom_{i}";
            }
            
            sr.sortingOrder = i;
        }
    }
}