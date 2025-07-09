using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BuildButtonOnHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float adjustmentSpeed = 10;
    [SerializeField] private float showcaseY;
    [SerializeField] private float defaultY;

    private float targetY;
    private bool canMove;

    private void Update()
    {
        if (Mathf.Abs(transform.position.y - targetY) > .01f && canMove)
        {
            float newPositionY = Mathf.Lerp(transform.position.y, targetY, adjustmentSpeed * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
        }
    }

    public void ToggleMovement(bool buttonsMenuActive)
    {
        canMove = buttonsMenuActive;
        SetTargetY(defaultY);

        if (buttonsMenuActive == false)
            SetPositionToDefault();

    }

    private void SetPositionToDefault()
    {
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
    }

    private void SetTargetY(float newY) => targetY = newY;

    public void OnPointerEnter(PointerEventData eventData) => SetTargetY(showcaseY);
    public void OnPointerExit(PointerEventData eventData) => SetTargetY(defaultY);



}
