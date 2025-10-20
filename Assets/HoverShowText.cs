using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverShowText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textToShow;
    public GameObject ImageToHide;
    public void OnPointerEnter(PointerEventData eventData)
    {
        textToShow.SetActive(true);
        ImageToHide.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textToShow.SetActive(false);
        ImageToHide.SetActive(true);
    }
}
