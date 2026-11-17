using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Slider sensitivitySlider;
    public AudioSource musicSource;
    public float pausedMusicVolume = 0.5f;

    public static bool GameIsPaused = false;
    public static float mouseSens = 775f;

    public TMP_Text crtButtonText;

    private float originalMusicVolume;
    AudioSource[] allAudio;

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        originalMusicVolume = musicSource.volume;
        sensitivitySlider.value = mouseSens;

        if (crtButtonText != null)
        {
            crtButtonText.text = "CRT Mode: ON";
            crtButtonText.color = new Color32(0, 255, 14, 255);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        musicSource.volume = pausedMusicVolume;

        allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudio)
        {
            if (audio != musicSource)
                audio.Pause();
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        musicSource.volume = originalMusicVolume;

        foreach (AudioSource audio in allAudio)
        {
            if (audio != musicSource)
                audio.UnPause();
        }

        Input.ResetInputAxes();
    }

    public void OnSensitivityChanged(float value)
    {
        float snappedValue = Mathf.Clamp(Mathf.Round(value / 50f) * 50f, 50f, 1500f);
        mouseSens = snappedValue;
    }

    public void OnSensitivityChangeEnd()
    {
        mouseSens = Mathf.Clamp(Mathf.Round(mouseSens / 50f) * 50f, 50f, 1500f);
        sensitivitySlider.value = mouseSens;
    }

    public void ToggleCRT()
    {
        CRTRenderFeature.IsEnabled = !CRTRenderFeature.IsEnabled;
        BitCrusher.IsEnabled = CRTRenderFeature.IsEnabled;

        if (crtButtonText != null)
        {
            if (CRTRenderFeature.IsEnabled)
            {
                crtButtonText.text = "CRT Mode: ON";
                crtButtonText.color = new Color32(0, 255, 14, 255);
            }
            else
            {
                crtButtonText.text = "CRT Mode: OFF";
                crtButtonText.color = Color.white;
            }
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        SceneManager.LoadScene("Main Menu");
    }
}