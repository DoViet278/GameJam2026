using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSfx : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clickClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip == null || AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySfx(clickClip);
    }
}
