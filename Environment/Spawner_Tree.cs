using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class Spawner_Tree : MonoBehaviour
{
    public float TreeDensity = 10;
    public float PerlinNoiseMultiplier = 0.05f;
    public Spawner_Tree_GroupSO TreeGroup;

    private void Start()
    {
        SpawnTrees();
    }

    public void SpawnTrees()
    {
        int wholeNumber = Mathf.FloorToInt(TreeDensity);

        Debug.Log($"Tree Density Number: {wholeNumber}");

        int spawnedTrees = 0;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        Vector2 boundarySize = Vector2.zero;

        if (boxCollider.enabled)
        {
            boundarySize = boxCollider.size;
        }
        else if (circleCollider.enabled)
        {
            boundarySize = new Vector2(circleCollider.radius * 2, circleCollider.radius * 2);
        }

        Debug.Log($"Square Size: {boundarySize}");

        float stepX = boundarySize.x / Mathf.Sqrt(wholeNumber);
        float stepY = boundarySize.y / Mathf.Sqrt(wholeNumber);

        Debug.Log($"StepX: {stepX}, StepY: {stepY}");

        Vector2 startPos = (Vector2)transform.position - boundarySize / 2f;

        Debug.Log($"Start Pos: {startPos}");

        for (float x = startPos.x; x <= startPos.x + boundarySize.x; x += stepX)
        {
            for (float y = startPos.y; y <= startPos.y + boundarySize.y; y += stepY)
            {
                if (spawnedTrees >= wholeNumber)
                {
                    break;
                }

                float noiseFactorX = 1 + (Mathf.PerlinNoise(x, y) - 0.5f) * PerlinNoiseMultiplier;
                float noiseFactorY = 1 + (Mathf.PerlinNoise(y, x) - 0.5f) * PerlinNoiseMultiplier;

                Debug.Log($"X: {x}, Y: {y}");
                Debug.Log($"NoiseFactorX: {noiseFactorX}, NoiseFactorY: {noiseFactorY}");

                float noiseX = x * noiseFactorX;
                float noiseY = y * noiseFactorY;

                Debug.Log($"NoiseX: {noiseX}, NoiseY: {noiseY}");

                Sprite selectedSprite = GetRandomTreeSprite();

                GameObject tree = new GameObject();
                tree.name = $"Tree_{spawnedTrees}";
                tree.transform.parent = transform;
                tree.transform.position = new Vector3(noiseX, noiseY, 0);
                tree.transform.rotation = Quaternion.identity;

                SpriteRenderer treeSprite = tree.AddComponent<SpriteRenderer>();
                treeSprite.sprite = selectedSprite;
                treeSprite.sortingLayerName = "Front Design";

                spawnedTrees++;
            }

            if (spawnedTrees >= wholeNumber)
            {
                break;
            }
        }

        if (UnityEngine.Random.Range(0f, 1f) < (TreeDensity - wholeNumber))
        {
            float noiseFactorX = 1 + (Mathf.PerlinNoise(startPos.x + boundarySize.x / 2, startPos.y + boundarySize.y / 2) - 0.5f) * PerlinNoiseMultiplier;
            float noiseFactorY = 1 + (Mathf.PerlinNoise(startPos.y + boundarySize.y / 2, startPos.x + boundarySize.x / 2) - 0.5f) * PerlinNoiseMultiplier;

            float noisyX = (startPos.x + boundarySize.x / 2) * noiseFactorX;
            float noisyY = (startPos.y + boundarySize.y / 2) * noiseFactorY;

            Sprite selectedSprite = GetRandomTreeSprite();

            GameObject tree = new GameObject();
            tree.name = $"Tree_{spawnedTrees}";
            tree.transform.parent = transform;
            tree.transform.position = new Vector3(noisyX, noisyY, 0);
            tree.transform.rotation = Quaternion.identity;

            SpriteRenderer treeSprite = tree.AddComponent<SpriteRenderer>();
            treeSprite.sprite = selectedSprite;
            treeSprite.sortingLayerName = "Front Design";
        }
    }

    private Sprite GetRandomTreeSprite()
    {
        int randomIndex = UnityEngine.Random.Range(0, TreeGroup.TreeSprites.Count);
        return TreeGroup.TreeSprites[randomIndex];
    }
}