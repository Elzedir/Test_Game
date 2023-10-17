using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEditor;
using Unity.VisualScripting;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;

public class Spawner_Tree : MonoBehaviour
{
    public List<Transform> AllTreeSpawners = new();
    [SerializeField] Spawner_Tree_GroupSO _treeGroup;

    [Range(0, 100)][SerializeField] private int _treeDensity;
    [Range(0f, 10f)][SerializeField] private float _treeSpawnRadius;

    void OnValidate()
    {
        EditorApplication.delayCall += DestroyAllChildren;
    }

    void DestroyAllChildren()
    {
        EditorApplication.delayCall -= DestroyAllChildren;

        foreach(Transform child in transform)
        {
            int childCount = child.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(child.transform.GetChild(i).gameObject);
            }
        }

        RefreshTrees();
    }

    void Start()
    {
        foreach(Transform child in transform)
        {
            AllTreeSpawners.Add(child);
            List<Transform> spawnedTrees = SpawnTrees(child);
            SortAndSetTrees(spawnedTrees);
        }
    }

    private void RefreshTrees()
    {
        AllTreeSpawners.Clear();

        foreach (Transform child in transform)
        {
            AllTreeSpawners.Add(child);
            List<Transform> spawnedTrees = SpawnTrees(child);
            SortAndSetTrees(spawnedTrees);
        }
    }

    List<Transform> SpawnTrees(Transform treeSpawner)
    {
        List<Transform> spawnedTrees = new List<Transform>();
        Vector3 centralPosition = treeSpawner.position;

        while (spawnedTrees.Count < _treeDensity)
        {
            float initialRadius = _treeSpawnRadius;
            float currentRadius = initialRadius;
            bool validPosition = false;
            float newX = 0, newY = 0;

            while (!validPosition && currentRadius >= 0)
            {
                float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
                float radius = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * currentRadius;
                float xCoord = centralPosition.x + radius * Mathf.Cos(angle);
                float yCoord = centralPosition.y + radius * Mathf.Sin(angle);

                float noiseFactorX, noiseFactorY;
                newX = xCoord; newY = yCoord;

                int attempts = 0;
                LayerMask blockingLayerMask = LayerMask.GetMask("Blocking");
                Vector2 originalPosition = new Vector2(centralPosition.x, centralPosition.y);
                Vector2 newPosition;

                do
                {
                    int attemptsX = 0, attemptsY = 0;

                    do
                    {
                        noiseFactorX = (Mathf.PerlinNoise(xCoord + (UnityEngine.Random.Range(0f, 1f)), yCoord) - 0.5f) * currentRadius;
                        newX = xCoord + noiseFactorX;
                        attemptsX++;
                    }
                    while (attemptsX < 5);

                    do
                    {
                        noiseFactorY = (Mathf.PerlinNoise(yCoord + (UnityEngine.Random.Range(0f, 1f)), xCoord) - 0.5f) * currentRadius;
                        newY = yCoord + noiseFactorY;
                        attemptsY++;
                    }
                    while (attemptsY < 5);

                    newPosition = new Vector2(newX, newY);
                    validPosition = !Physics2D.Linecast(originalPosition, newPosition, blockingLayerMask);
                    attempts++;

                } while (!validPosition && attempts < 20);

                if (attempts >= 20)
                {
                    currentRadius -= 0.1f;

                    if (currentRadius <= 0f)
                    {
                        Debug.Log("Original tree position is within a blocking layer.");
                        break;
                    }
                }
            }

            if (!validPosition && currentRadius == 0 && newX == 0 && newY == 0)
            {
                Debug.Log("While loop didn't run.");
            }


            _treeSpawnRadius = initialRadius;

            Tree selectedSprite = _treeGroup.Trees[UnityEngine.Random.Range(0, _treeGroup.Trees.Count)];

            GameObject treeTop = new GameObject();
            treeTop.transform.parent = treeSpawner;
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

            spawnedTrees.Add(treeTop.transform);
            spawnedTrees.Add(treeBottom.transform);
        }
        return spawnedTrees;
    }

    void SortAndSetTrees(List<Transform> spawnedTrees)
    {
        var sortedTrees = spawnedTrees.OrderByDescending(t => t.position.y).ThenByDescending(t => t.position.x).ToList();

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