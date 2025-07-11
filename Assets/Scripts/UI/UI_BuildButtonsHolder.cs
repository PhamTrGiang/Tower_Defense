using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class UI_BuildButtonsHolder : MonoBehaviour
{
    [SerializeField] private float yPositionOffset;
    [SerializeField] private float openAnimationDuration = .1f;

    private bool isBuildMenuActive;
    private UI_Animator uiAnimator;

    private UI_BuildButtonOnHoverEffect[] buildButtonEffect;
    private UI_BuildButton[] buildButtons;

    private List<UI_BuildButton> unlockButtons;
    private UI_BuildButton lastSelectedButton;

    private void Awake()
    {
        uiAnimator = GetComponentInParent<UI_Animator>();
        buildButtonEffect = GetComponentsInChildren<UI_BuildButtonOnHoverEffect>();
        buildButtons = GetComponentsInChildren<UI_BuildButton>();
    }

    private void Update()
    {
        CheckBuildButtonHotkeys();

        
    }

    private void CheckBuildButtonHotkeys()
    {
        if (isBuildMenuActive == false)
            return;

        for (int i = 0; i < unlockButtons.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectNewButton(i);
                    break;
                }
            }

        if (Input.GetKeyDown(KeyCode.Space)&& lastSelectedButton != null)
            lastSelectedButton.BuildTower();
    }

    public void SelectNewButton(int buttonIndex)
    {
        if (buttonIndex >= unlockButtons.Count)
            return;

        foreach (var button in unlockButtons)
        {
            button.SelectButton(false);
        }

        UI_BuildButton selectedButton = unlockButtons[buttonIndex];

        selectedButton.SelectButton(true);
    }

    public UI_BuildButton[] GetBuildButtons() => buildButtons;
    public List<UI_BuildButton> GetUnlockedButtons() => unlockButtons;

    public void SetLastSelected(UI_BuildButton newLastSelected) => lastSelectedButton = newLastSelected;
    public UI_BuildButton GetLastSelected() => lastSelectedButton;

    public void UpdateUnlockedButtons()
    {
        unlockButtons = new List<UI_BuildButton>();

        foreach (var button in buildButtons)
        {
            if (button.buttonUnlocked)
                unlockButtons.Add(button);
        }
    }

    public void ShowBuildButtons(bool showButtons)
    {
        isBuildMenuActive = showButtons;

        float yOffset = isBuildMenuActive ? yPositionOffset : -yPositionOffset;
        float methodDelay = isBuildMenuActive ? openAnimationDuration : 0;

        uiAnimator.ChangePosition(transform, new Vector3(0, yOffset), openAnimationDuration);
        Invoke(nameof(ToggleButtonMovement), methodDelay);
    }

    private void ToggleButtonMovement()
    {
        foreach (var button in buildButtonEffect)
        {
            button.ToggleMovement(isBuildMenuActive);
        }
    }
}
