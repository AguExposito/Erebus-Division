using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshSettings();
    }

    public void RefreshSettings()
    {
        volumeSlider.value = Settings.volumeLevel;

        //self exp
        Apply();
    }

    public void Apply()
    {
        Settings.volumeLevel = volumeSlider.value;
        //aplica los valores de settings al juego
        QualitySettings.SetQualityLevel(Settings.graphicQuality);
        audioMixer.SetFloat("Master", Mathf.Log10(Settings.volumeLevel) * 20);

    }

}

public class Settings
{
    public static int graphicQuality;
    public static float volumeLevel = 0.7f;
    public static bool isLowRes = false;
}

