using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    [SerializeField] Sprite[] spritesBonusIcon;
    [SerializeField] Sprite[] spritesBonusWeaponIcon;
    [SerializeField] List<Sprite> allIconList;
    [SerializeField] AudioClip[] audioClips;
    List<string> nameIconsList;
    private void OnEnable()
    {
        nameIconsList = new List<string>();
        instance = this;
        spritesBonusIcon = Resources.LoadAll<Sprite>("InventaryBonusIcon");
        spritesBonusWeaponIcon = Resources.LoadAll<Sprite>("InventaryWeaponIcon");
        audioClips = Resources.LoadAll("Audio", typeof(AudioClip)).Cast<AudioClip>().ToArray();

    }
    private void Start()
    {

        foreach (var item in spritesBonusIcon)
        {
            allIconList.Add(item);
        }
        foreach (var item in spritesBonusWeaponIcon)
        {
            allIconList.Add(item);
        }
    }
    public AudioClip GetAudioByName(string name)
    {
        foreach (var item in audioClips)
        {
            if(item.name == name) { return item; }
        }
        return null;
    }
    public List<Sprite> GetAllIcons() { return allIconList; }
    public Sprite[] GetAllWeaponIcons() { return spritesBonusWeaponIcon; }
    public Sprite[] GetAllBonusIcons() { return spritesBonusIcon; }
    public List<string> GetNameIconsList() { return nameIconsList; }
    public  Sprite GetIconByName(string nameIcon)
    {
        foreach (var item in spritesBonusIcon)
        {
            if (item.name == nameIcon) { return item; }
        }
        foreach (var item in spritesBonusWeaponIcon)
        {
            if (item.name == nameIcon) { return item; }
        }
        return null;
    }
}
