using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Inventary: MonoBehaviour
{
    [SerializeField] static string lastDropItem;
    [SerializeField] List<string> itemsMainInventary;
    [SerializeField] List<string> itemsQuickInventary;

    public List<string> ItemsMainInventary { get => itemsMainInventary; set => itemsMainInventary = value; }
    public List<string> ItemsQuickInventary { get => itemsQuickInventary; set => itemsQuickInventary = value; }

    private void OnEnable()
    {
        Character.OnShoot += DeleteBullet;
        Character.OnShoot += CheckAndRemoveItemZeroAmount;
    }
    private void Start()
    {
        UI_Controller.instance.OnDeleteItemFromAllInventary += DeleteFromMainInventary;
        UI_Controller.instance.OnDeleteItemFromAllInventary += DeleteFromQuickInventary;
        UI_Controller.instance.OnAddItemToMainInventary += AddToMainInventary;
        UI_Controller.instance.OnAddItemToQuickInventary += AddToQuickInventary;
        UI_Controller.instance.OnUseHealth += UseHealth;
        UI_Controller.instance.OnUseHealth += CheckAndRemoveItemZeroAmount;
        UI_Controller.instance.OnStart += DefaultInventary;
    }
    public void DeleteFromMainInventary(string item)
    {
        var curItem = ItemsMainInventary.Find((i) => GetFormatingName(i) == item);
        ItemsMainInventary.Remove(curItem);
    }
    public void DeleteFromQuickInventary(string item)
    {
        var curItem = ItemsQuickInventary.Find((i) => GetFormatingName(i) == item);
        ItemsQuickInventary.Remove(curItem);
    }
    private void UseHealth()
    {
        RemoveBullet("item_health");
        AudioController.instance.PlaySFX("health_1");
    }
    public void DefaultInventary()
    { 
        ItemsMainInventary.Clear();
        ItemsQuickInventary.Clear(); 
        ItemsQuickInventary.Add("gun");
        ItemsMainInventary.Add("imagazine_gun,100"); 
    }

    void DeleteBullet()
    {
        switch (Character.curWeapon)
        {
            case "gun":
                RemoveBullet("imagazine_gun");
                break;
            case "cylinder": 
                RemoveBullet("cylinder");
                break;
            case "riffle": 
                RemoveBullet("magazine_riffle");
                break; 
        }
    }
    void RemoveBullet( string nameBullet) {
        var bullet = FindInInventaryes(nameBullet);
        if (bullet != null)
        {
            RemoveByName(bullet);
        }
    }
    void RemoveByName(string bullet)
    {
        var itemName = GetFormatingName(bullet);
        var newAmount = int.Parse(GetFormatingAmount(bullet));
        newAmount--;
        var newName = itemName + ',' + newAmount.ToString();
        ItemsMainInventary.Remove(bullet);
        ItemsMainInventary.Add(newName);
    }
    void CheckAndRemoveItemZeroAmount() {
        var bullet = ItemsMainInventary.Find((i) => GetFormatingAmount(i) == "0");
        if (bullet != null) { 
            UI_Controller.instance.OnDestroyItemFromAllInventary?.Invoke(bullet); 
            ItemsMainInventary.Remove(bullet); }   
        var bulletFromQuickInventary = ItemsQuickInventary.Find((i) => GetFormatingName(i) == "0");
        if (bulletFromQuickInventary != null) {
            UI_Controller.instance?.OnDestroyItemFromAllInventary?.Invoke(bulletFromQuickInventary); 
            ItemsQuickInventary.Remove(bulletFromQuickInventary);
        } 
    }
    string FindInInventaryes(string nameBullet)
    {
        var bullet = ItemsMainInventary.Find((i) => GetFormatingName(i) == nameBullet);
        if (bullet != null) { return bullet; }
        var bulletFromQuickInventary = ItemsQuickInventary.Find((i) => GetFormatingName(i) == nameBullet);
        if (bulletFromQuickInventary != null) { return bulletFromQuickInventary; }
        return default;
    }
    public void AddToMainInventary(string itemName) 
    {
        var amountItem = ItemsMainInventary.Find((i) => GetFormatingName(i) == GetFormatingName(itemName));
       
        int amountItemInt = default;
        if (amountItem != null)
        {
            amountItemInt = int.Parse(GetFormatingAmount(amountItem));
            amountItemInt += amountItemInt;
            Debug.Log(amountItem);
        }
        if (amountItem == null)
        {
            amountItemInt = int.Parse(GetFormatingAmount(itemName));
        }
        ItemsMainInventary.Add(itemName);
        List<string> listAllCurItem = new List<string>();
        foreach (var item in ItemsMainInventary)
        {
            listAllCurItem.Add(GetFormatingName(item));
        }
        ItemsMainInventary.RemoveAll((i) => GetFormatingName(i) == GetFormatingName(itemName));
        ItemsMainInventary.Add(GetFormatingName(itemName) + "," + amountItemInt);  
    }
    public bool CheckingBullet(string nameWeaponBullet)
    {
        var curWeapon = ItemsMainInventary.Find((b) => GetFormatingName(b) == nameWeaponBullet);
        var curWeaponInQuickInvemtary = ItemsQuickInventary.Find((b) => GetFormatingName(b) == nameWeaponBullet);
        if (curWeapon != null) { 
            var amount = GetFormatingAmount(curWeapon);
            if (int.Parse(amount) > 0) { return true; }
            else return false;
        }
        if (curWeaponInQuickInvemtary != null)
        {
            var amount = GetFormatingAmount(curWeaponInQuickInvemtary);
            if (int.Parse(amount) > 0) { return true; }
            else return false;
        }
        else return false;
    }
    public void AddToQuickInventary(string itemName)
    {
        var amountItem = ItemsQuickInventary.Find((i) => GetFormatingName(i) == GetFormatingName(itemName));
        Debug.Log(itemName);
        int amountItemInt = default;
        if (amountItem != null)
        {
            amountItemInt = int.Parse(GetFormatingAmount(amountItem));
            amountItemInt += amountItemInt;
        }
        if (amountItem == null)
        {
            amountItemInt = int.Parse(GetFormatingAmount(itemName));
        }
        ItemsQuickInventary.Add(itemName);
        List<string> listAllCurItem = new List<string>();
        foreach (var item in ItemsQuickInventary)
        {
            listAllCurItem.Add(GetFormatingName(item));
        }
        ItemsQuickInventary.RemoveAll((i) => GetFormatingName(i) == GetFormatingName(itemName));
        ItemsQuickInventary.Add(GetFormatingName(itemName) + "," + amountItemInt);
    }
    string GetFormatingName(string itemName)
    {
       return UtilsClass.GetByFormatTextByIndex(itemName,0);
    }
    string GetFormatingAmount(string itemName)
    {
       return UtilsClass.GetByFormatTextByIndex(itemName, 1);
    }
    private void OnDisable()
    {
        Character.OnShoot -= DeleteBullet;
        Character.OnShoot -= CheckAndRemoveItemZeroAmount;
        UI_Controller.instance.OnDeleteItemFromAllInventary -= DeleteFromMainInventary;
        UI_Controller.instance.OnDeleteItemFromAllInventary -= DeleteFromQuickInventary;
        UI_Controller.instance.OnAddItemToMainInventary -= AddToMainInventary;
        UI_Controller.instance.OnAddItemToQuickInventary -= AddToQuickInventary;
        UI_Controller.instance.OnUseHealth -= CheckAndRemoveItemZeroAmount;
        UI_Controller.instance.OnUseHealth -= UseHealth;
        UI_Controller.instance.OnStart -= DefaultInventary;
    }
}
