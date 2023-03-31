using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class OnDragInventary : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] public Canvas canvas;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    public Vector2 beginVector;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        beginVector = transform.position;
        canvas = GameObject.FindObjectOfType<Canvas>(); 
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        var obj = GameObject.Find("Canvas"); gameObject.transform.SetParent(obj.gameObject.transform);
        beginVector = rectTransform.anchoredPosition;
        var curItemName = eventData.pointerDrag.GetComponent<Image>().sprite.name;
        UI_Controller.instance.PanelBlockRaycastTarget(true);
        UI_Controller.instance.OnDeleteItemFromAllInventary?.Invoke(curItemName);
        UI_Controller.instance?.OnUpdateStatusPlace.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition); this.transform.position = new Vector3(cursor.x, cursor.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
    }

}
