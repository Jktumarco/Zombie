using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Inventary: MonoBehaviour
{
    [SerializeField] public static string lastDropItem;
    public List<string> itemsMainInventary;
    public List<string> itemsQuickInventary;

    public void DeleteFromMainInventary(string item)
    {
       var curItem = itemsMainInventary.Find((i) => GetFormatingName(i) == item);
        itemsMainInventary.Remove(curItem);
    }
    public void DeleteFromQuickInventary(string item) {
        var curItem = itemsQuickInventary.Find((i) => GetFormatingName(i) == item);
        itemsQuickInventary.Remove(curItem);
    }
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
    private void UseHealth()
    {
        RemoveBullet("item_health");
        AudioController.instance.PlaySFX("health_1");
    }
    public void DefaultInventary() { itemsMainInventary.Clear(); itemsQuickInventary.Clear(); itemsQuickInventary.Add("gun"); itemsMainInventary.Add("imagazine_gun,100"); }

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
        itemsMainInventary.Remove(bullet);
        itemsMainInventary.Add(newName);
    }
    void CheckAndRemoveItemZeroAmount() {
        var bullet = itemsMainInventary.Find((i) => GetFormatingAmount(i) == "0");
        if (bullet != null) { 
            UI_Controller.instance.OnDestroyItemFromAllInventary?.Invoke(bullet); 
            itemsMainInventary.Remove(bullet); }   
        var bulletFromQuickInventary = itemsQuickInventary.Find((i) => GetFormatingName(i) == "0");
        if (bulletFromQuickInventary != null) {
            UI_Controller.instance?.OnDestroyItemFromAllInventary?.Invoke(bulletFromQuickInventary); 
            itemsQuickInventary.Remove(bulletFromQuickInventary);
        } 
    }
    string FindInInventaryes(string nameBullet)
    {
        var bullet = itemsMainInventary.Find((i) => GetFormatingName(i) == nameBullet);
        if (bullet != null) { return bullet; }
        var bulletFromQuickInventary = itemsQuickInventary.Find((i) => GetFormatingName(i) == nameBullet);
        if (bulletFromQuickInventary != null) { return bulletFromQuickInventary; }
        return default;
    }
    public void AddToMainInventary(string itemName) 
    {
        var amountItem = itemsMainInventary.Find((i) => GetFormatingName(i) == GetFormatingName(itemName));
       
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
        itemsMainInventary.Add(itemName);
        List<string> listAllCurItem = new List<string>();
        foreach (var item in itemsMainInventary)
        {
            listAllCurItem.Add(GetFormatingName(item));
        }
        itemsMainInventary.RemoveAll((i) => GetFormatingName(i) == GetFormatingName(itemName));
        itemsMainInventary.Add(GetFormatingName(itemName) + "," + amountItemInt);  
    }
    public bool CheckingBullet(string nameWeaponBullet)
    {
        var curWeapon = itemsMainInventary.Find((b) => GetFormatingName(b) == nameWeaponBullet);
        var curWeaponInQuickInvemtary = itemsQuickInventary.Find((b) => GetFormatingName(b) == nameWeaponBullet);
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
        var amountItem = itemsQuickInventary.Find((i) => GetFormatingName(i) == GetFormatingName(itemName));
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
        itemsQuickInventary.Add(itemName);
        List<string> listAllCurItem = new List<string>();
        foreach (var item in itemsQuickInventary)
        {
            listAllCurItem.Add(GetFormatingName(item));
        }
        itemsQuickInventary.RemoveAll((i) => GetFormatingName(i) == GetFormatingName(itemName));
        itemsQuickInventary.Add(GetFormatingName(itemName) + "," + amountItemInt);
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
