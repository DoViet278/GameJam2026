using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Defaults")]
    public AudioSettingsSO defaultSettings;

    [Header("Music Startup")]
    public AudioClip defaultMusicClip;
    public bool playMusicOnStart = true;
    public bool musicLoop = true;

    [Header("Startup")]
    public bool forceStartVolumes = true;
    [Range(0f, 1f)]
    public float startMusicVolume = 0.5f;
    [Range(0f, 1f)]
    public float startSfxVolume = 0.5f;

    private readonly List<IAudioSettingsObserver> observers = new List<IAudioSettingsObserver>();
    private AudioSettingsData current = new AudioSettingsData();

    private const string MusicVolumeKey = "Audio.MusicVolume";
    private const string SfxVolumeKey = "Audio.SfxVolume";
    private const string MusicEnabledKey = "Audio.MusicEnabled";
    private const string SfxEnabledKey = "Audio.SfxEnabled";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        LoadSettings();
        ApplyStartupOverrides();
        ApplySettings();

        if (musicSource.isPlaying)
            musicSource.Stop();

        if (playMusicOnStart)
        {
            AudioClip clipToPlay = defaultMusicClip != null ? defaultMusicClip : musicSource.clip;
            if (clipToPlay != null)
                PlayMusic(clipToPlay, musicLoop);
        }
    }

    private void Start()
    {
        musicSource?.Stop();
    }

    public void RegisterObserver(IAudioSettingsObserver observer)
    {
        if (observer == null || observers.Contains(observer))
            return;

        observers.Add(observer);
        observer.OnAudioSettingsChanged(current);
    }

    public void UnregisterObserver(IAudioSettingsObserver observer)
    {
        if (observer == null)
            return;

        observers.Remove(observer);
    }

    public AudioSettingsData GetSettings()
    {
        return current;
    }

    public void SetMusicVolume(float value)
    {
        current.musicVolume = Mathf.Clamp01(value);
        ApplyAndNotify();
    }

    public void SetSfxVolume(float value)
    {
        current.sfxVolume = Mathf.Clamp01(value);
        ApplyAndNotify();
    }

    public void SetMusicEnabled(bool enabled)
    {
        current.musicEnabled = enabled;
        ApplyAndNotify();
    }

    public void SetSfxEnabled(bool enabled)
    {
        current.sfxEnabled = enabled;
        ApplyAndNotify();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;

        if (current.musicEnabled)
            musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || !current.sfxEnabled)
            return;

        sfxSource.PlayOneShot(clip, current.sfxVolume);
    }

    private void ApplyAndNotify()
    {
        ApplySettings();
        SaveSettings();
        NotifyObservers();
    }

    private void ApplySettings()
    {
        musicSource.volume = current.musicEnabled ? current.musicVolume : 0f;
        sfxSource.volume = current.sfxEnabled ? current.sfxVolume : 0f;

        if (!current.musicEnabled && musicSource.isPlaying)
            musicSource.Stop();
        else if (current.musicEnabled && musicSource.clip != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    private void NotifyObservers()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
                observers[i].OnAudioSettingsChanged(current);
        }
    }

    private void LoadSettings()
    {
        AudioSettingsData defaults = defaultSettings != null ? defaultSettings.defaults : null;

        current.musicVolume = PlayerPrefs.HasKey(MusicVolumeKey)
            ? PlayerPrefs.GetFloat(MusicVolumeKey)
            : (defaults != null ? defaults.musicVolume : 1f);

        current.sfxVolume = PlayerPrefs.HasKey(SfxVolumeKey)
            ? PlayerPrefs.GetFloat(SfxVolumeKey)
            : (defaults != null ? defaults.sfxVolume : 1f);

        current.musicEnabled = PlayerPrefs.HasKey(MusicEnabledKey)
            ? PlayerPrefs.GetInt(MusicEnabledKey) == 1
            : (defaults != null ? defaults.musicEnabled : true);

        current.sfxEnabled = PlayerPrefs.HasKey(SfxEnabledKey)
            ? PlayerPrefs.GetInt(SfxEnabledKey) == 1
            : (defaults != null ? defaults.sfxEnabled : true);
    }

    private void ApplyStartupOverrides()
    {
        if (!forceStartVolumes)
            return;

        current.musicVolume = Mathf.Clamp01(startMusicVolume);
        current.sfxVolume = Mathf.Clamp01(startSfxVolume);
        current.musicEnabled = true;
        current.sfxEnabled = true;
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, current.musicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, current.sfxVolume);
        PlayerPrefs.SetInt(MusicEnabledKey, current.musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SfxEnabledKey, current.sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
