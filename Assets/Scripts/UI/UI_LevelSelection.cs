using UnityEngine;

public class UI_LevelSelection : MonoBehaviour
{
    private void MakeButtonsClickable(bool canClick)
    {
        LevelButtonTile[] levelButtons = FindObjectsByType<LevelButtonTile>(FindObjectsSortMode.None);

        foreach (var btn in levelButtons)
        {
            btn.CheckIfLevelUnlocked();
            btn.EnableClickOnButton(canClick);
        }
    }

    private void OnEnable()
    {
        MakeButtonsClickable(true);
    }

    private void OnDisable()
    {
        MakeButtonsClickable(false);
    }
}
