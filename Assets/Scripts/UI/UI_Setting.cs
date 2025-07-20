using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    private CameraController cameraController;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;
    private const string sfxParameter = "sfxVolume";

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmText;
    private const string bgmParameter = "bgmVolume";

    [Header("Keyboard Sensetivity")]
    [SerializeField] private Slider keyboardSenseSlider;
    [SerializeField] private TextMeshProUGUI keyboardSensText;
    private const string keyboardSenseParameter = "keyboardSens";

    [SerializeField] private float minKeyboardSensetivity = 60;
    [SerializeField] private float maxKeyboardSensetivity = 240;

    [Header("Mouse Sensetivity")]
    [SerializeField] private Slider mouseSenseSlider;
    [SerializeField] private TextMeshProUGUI MouseSensText;
    private const string mouseSenseParameter = "mouseSens";

    [SerializeField] private float minMouseSensetivity = 1;
    [SerializeField] private float maxMouseSensetivity = 10;



    private void Awake()
    {
        cameraController = FindFirstObjectByType<CameraController>();
    }

    public void SFXSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParameter, newValue);
        sfxText.text = Mathf.RoundToInt(value * 100) + "%";

    }

    public void BGMSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
        bgmText.text = Mathf.RoundToInt(value * 100) + "%";

    }

    public void KeyboardSensetivity()
    {
        float value = keyboardSenseSlider.value;
        float newSensetivity = Mathf.Lerp(minKeyboardSensetivity, maxKeyboardSensetivity, value);
        cameraController.AdjustKeyboardSensetivity(newSensetivity);
        keyboardSensText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void MouseSensetivity()
    {
        float value = mouseSenseSlider.value;
        float newSensetivity = Mathf.Lerp(minMouseSensetivity, maxMouseSensetivity, value);
        cameraController.AdjustMouseSensetivity(newSensetivity);
        MouseSensText.text = Mathf.RoundToInt(value * 100) + "%";

    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(keyboardSenseParameter, keyboardSenseSlider.value);
        PlayerPrefs.SetFloat(mouseSenseParameter, mouseSenseSlider.value);
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }

    private void OnEnable()
    {
        keyboardSenseSlider.value = PlayerPrefs.GetFloat(keyboardSenseParameter, .5f);
        mouseSenseSlider.value = PlayerPrefs.GetFloat(mouseSenseParameter, .5f);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .5f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .5f);
    }
}
