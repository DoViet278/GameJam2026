using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchTimerUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float searchTime = 3f;

    private Coroutine searchRoutine;

    void Awake()
    {
        fillImage.fillAmount = 0;
        gameObject.SetActive(false);
    }

    public void StartSearch(System.Action onComplete)
    {
        gameObject.SetActive(true);

        if (searchRoutine != null)
            StopCoroutine(searchRoutine);

        searchRoutine = StartCoroutine(SearchCoroutine(onComplete));
    }

    public void CancelSearch()
    {
        if (searchRoutine != null)
            StopCoroutine(searchRoutine);

        fillImage.fillAmount = 0;
        gameObject.SetActive(false);
    }

    private IEnumerator SearchCoroutine(System.Action onComplete)
    {
        float timer = 0f;
        fillImage.fillAmount = 1f;

        while (timer < searchTime)
        {
            timer += Time.deltaTime;
            fillImage.fillAmount = 1 - (timer / searchTime);
            yield return null;
        }

        fillImage.fillAmount = 0;
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
