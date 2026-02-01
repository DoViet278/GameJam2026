using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour, IAudioSettingsObserver
{
    [Header("Music UI")]
    public Button musicButton;
    public Slider musicSlider;
    public Image musicIcon;
    public Sprite musicOnIcon;
    public Sprite musicOffIcon;

    [Header("SFX UI (Optional)")]
    public Button sfxButton;
    public Slider sfxSlider;
    public Image sfxIcon;
    public Sprite sfxOnIcon;
    public Sprite sfxOffIcon;

    private bool suppressCallbacks;
    private bool listenersWired;

    private void OnEnable()
    {
        WireUiEvents();
        if (AudioManager.Instance != null)
            AudioManager.Instance.RegisterObserver(this);

        SyncFromManager();
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.UnregisterObserver(this);

        UnwireUiEvents();
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxVolume(value);
    }

    public void SetMusicEnabled(bool enabled)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicEnabled(enabled);
    }

    public void SetSfxEnabled(bool enabled)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxEnabled(enabled);
    }

    public void OnMusicButtonClicked()
    {
        if (AudioManager.Instance == null)
            return;

        AudioSettingsData data = AudioManager.Instance.GetSettings();
        SetMusicEnabled(!data.musicEnabled);
    }

    public void OnSfxButtonClicked()
    {
        if (AudioManager.Instance == null)
            return;

        AudioSettingsData data = AudioManager.Instance.GetSettings();
        SetSfxEnabled(!data.sfxEnabled);
    }

    public void OnMusicSliderChanged(float value)
    {
        if (suppressCallbacks)
            return;

        SetMusicVolume(value);
    }

    public void OnSfxSliderChanged(float value)
    {
        if (suppressCallbacks)
            return;

        SetSfxVolume(value);
    }

    public void OnAudioSettingsChanged(AudioSettingsData settings)
    {
        suppressCallbacks = true;

        if (musicSlider != null)
            musicSlider.value = settings.musicVolume;
        if (sfxSlider != null)
            sfxSlider.value = settings.sfxVolume;

        if (musicIcon != null)
            musicIcon.sprite = settings.musicEnabled ? musicOnIcon : musicOffIcon;
        if (sfxIcon != null)
            sfxIcon.sprite = settings.sfxEnabled ? sfxOnIcon : sfxOffIcon;

        suppressCallbacks = false;
    }

    private void WireUiEvents()
    {
        if (listenersWired)
            return;

        if (musicButton != null)
            musicButton.onClick.AddListener(OnMusicButtonClicked);
        if (sfxButton != null)
            sfxButton.onClick.AddListener(OnSfxButtonClicked);
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        listenersWired = true;
    }

    private void UnwireUiEvents()
    {
        if (!listenersWired)
            return;

        if (musicButton != null)
            musicButton.onClick.RemoveListener(OnMusicButtonClicked);
        if (sfxButton != null)
            sfxButton.onClick.RemoveListener(OnSfxButtonClicked);
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);

        listenersWired = false;
    }

    private void SyncFromManager()
    {
        if (AudioManager.Instance == null)
            return;

        OnAudioSettingsChanged(AudioManager.Instance.GetSettings());
    }
}
