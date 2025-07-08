using UnityEditor;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;
    private UI_Setting uiSetting;
    private UI_MainMenu uiMainMenu;
    private UI_InGame uiInGame;

    private void Awake()
    {
        uiSetting = GetComponentInChildren<UI_Setting>(true);
        uiMainMenu = GetComponentInChildren<UI_MainMenu>(true);
        uiInGame = GetComponentInChildren<UI_InGame>(true);

        SwitchTo(uiSetting.gameObject);
        // SwitchTo(uiMainMenu.gameObject);
        SwitchTo(uiInGame.gameObject);
    }

    public void SwitchTo(GameObject uiEneble)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }

        uiEneble.SetActive(true);
    }

    public void QuitButton()
    {
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
