using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SwarmCore2D.Core;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject pausePanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button mainMenuButton;

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

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        SetupButtonNavigation();

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

        if (isPaused && UIInputMode.Current == UIInputMode.Mode.Keyboard
            && Keyboard.current.spaceKey.wasPressedThisFrame)
            InvokeSelected();
    }

    void InvokeSelected()
    {
        var go = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;
        if (go == null) return;
        var btn = go.GetComponent<Button>();
        if (btn != null && btn.interactable)
            btn.onClick.Invoke();
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

        if (resumeButton != null && UIInputMode.Current == UIInputMode.Mode.Keyboard)
            EventSystem.current?.SetSelectedGameObject(resumeButton.gameObject);
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

        EventSystem.current?.SetSelectedGameObject(null);
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

    void SetupButtonNavigation()
    {
        if (resumeButton == null || mainMenuButton == null)
            return;

        var navResume = resumeButton.navigation;
        navResume.mode = Navigation.Mode.Explicit;
        navResume.selectOnDown = mainMenuButton;
        navResume.selectOnUp   = mainMenuButton;
        resumeButton.navigation = navResume;

        var navMenu = mainMenuButton.navigation;
        navMenu.mode = Navigation.Mode.Explicit;
        navMenu.selectOnUp   = resumeButton;
        navMenu.selectOnDown = resumeButton;
        mainMenuButton.navigation = navMenu;
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