using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
public class UI_Inventary : UI_Base
{
    [SerializeField] InventaryPlace[] upInventaryPanels;
    [SerializeField] List<Sprite> spritesIcon = new List<Sprite>();
    [SerializeField] Inventary InventaryCharacter;
    [SerializeField] Canvas canvas;

    [SerializeField] InventaryPlace[] downInventaryPanels;
    //[SerializeField] List<GameObject> ObjSprite = new List<GameObject>();

    [SerializeField] GameObject activeInventary;
    
    private void OnEnable()
    {
        UI_Controller.instance.OnInventaryUpdate += UIUpdate;
        UI_Controller.instance.OnUpdateStatusPlace += UpdateStateOfPlaceUpInventary;
        UI_Controller.instance.OnUpdateStatusPlace += UpdateStateOfPlaceDownInventary;
        UI_Controller.instance.OnDestroyItemFromAllInventary += DestroyItemInBothInventry;
        Character.OnShoot += UIUpdate;
    }
    private void Awake()
    {
        downInventaryPanels = UI_Controller.instance.UI_QuickInventaryDownPanelRoot.gameObject.GetComponentsInChildren<InventaryPlace>();
        upInventaryPanels = UI_Controller.instance.UI_InventaryPanelRoot.GetComponentsInChildren<InventaryPlace>();
        spritesIcon = GameLoader.instance.GetAllIcons();
    } 
    public void UIUpdate()
    {
        for (int i = 0; i < InventaryCharacter.itemsMainInventary.Count; i++)
        {
            if (upInventaryPanels != null)
            {
                if (IsItemExistInPanelList(InventaryCharacter.itemsMainInventary[i]))
                { 
                    var curItem = GetPlaceByNameItem(InventaryCharacter.itemsMainInventary[i]); 
                    curItem.TextAmount.text = UtilsClass.GetByFormatTextByIndex(InventaryCharacter.itemsMainInventary[i], 1);
                }
                if (!IsItemExistInPanelList(InventaryCharacter.itemsMainInventary[i]))
                { 
                   
                    var freePlace = GetFreePanel();


                    var go = Factorys.instance.FactoryInventaryItem.GetNewInstance();
                    go.transform.SetParent(freePlace.gameObject.transform);
                    go.transform.position = freePlace.transformPlace.position;
                    freePlace.ItemImage = go.GetComponent<Image>();
                    var textAmount = go.GetComponentInChildren<Text>();
                    freePlace.ItemImage.material = GameAsset.i.m_unlitDefault;
                    var nameTexture = UtilsClass.GetByFormatTextByIndex(InventaryCharacter.itemsMainInventary[i], 0);
                    var amount = UtilsClass.GetByFormatTextByIndex(InventaryCharacter.itemsMainInventary[i], 1);
                    freePlace.ItemImage.sprite = GetIconByName(nameTexture);
                    freePlace.TextAmount = textAmount;
                    go.name = nameTexture + "," + amount;
                    freePlace.ImageName = go.name;

                    freePlace.isFree = false;
                    if (amount != null)
                    {
                        textAmount.text = amount;
                    }

                }
            }
        }
  
    }
   bool  IsItemExistInPanelList(string name)
    {
        var nameMain = UtilsClass.GetByFormatTextByIndex(name, 0);
      
        foreach (var item in upInventaryPanels)
        {
           
            var image = item.ItemImage;
            if(item.ItemImage != null) {
                
                var curImageName = UtilsClass.GetByFormatTextByIndex(item.ImageName, 0);
                if (curImageName == nameMain) { return true; } 
            }  
        }
        return false;
   }
    void DestroyItemInBothInventry(string itemName)
    {
        var goUp = GetItemInUpInventaryPanelList(itemName);
        if(goUp!= null) { var inventaryPlace = goUp.transform.GetComponentInParent<InventaryPlace>(); inventaryPlace.isFree = true; inventaryPlace.ImageName = null; Destroy(goUp.gameObject); }
        var goDown = GetItemInDownInventaryPanelList(itemName);
        if (goDown != null) { Destroy(goDown.gameObject); }
    }
    GameObject GetItemInUpInventaryPanelList(string name)
    {
        var nameMain = UtilsClass.GetByFormatTextByIndex(name, 0);

        foreach (var item in upInventaryPanels)
        {

            var image = item.ItemImage;
            if (item.ItemImage != null)
            {

                var curImageName = UtilsClass.GetByFormatTextByIndex(item.ImageName, 0);
                if (curImageName == nameMain) { return image.gameObject; }
            }
        }
        return null;
    }
    GameObject GetItemInDownInventaryPanelList(string name)
    {
        var nameMain = UtilsClass.GetByFormatTextByIndex(name, 0);

        foreach (var item in downInventaryPanels)
        {

            var image = item.ItemImage;
            if (item.ItemImage != null)
            {

                var curImageName = UtilsClass.GetByFormatTextByIndex(item.ImageName, 0);
                if (curImageName == nameMain) { return image.gameObject; }
            }
        }
        return null;
    }
    void UpdateStateOfPlaceUpInventary()
    {
        foreach (var place in upInventaryPanels)
        {
            OnDragInventary onDragInventary = place.gameObject.GetComponentInChildren<OnDragInventary>();
            if (onDragInventary == null) { place.isFree = true; }
            else place.isFree = false;
        } 
    }
    void UpdateStateOfPlaceDownInventary()
    {
        foreach (var place in downInventaryPanels)
        {
            OnDragInventary onDragInventary = place.gameObject.GetComponentInChildren<OnDragInventary>();
            if (onDragInventary != null) { Debug.Log(onDragInventary); }
            if (onDragInventary == null) { place.isFree = true; }
            else place.isFree = false;
        }
    }
    InventaryPlace GetPlaceByNameItem(string name)
    {
        var nameMain = UtilsClass.GetByFormatTextByIndex(name, 0);
        foreach (var item in upInventaryPanels)
        {

            if (item.name != null)
            {
                var curImageName = UtilsClass.GetByFormatTextByIndex(item.ImageName, 0);
                if (curImageName == nameMain) { return item; }   
            }
        }
        return null;
    }
    InventaryPlace GetFreePanel()
    {
        foreach (var itemPanele in upInventaryPanels)
        { 
            if (itemPanele != null && itemPanele.isFree) { return itemPanele; }
        }
        return null;
    }
    Sprite GetIconByName(string nameIcon)
    {
        foreach (var item in GameLoader.instance.GetAllIcons())
        {
            if(item.name == nameIcon) { return item; }
        }
        return null;
    }

    private void OnDisable()
    {
        UI_Controller.instance.OnInventaryUpdate -= UIUpdate;
        Character.OnShoot -= UIUpdate;
        UI_Controller.instance.OnUpdateStatusPlace -= UpdateStateOfPlaceUpInventary;
        UI_Controller.instance.OnUpdateStatusPlace -= UpdateStateOfPlaceDownInventary;
        UI_Controller.instance.OnDestroyItemFromAllInventary -= DestroyItemInBothInventry;
    }
    public void InventaryButtonOn( Toggle toggle) 
    {
        if (toggle.isOn) { 
            activeInventary.SetActive(true); 
            UI_Controller.instance.OnInventaryUpdate.Invoke();
            AudioController.instance.PlaySFX("buttonInventaryON");
        }
        else if (!toggle.isOn) { 
            activeInventary.SetActive(false);
            UI_Controller.instance.OnInventaryUpdate.Invoke(); 
            AudioController.instance.PlaySFX("buttonInventaryOff"); 
        }
    }
   

}
