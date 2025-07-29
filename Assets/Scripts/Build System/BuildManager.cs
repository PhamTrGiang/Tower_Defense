using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private UI ui;
    private GameManager gameManager;
    private CameraEffects cameraEffects;

    public BuildSlot selectedBuildSlot;
    public WaveManager waveManager;
    public GridBuilder currentGrid;

    [SerializeField] private LayerMask whatToIgnore;

    [Header("Build Materials")]
    [SerializeField] private Material attackRadiusMat;
    [SerializeField] private Material buildPreviewMat;

    [Header("Build Details")]
    [SerializeField] private float towerCenterY = .5f;
    [SerializeField] private float camShakeDuration = .15f;
    [SerializeField] private float camShakeMagnitude = .02f;

    private bool isMouseOverUI;

    private void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        cameraEffects = FindFirstObjectByType<CameraEffects>();

        MakeBuildSlotNotAvalibleIfNeeded(waveManager, currentGrid);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void BuildTower(GameObject towerToBuild, int towerPrice,Transform newPreviewTower)
    {
        if (gameManager.HasEnoughtCurrency(towerPrice) == false)
        {
            ui.inGameUI.ShakeCurrencyUI();
            return;
        }

        if (towerToBuild == null)
        {
            Debug.LogWarning("You did not assign tower to this button!");
            return;
        }

        if (ui.buildButtonsUI.GetLastSelectedButton() == null)
            return;
        Transform previewTower = newPreviewTower;
        BuildSlot slotToUse = GetSelectedSlot();
        CancelBuildAction();

        slotToUse.SnapToDefaultPositionImmidiatly();
        slotToUse.SetSlotAvailibleTo(false);

        ui.buildButtonsUI.SetLastSelected(null, null);

        cameraEffects.ScreenShake(camShakeDuration, camShakeMagnitude);

        GameObject newTower = Instantiate(towerToBuild, slotToUse.GetBuildPosition(towerCenterY), Quaternion.identity);
        newTower.transform.rotation = previewTower.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            CancelBuildAction();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isMouseOverUI)
                return;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
            {
                bool clickNotOnBuildSlot = hit.collider.GetComponent<BuildSlot>() == null;

                if (clickNotOnBuildSlot)
                    CancelBuildAction();
            }
        }
    }

    public void MouseOverUI(bool isOverUI) => isMouseOverUI = isOverUI;

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

        ui.buildButtonsUI.GetLastSelectedButton()?.SelectButton(false);

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
    public Material GetAttackRadiusMat() => attackRadiusMat;
    public Material GetBuildPreviewMat() => buildPreviewMat;
}
