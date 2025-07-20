using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private UI_InGame inGameUI;
    private WaveManager currentActiveWaveManager;
    private LevelManager levelManager;
    private CameraEffects cameraEffects;

    [SerializeField] private int currency;

    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    public int EnemiesKilled { get; private set; }

    private bool gameLost;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        inGameUI = FindFirstObjectByType<UI_InGame>(FindObjectsInactive.Include);
        levelManager = FindFirstObjectByType<LevelManager>();
        cameraEffects = FindFirstObjectByType<CameraEffects>();

    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        currentHp = maxHp;

        if (IsTestingLevel())
        {
            currency += 9999;
            currentHp += 9999;
        }


        inGameUI.UpdateHealthPointsUI(currentHp, maxHp);
        inGameUI.UpdateCurrencyUI(currency);

    }

    public bool IsTestingLevel() => levelManager == null;

    public IEnumerator LevelFailed()
    {
        gameLost = true;
        currentActiveWaveManager.DectivateWaveManager();
        cameraEffects.FocusOnCastle();

        yield return cameraEffects.GetActiveCamCo();

        inGameUI.EnableGameOverUI(true);
    }

    public void LevelCompleted() => StartCoroutine(LevelCompletedCo());

    private IEnumerator LevelCompletedCo()
    {
        cameraEffects.FocusOnCastle();

        yield return cameraEffects.GetActiveCamCo();

        if (levelManager.HasNoMoreLevels())
            inGameUI.EnableVictoryUI(true);
        else
        {
            inGameUI.LevelCompletedUI(true);
            PlayerPrefs.SetInt(levelManager.GetNextLevelName() + "unlocked", 1);
        }
    }

    public void UpdateGameManager(int levelCurrency, WaveManager newWaveManager)
    {
        gameLost = false;
        EnemiesKilled = 0;

        currentActiveWaveManager = newWaveManager;
        currency = levelCurrency;
        currentHp = maxHp;

        inGameUI.UpdateCurrencyUI(levelCurrency);
        inGameUI.UpdateHealthPointsUI(currentHp, maxHp);
    }

    public void UpdateHp(int value)
    {
        currentHp += value;
        inGameUI.UpdateHealthPointsUI(currentHp, maxHp);
        inGameUI.ShakeHealthUI();

        if (currentHp <= 0 && gameLost == false)
            StartCoroutine(LevelFailed());
    }

    public void UpdateCurrency(int value)
    {
        EnemiesKilled++;
        currency += value;
        inGameUI.UpdateCurrencyUI(currency);
    }

    public bool HasEnoughtCurrency(int price)
    {
        if (price <= currency)
        {
            currency -= price;
            inGameUI.UpdateCurrencyUI(currency);
            return true;
        }

        return false;
    }
}
