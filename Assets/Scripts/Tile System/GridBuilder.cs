using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    private NavMeshSurface MyNavMesh => GetComponent<NavMeshSurface>();
    [SerializeField] private GameObject mainPrefab;

    [SerializeField] private int gridLength = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private List<GameObject> createdTiles;

    public List<GameObject> GetTileSetUp() => createdTiles;

    public void UpdateNavMesh() => MyNavMesh.BuildNavMesh();

    private bool hadFiestLoad;

    public bool IsOnFirstLoad()
    {
        if (hadFiestLoad == false)
        {
            hadFiestLoad = true;
            return true;
        }

        return false;
    }

    [ContextMenu("Build Grid")]
    private void BuildGrid()
    {
        ClearGrid();
        createdTiles = new List<GameObject>();

        for (int x = 0; x < gridLength; x++)
        {
            for (int z = 0; z < gridWidth; z++)
            {
                Cretile(x, z);
            }
        }
    }

    [ContextMenu("Clear grid")]
    private void ClearGrid()
    {
        foreach (GameObject tile in createdTiles)
        {
            DestroyImmediate(tile);
        }
        createdTiles.Clear();
    }

    private void Cretile(float xPosition, float zPosition)
    {
        Vector3 newPosition = new Vector3(xPosition, 0, zPosition);
        GameObject newTile = Instantiate(mainPrefab, newPosition, Quaternion.identity, transform);

        createdTiles.Add(newTile);

        newTile.GetComponent<TileSlot>().TurnIntoBuildSlotIfNeeded(mainPrefab);
    }
}
