using UnityEngine;
using UnityEngine.EventSystems;
public class OnDropBack : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = eventData.pointerDrag.GetComponent<OnDragInventary>().beginVector;
            UI_Controller.instance.PanelBlockRaycastTarget(false);
        }
    }
}
