using UnityEngine;
using UnityEngine.EventSystems;

public class BuildSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private TileAnimator tileAnim;
    private BuildManager buildManager;
    private Vector3 defaultPosition;

    private bool tileCanBeMoved = true;
    private bool buildSlotAvailible = true;

    private Coroutine currenMovementUpCo;
    private Coroutine moveToDefaultCo;

    private void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        tileAnim = FindFirstObjectByType<TileAnimator>();
        buildManager = FindFirstObjectByType<BuildManager>();
        defaultPosition = transform.position;
    }

    private void Start()
    {
        if (buildSlotAvailible == false)
            transform.position += new Vector3(0, .1f);

    }

    public void SetSlotAvailibleTo(bool value) => buildSlotAvailible = value;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buildSlotAvailible == false)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (buildManager.GetSelectedSlot() == this)
            return;

        buildManager.EnebleBuildMenu();
        buildManager.SelectBuildSlot(this);
        MoveTileUp();

        tileCanBeMoved = false;

        ui.buildButtonsUI.GetLastSelected()?.SelectButton(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buildSlotAvailible == false)
            return;

        if (tileCanBeMoved == false)
            return;

        MoveTileUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buildSlotAvailible == false)
            return;

        if (tileCanBeMoved == false)
            return;

        if (currenMovementUpCo != null)
        {
            Invoke(nameof(MoveToDelaultPosition), tileAnim.GetTravelDuration());
        }
        else
            MoveToDelaultPosition();
    }

    public void UnselectTile()
    {
        MoveToDelaultPosition();
        tileCanBeMoved = true;
    }

    private void MoveTileUp()
    {
        Vector3 targetPosition = transform.position + new Vector3(0, tileAnim.GetBuildOffset(), 0);

        currenMovementUpCo = StartCoroutine(tileAnim.MoveTileCo(transform, targetPosition));
    }

    private void MoveToDelaultPosition()
    {
        moveToDefaultCo = StartCoroutine(tileAnim.MoveTileCo(transform, defaultPosition));
    }
    public void SnapToDefaultPositionImmidiatly()
    {
        if (moveToDefaultCo != null)
            StopCoroutine(moveToDefaultCo);

        transform.position = defaultPosition;
    }

    public Vector3 GetBuildPosition(float yOffset) => defaultPosition + new Vector3(0, yOffset);
}