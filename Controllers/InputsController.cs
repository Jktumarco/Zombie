using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

 public enum mouseTypeEvent { OnClickDown, OnClickUp, OnClickMove, OnDubleClick, OnDefault, OnHold };
public class InputsController : MonoBehaviour
{
    static public InputsController instance;
    mouseTypeEvent curMouseEvent;
    public bool IsMouseSelection = false;
    public bool isOverUi = false;
    [SerializeField] float _timeDelay;
    [SerializeField] float _delay = .6f;
    public mouseTypeEvent currentMouseEvent => curMouseEvent;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        curMouseEvent = new mouseTypeEvent();
        curMouseEvent = mouseTypeEvent.OnDefault;
    }

    public bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }

    public bool IsMouseOverUIWithIgnores()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            if(raycastResultsList[i].gameObject.GetComponent<OnUiMous>() != null)
            {
                raycastResultsList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultsList.Count > 0;
    }
    void Update()
    {
       
        if (Input.GetMouseButtonUp(0)) { IsMouseSelection = false; curMouseEvent = mouseTypeEvent.OnClickUp; curMouseEvent = mouseTypeEvent.OnDefault; }
        TimeByClickOrHold();
        if (Input.GetMouseButtonDown(0))
        {
            if (_timeDelay < _delay)
            { curMouseEvent = mouseTypeEvent.OnDubleClick; Debug.Log("Double click"); }
            else curMouseEvent = mouseTypeEvent.OnClickDown; _timeDelay = 0f;
        }

        if (Input.GetMouseButton(0) && _timeDelay > _delay) { curMouseEvent = mouseTypeEvent.OnHold; IsMouseSelection = true;  Debug.Log("OnHold"); } 
    }
   

    


    mouseTypeEvent EventMous()
    {
        switch (curMouseEvent)
        {
            case  mouseTypeEvent.OnClickDown:
                return mouseTypeEvent.OnClickDown;
                break;

        }
        return mouseTypeEvent.OnDefault;
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLick");
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Up");
       
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Debug.Log("Move");
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
        
    }
    void TimeByClickOrHold()
    {
     
        {
            _timeDelay += Time.deltaTime;
        }
    }
}
