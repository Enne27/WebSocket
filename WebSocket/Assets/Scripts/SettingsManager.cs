using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle muteToggle;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    void Start()
    {
        // Configurar los sliders y toggle al inicio
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Inicializar los valores
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        // Cargar configuración de PlayerPrefs
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        }
        else
        {
            musicSlider.value = 0.5f;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        }
        else
        {
            sfxSlider.value = 0.5f;
        }

        if (PlayerPrefs.HasKey("Muted"))
        {
            muteToggle.isOn = PlayerPrefs.GetInt("Muted", 0) == 1;
        }
        else
        {
            muteToggle.isOn = false;
        }

        if (PlayerPrefs.HasKey("Quality"))
        {

        }
        else
        {
            qualityDropdown.value = 2;
            QualitySettings.SetQualityLevel(2);
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullScreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }
        else
        {
            fullScreenToggle.isOn = Screen.fullScreen;
        }

        // Cargar resolución seleccionada previamente
        if (PlayerPrefs.HasKey("Resolution"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }
        else
        {
            resolutionDropdown.value = GetCurrentResolutionIndex();
        }
    }

    public void SetMusicVolume(float value)
    {
        if (value <= 0.01f)
        {
            audioMixer.SetFloat("Music", -80f); // Mínimo volumen (silencio)
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value <= 0.01f)
        {
            audioMixer.SetFloat("SFX", -80f); // Mínimo volumen (silencio)
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void ToggleMute(bool isMuted)
    {
        if (isMuted)
        {
            // Mutear estableciendo el volumen a -80 dB (mínimo)
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            // Reactivar estableciendo el volumen a 0 dB (máximo)
            audioMixer.SetFloat("Master", 0f);
        }
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }

    public void SetQuality(int index)
    {
        // Configurar el nivel de calidad gráfica basado en el índice seleccionado
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Configurar pantalla completa
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int index)
    {
        // Obtiene el texto de la opción seleccionada
        string resolutionOption = resolutionDropdown.options[index].text;

        // Extrae las dimensiones (ancho x alto) de la opción seleccionada
        string[] resolutionParts = resolutionOption.Split('x');
        int width = int.Parse(resolutionParts[0].Trim());
        int height = int.Parse(resolutionParts[1].Trim());

        // Aplica la resolución seleccionada
        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", index);
    }

    private int GetCurrentResolutionIndex()
    {
        int width = Screen.currentResolution.width;
        int height = Screen.currentResolution.height;

        // Busca el índice de la resolución actual
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            string[] resolutionParts = resolutionDropdown.options[i].text.Split('x');
            int optionWidth = int.Parse(resolutionParts[0].Trim());
            int optionHeight = int.Parse(resolutionParts[1].Trim());

            if (optionWidth == width && optionHeight == height)
            {
                return i;
            }
        }

        return 0; // Si no se encuentra, devuelve el primer índice por defecto
    }
}
