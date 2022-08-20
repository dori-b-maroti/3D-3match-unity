/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public uint width;
    public uint height;
    public List<Tile> tileTemplates = new List<Tile>();

    private const int kMinMatchRequirement = 3;
    private const uint kTileSize = 1;
    private Camera cam;
    private AnimationCallbackManager animationCallbackManager;
    private List<Tile> tiles = new List<Tile>();

    void Start()
    {
        cam = Camera.main;
        animationCallbackManager = new AnimationCallbackManager();
        animationCallbackManager.dropCompleted += FindMatches;
        CreateTiles();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * -cam.transform.position.z);
            Tile tile = FindTileAt(worldPosition.x, worldPosition.y);
            if (tile)
            {
                PerformDropOnDestruction(tile.index);
            }
        }
    }

    private void PerformDropOnDestruction(int destroyedTileIndex)
    {
        int currentColumnFirstIndex = (destroyedTileIndex / (int)height) * (int)height;
        animationCallbackManager.destroyedTileIndex = destroyedTileIndex;
        for (int i = destroyedTileIndex; i >= currentColumnFirstIndex; i--)
        {
            Tile tile = tiles[i];
            if (i > currentColumnFirstIndex)
            {
                int topNeighbourIndex = i - 1;
                Tile topNeighbour = tiles[topNeighbourIndex];
                if (topNeighbour == null)
                {
                    HideTile(i);
                    break;
                }
                tile.CopyTile(tiles[topNeighbourIndex]);

                int startYPosition = (int)(height / 2);
                int columnPositionIndex = tile.index % (int)height;
                float toYPosition = startYPosition - columnPositionIndex;
                tile.SetupForDrop(toYPosition);
                tile.movedDown += OnTileMovedDown;
                animationCallbackManager.AddCallbackId(tile.index);
            }
            else
            {
                HideTile(i);
            }
        }
    }

    private void HideTile(int index)
    {
        tiles[index].Hide();
        tiles[index] = null;
    }

    private void OnTileMovedDown(int index)
    {
        FindMatches(index);
        animationCallbackManager.CompleteDropOnTile(index);
    }

    private void FindMatches(int index)
    {
        List<int> matchingTileIndices = new List<int>();
        matchingTileIndices.Add(index);

        FindLeftMatch(ref matchingTileIndices, index);
        FindRightMatch(ref matchingTileIndices, index);

        if (matchingTileIndices.Count < kMinMatchRequirement)
        {
            return;
        }

        for (int i = 0; i < matchingTileIndices.Count; i++)
        {
            int tileIndex = matchingTileIndices[i];
            PerformDropOnDestruction(tileIndex);
        }
    }

    private void FindLeftMatch(ref List<int> matchingTileIndices, int index)
    {
        int leftNeighbourIndex = index - (int)height;

        if (leftNeighbourIndex < 0)
        {
            return;
        }

        Tile leftNeighbour = tiles[leftNeighbourIndex];
        if (leftNeighbour == null)
        {
            return;
        }

        if (tiles[index].type == leftNeighbour.type)
        {
            matchingTileIndices.Add(leftNeighbourIndex);
            FindLeftMatch(ref matchingTileIndices, leftNeighbourIndex);
        }
    }

    private void FindRightMatch(ref List<int> matchingTileIndices, int index)
    {
        int rightNeighbourIndex = index + (int)height;

        if (rightNeighbourIndex >= (width * height))
        {
            return;
        }

        Tile rightNeighbour = tiles[rightNeighbourIndex];
        if (rightNeighbour == null)
        {
            return;
        }

        if (tiles[index].type == rightNeighbour.type)
        {
            matchingTileIndices.Add(rightNeighbourIndex);
            FindRightMatch(ref matchingTileIndices, rightNeighbourIndex);
        }
    }

    private void CreateTiles()
    {
        int startXPosition = (int)(-width / 2);
        int startYPosition = (int)(height / 2);
        int[] horizontalMatchCount = new int[height];
        int tileTemplatesAmount = tileTemplates.Count;

        for (uint i = 0; i < width; i++)
        {
            for (uint j = 0; j < height; j++)
            {
                Tile leftNeighbour = null;
                int tileIndex = (int)(i * height + j);
                int randomTileTemplateIndex = Random.Range(0, tileTemplatesAmount);
                Tile tileTemplate = tileTemplates[randomTileTemplateIndex];

                if (i != 0)
                {
                    leftNeighbour = tiles[(int)(tileIndex - height)];
                }

                if (leftNeighbour)
                {
                    if (tileTemplate.type == leftNeighbour.type)
                        horizontalMatchCount[j]++;

                    if (horizontalMatchCount[j] >= kMinMatchRequirement - 1)
                    {
                        while (tileTemplate.type == leftNeighbour.type)
                        {
                            randomTileTemplateIndex = Random.Range(0, tileTemplatesAmount);
                            tileTemplate = tileTemplates[randomTileTemplateIndex];
                        }
                        horizontalMatchCount[j] = 0;
                    }
                }

                Tile newTile = Instantiate(tileTemplate);
                newTile.index = tileIndex;
                newTile.transform.position = new Vector3(startXPosition + i, startYPosition - j);
                tiles.Add(newTile);
            }
        }
    }

    Tile FindTileAt(float inputXPosition, float inputYPosition)
    {
        const float halfTileSize = (float)kTileSize / 2;

        for (int i = 0, length = tiles.Count; i < length; ++i)
        {
            Tile tile = tiles[i];
            if (tile == null)
            {
                continue;
            }
            Vector3 tilePosition = tile.gameObject.transform.position;
            float xPosition = tilePosition.x;
            float yPosition = tilePosition.y;
            if (inputXPosition >= xPosition - halfTileSize
                && inputXPosition <= xPosition + halfTileSize
                && inputYPosition >= yPosition - halfTileSize
                && inputYPosition <= yPosition + halfTileSize)
            {
                return tile;
            }
        }
        return null;
    }
}
