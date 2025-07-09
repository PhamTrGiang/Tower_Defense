using UnityEngine;

public class UI_BuildButton : MonoBehaviour
{
    private BuildManager buildManager;
    private CameraEffects cameraEffects;
    private GameManager gameManager;

    [SerializeField] private int price = 50;
    [SerializeField] private GameObject towerToBuild;
    [SerializeField] private float towerCenterY = .5f;

    private void Awake()
    {
        buildManager = FindFirstObjectByType<BuildManager>();
        cameraEffects = FindFirstObjectByType<CameraEffects>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void BuildTower()
    {
        if (gameManager.HasEnoughtCurrency(price) == false)
            return;

        if (towerToBuild == null)
            {
                Debug.LogWarning("You did not assign tower to this button!");
                return;
            }

        BuildSlot slotToUse = buildManager.GetSelectedSlot();
        buildManager.CancelBuildAction();

        slotToUse.SnapToDefaultPositionImmidiatly();
        slotToUse.SetSlotAvailibleTo(false);

        cameraEffects.ScreenShake(.15f, .02f);

        GameObject newTower = Instantiate(towerToBuild, slotToUse.GetBuildPosition(towerCenterY), Quaternion.identity);
    }
}
