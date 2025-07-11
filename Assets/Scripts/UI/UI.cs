using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image fadeImageUI;
    [SerializeField] private GameObject[] uiElements;

    private UI_Setting uiSetting;
    private UI_MainMenu uiMainMenu;

    public UI_InGame uiInGame { get; private set; }
    public UI_Animator uiAnimator { get; private set; }

    public UI_BuildButtonsHolder buildButtonsUI { get; private set; }

    private void Awake()
    {
        buildButtonsUI = GetComponentInChildren<UI_BuildButtonsHolder>(true);

        uiSetting = GetComponentInChildren<UI_Setting>(true);
        uiMainMenu = GetComponentInChildren<UI_MainMenu>(true);
        uiInGame = GetComponentInChildren<UI_InGame>(true);
        uiAnimator = GetComponent<UI_Animator>();

        //ActivateFadeEffect(true);

        SwitchTo(uiSetting.gameObject);
        //SwitchTo(uiMainMenu.gameObject);
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

    public void ActivateFadeEffect(bool faceIn)
    {
        if (faceIn)
            uiAnimator.ChangeColor(fadeImageUI, 0, 1);
        else
            uiAnimator.ChangeColor(fadeImageUI, 1, 1);

    }
}
