using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    private UI ui;
    private UI_InGame uiInGame;

    [SerializeField] private GameObject[] pauseUiElements;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        uiInGame = ui.GetComponentInChildren<UI_InGame>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ui.SwitchTo(uiInGame.gameObject);
    }

    public void SwitchPauseUIElments(GameObject elementToEnable)
    {
        foreach (GameObject obj in pauseUiElements)
        {
            obj.SetActive(false);
        }

        elementToEnable.SetActive(true);
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
