using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private UI ui;
    public BuildSlot selectedBuildSlot;

    public WaveManager waveManager;
    public GridBuilder currentGrid;

    private void Awake()
    {
        ui = FindFirstObjectByType<UI>();

        MakeBuildSlotNotAvalibleIfNeeded(waveManager, currentGrid);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            CancelBuildAction();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                bool clickNotOnBuildSlot = hit.collider.GetComponent<BuildSlot>() == null;

                if (clickNotOnBuildSlot)
                    CancelBuildAction();
            }
        }
    }

    public void MakeBuildSlotNotAvalibleIfNeeded(WaveManager waveManager, GridBuilder currentGrid)
    {
        foreach (var wave in waveManager.GetLevelWaves())
        {
            if (wave.nextGird == null)
                continue;


            List<GameObject> grid = currentGrid.GetTileSetUp();
            List<GameObject> nextWaveGrid = wave.nextGird.GetTileSetUp();


            for (int i = 0; i < grid.Count; i++)
            {
                TileSlot currentTile = grid[i].GetComponent<TileSlot>();
                TileSlot netxTile = nextWaveGrid[i].GetComponent<TileSlot>();

                bool tileNotTheSame = currentTile.GetMesh() != netxTile.GetMesh() ||
                                currentTile.GetMaterial() != netxTile.GetMaterial() ||
                                currentTile.GetAllChildren().Count != netxTile.GetAllChildren().Count;

                if (tileNotTheSame == false)
                    continue;

                BuildSlot buildSlot = grid[i].GetComponent<BuildSlot>();

                if (buildSlot != null)
                    buildSlot.SetSlotAvailibleTo(false);

            }

        }
    }

    public void CancelBuildAction()
    {
        if (selectedBuildSlot == null)
            return;

        selectedBuildSlot.UnselectTile();
        selectedBuildSlot = null;
        DisableMenu();

    }

    public void SelectBuildSlot(BuildSlot newSlot)
    {
        if (selectedBuildSlot != null)
            selectedBuildSlot.UnselectTile();

        selectedBuildSlot = newSlot;
    }

    public void EnebleBuildMenu()
    {
        if (selectedBuildSlot != null)
            return;

        ui.buildButtonsUI.ShowBuildButtons(true);
    }

    private void DisableMenu()
    {
        ui.buildButtonsUI.ShowBuildButtons(false);
    }

    public BuildSlot GetSelectedSlot() => selectedBuildSlot;
}
