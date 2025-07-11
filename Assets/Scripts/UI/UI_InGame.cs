using TMPro;
using UnityEngine;

public class UI_InGame : MonoBehaviour
{
    private UI ui;
    private UI_Pause uiPause;
    private UI_Animator uiAnimator;

    [SerializeField] private TextMeshProUGUI healthPointsText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [Space]
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private float waveTimerOffset;
    [SerializeField] private UI_TextBlinkEffect waveTimerTimerTextBlinkEffect;

    private void Awake()
    {
        uiAnimator = GetComponentInParent<UI_Animator>();
        ui = GetComponentInParent<UI>();
        uiPause = ui.GetComponentInChildren<UI_Pause>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ui.SwitchTo(uiPause.gameObject);
    }

    public void ShakeCurrencyUI() => ui.uiAnimator.Shake(currencyText.transform.parent);
    public void ShakeHealthUI() => ui.uiAnimator.Shake(healthPointsText.transform.parent);

    public void UpdateHealthPointsUI(int value, int maxValue)
    {
        int newValue = maxValue - value;
        healthPointsText.text = "Threat: " + newValue + "/" + maxValue;
    }
    public void UpdateCurrencyUI(int value)
    {
        currencyText.text = "Resources: " + value;
    }

    public void UpdateWaveTimerUI(float value) => waveTimerText.text = "seconds: " + value.ToString("00");
    public void EnebleWaveTimer(bool enable)
    {
        Transform waveTimerTransform = waveTimerText.transform.parent;

        float yOffset = enable ? waveTimerOffset : -waveTimerOffset;
        Vector3 offset = new Vector3(0, yOffset);

        uiAnimator.ChangePosition(waveTimerTransform, offset);
        waveTimerTimerTextBlinkEffect.EnableBlink(enable);
        //waveTimerText.transform.parent.gameObject.SetActive(enable);
    }

    public void ForceWaveButton()
    {
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        waveManager.ForceNextWave();
    }
}
