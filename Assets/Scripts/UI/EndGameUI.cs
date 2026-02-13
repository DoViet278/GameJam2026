using System.Collections;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private bool pauseOnShow = true;

    [Header("Popups")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject losePopup;

    private Coroutine running;
    private float cachedTimeScale = 1f;
    private bool timeScaleOverridden;

    private void Awake()
    {
        EnsureFadeGroup();
        SetPopupActive(false, false);
        SetFade(0f, false);
    }

    public void ShowWin()
    {
        Show(true);
    }

    public void ShowLose()
    {
        Show(false);
    }

    private void Update()
    {
        if (GameController.instance.isGameOver)
        {
            GameController.instance.isGameOver = false;
            ShowLose();
            Time.timeScale = 0f;
        }else if (GameController.instance.isWin)
        {
            GameController.instance.isWin = false;
            GameController.instance.hasCoin = false;
            ShowWin();
            Time.timeScale = 0f;
        }
    }
    public void Show(bool isWin)
    {
        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(ShowRoutine(isWin));
    }

    public void HideAll()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }

        SetPopupActive(false, false);
        SetFade(0f, false);
        SetPaused(false);
    }

    private IEnumerator ShowRoutine(bool isWin)
    {
        SetPopupActive(false, false);
        SetPaused(true);

        EnsureFadeGroup();
        if (fadeGroup != null)
        {
            if (fadePanel != null && !fadePanel.activeSelf)
                fadePanel.SetActive(true);

            fadeGroup.blocksRaycasts = true;
            fadeGroup.interactable = true;

            float startAlpha = fadeGroup.alpha;
            float duration = Mathf.Max(0f, fadeDuration);
            if (duration <= 0f)
            {
                fadeGroup.alpha = 1f;
            }
            else
            {
                float t = 0f;
                while (t < duration)
                {
                    t += Time.unscaledDeltaTime;
                    fadeGroup.alpha = Mathf.Lerp(startAlpha, 1f, t / duration);
                    yield return null;
                }

                fadeGroup.alpha = 1f;
            }
        }

        if (isWin)
        {
            if (winPopup != null) winPopup.SetActive(true);
        }
        else
        {
            if (losePopup != null) losePopup.SetActive(true);
        }

        running = null;
    }

    private void SetFade(float alpha, bool blockRaycasts)
    {
        EnsureFadeGroup();
        if (fadeGroup == null)
            return;

        fadeGroup.alpha = alpha;
        fadeGroup.blocksRaycasts = blockRaycasts;
        fadeGroup.interactable = blockRaycasts;

        if (fadePanel != null)
            fadePanel.SetActive(blockRaycasts || alpha > 0.0001f);
    }

    private void SetPopupActive(bool winActive, bool loseActive)
    {
        if (winPopup != null) winPopup.SetActive(winActive);
        if (losePopup != null) losePopup.SetActive(loseActive);
    }

    private void SetPaused(bool paused)
    {
        if (!pauseOnShow)
            return;

        if (paused)
        {
            if (!timeScaleOverridden)
            {
                cachedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                timeScaleOverridden = true;
            }
        }
        else if (timeScaleOverridden)
        {
            Time.timeScale = cachedTimeScale;
            timeScaleOverridden = false;
        }
    }

    private void OnDisable()
    {
        SetPaused(false);
    }

    private void OnDestroy()
    {
        SetPaused(false);
    }

    private void EnsureFadeGroup()
    {
        if (fadeGroup != null)
            return;

        if (fadePanel == null)
            return;

        fadeGroup = fadePanel.GetComponent<CanvasGroup>();
        if (fadeGroup == null)
            fadeGroup = fadePanel.AddComponent<CanvasGroup>();
    }
}
