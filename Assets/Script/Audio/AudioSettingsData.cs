using UnityEngine;

[System.Serializable]
public class AudioSettingsData
{
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
}
