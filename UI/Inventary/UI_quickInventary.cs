using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class UI_quickInventary : Item
{
    public static UI_quickInventary instance;

    [SerializeField] CharactersController charactersController;
    
    public List<string> itemsInventary;

    public static Action<string> OnChangeItem;
    [SerializeField] Image mainPaneleIcon;

    
    [SerializeField] InventaryPlace[] quickPanelItems;
    [SerializeField] InventaryPlace[] InventaryDownPanelItems;
    private void OnEnable()
    {
        UI_Controller.instance.OnInventaryUpdate += AddItems;
        UI_Controller.instance.OnAddItemToQuickInventary += Add;
    }
    void Add(string item)
    {
        itemsInventary.Add(item);
    }
    private void Awake()
    {
        instance = this;
        quickPanelItems = UI_Controller.instance.UI_QuickInventaryPanelRoot.gameObject.GetComponentsInChildren<InventaryPlace>();
        InventaryDownPanelItems = UI_Controller.instance.UI_QuickInventaryDownPanelRoot.gameObject.GetComponentsInChildren<InventaryPlace>();
    }
    public void AddToMainPanele(Image itemImage)
    {
        if (itemImage.sprite != null)
        {
            mainPaneleIcon.enabled = true;
            mainPaneleIcon.sprite = GameLoader.instance.GetIconByName(itemImage.sprite.name);
            charactersController.CurCharacter.OnChangeState(itemImage.sprite.name);
        }
    }


    public void AddItems() {
        if ( InventaryDownPanelItems != null)
        {
            for (int i = 0; i < InventaryDownPanelItems.Length; i++)
            {
                var item = InventaryDownPanelItems[i].gameObject.GetComponentInChildren<OnDragInventary>();
                var image = quickPanelItems[i].gameObject.GetComponentInChildren<Image>();
                if (item != null)
                {
                    image.enabled = true;
                    var curItem = item.gameObject.GetComponent<Image>().sprite;
                    quickPanelItems[i].gameObject.GetComponentInChildren<Image>().sprite = curItem;
                    quickPanelItems[i].gameObject.GetComponentInChildren<Item>().itemName = curItem.name;
                }
                else image.enabled = false;
            }
        }
    }
    private void OnDisable()
    {
        UI_Controller.instance.OnInventaryUpdate -= AddItems;
        UI_Controller.instance.OnAddItemToQuickInventary -= Add;
    }
}
