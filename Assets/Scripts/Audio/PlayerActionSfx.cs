using UnityEngine;

public class PlayerActionSfx : MonoBehaviour
{
    [System.Serializable]
    public class ActionSfx
    {
        public string actionId;
        public AudioClip clip;
    }

    public AudioClip defaultClip;
    public ActionSfx[] actionClips;
    public bool useDebugKey = true;
    public KeyCode debugKey = KeyCode.Space;
    public string debugActionId = "Jump";

    private readonly System.Collections.Generic.Dictionary<string, AudioClip> map =
        new System.Collections.Generic.Dictionary<string, AudioClip>();

    private void Awake()
    {
        map.Clear();
        if (actionClips == null)
            return;

        for (int i = 0; i < actionClips.Length; i++)
        {
            ActionSfx entry = actionClips[i];
            if (entry == null || string.IsNullOrEmpty(entry.actionId) || entry.clip == null)
                continue;

            if (!map.ContainsKey(entry.actionId))
                map.Add(entry.actionId, entry.clip);
        }
    }

    private void Update()
    {
        if (!useDebugKey)
            return;

    }

    public void PlayAction()
    {
        PlayClip(defaultClip);
    }

    public void PlayAction(string actionId)
    {
        if (string.IsNullOrEmpty(actionId))
        {
            PlayClip(defaultClip);
            return;
        }

        if (map.TryGetValue(actionId, out AudioClip clip))
            PlayClip(clip);
        else
            PlayClip(defaultClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySfx(clip);
    }
}
