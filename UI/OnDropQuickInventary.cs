using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnDropQuickInventary : MonoBehaviour, IDropHandler
{
    InventaryPlace inventaryPlace;
    private void Start()
    {
        inventaryPlace = GetComponent<InventaryPlace>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var onDrop = eventData.pointerDrag.GetComponent<OnDropQuickInventary>();
            if (onDrop == null && !inventaryPlace.isFree) 
            {
                DropToPlaceToReturn(eventData);
            }
            if (onDrop == null && inventaryPlace.isFree)
            {
                DropToPlace(eventData);
            }
        }
    }
    void DropToPlace(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        otherItemTransform.SetParent(transform);
        otherItemTransform.localPosition = Vector3.zero;
        string name = eventData.pointerDrag.GetComponent<Image>().sprite.name;
        Text amount = eventData.pointerDrag.GetComponentInChildren<Text>();
        string curName = default;
        if (amount != null)
        {
            curName = name + ',' + amount.text;
        }
        if (amount == null) { curName = name; }
        UpdateUIEvent(curName);
    }
    void DropToPlaceToReturn(PointerEventData eventData)
    {
        ReturnBeginPlace(eventData);
        string name = eventData.pointerDrag.GetComponent<Image>().sprite.name;
        Text amount = eventData.pointerDrag.GetComponentInChildren<Text>();
        string curName = default;
        if (amount != null)
        {
            curName = name + ',' + amount.text;
        }
        if (amount == null) { curName = name; }
        UpdateUIEvent(curName);
    }
    void ReturnBeginPlace(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = eventData.pointerDrag.GetComponent<OnDragInventary>().beginVector;
    }
    void UpdateUIEvent(string name)
    {
        UI_Controller.instance.PanelBlockRaycastTarget(false);
        UI_Controller.instance.OnAddItemToQuickInventary?.Invoke(name);
        UI_quickInventary.instance.AddItems();
        UI_Controller.instance.OnUpdateStatusPlace.Invoke();
        AudioController.instance.PlaySFX("onDrop_1");
    }
}
