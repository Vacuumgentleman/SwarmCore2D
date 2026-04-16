using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using SwarmCore2D.Core;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject pausePanel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    SwarmCore2D.Simulation.SwarmSimulationController simulation;

    bool isPaused = false;

    const string MusicVolumeKey = "MusicVolume";
    const string SfxVolumeKey = "SfxVolume";

    void Start()
    {
        simulation = FindFirstObjectByType<SwarmCore2D.Simulation.SwarmSimulationController>();

        if (pausePanel != null)
            pausePanel.SetActive(false);

        float savedMusic = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfx;
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        ApplyVolumes(savedMusic, savedSfx);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        SwarmTime.Paused = true;

        if (simulation != null)
            simulation.SetPaused(true);

        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        SwarmTime.Paused = false;

        if (simulation != null)
            simulation.SetPaused(false);

        if (GameTimer.Instance != null)
            GameTimer.Instance.StartTimer();

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SwarmTime.Paused = false;
        SwarmTime.Reset();

        if (simulation != null)
            simulation.SetPaused(false);

        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(Mathf.Max(0, current - 1));
    }

    void OnMusicVolumeChanged(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    void OnSfxVolumeChanged(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;

        PlayerPrefs.SetFloat(SfxVolumeKey, value);
    }

    void ApplyVolumes(float music, float sfx)
    {
        if (musicSource != null)
            musicSource.volume = music;

        if (sfxSource != null)
            sfxSource.volume = sfx;
    }
}