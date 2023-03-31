using UnityEngine;

public class BonusWeapon : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] float speedRotate;
    [SerializeField] public string bonusName;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var bonusList = GameLoader.instance.GetAllWeaponIcons();
        spriteRenderer.sprite = GameLoader.instance.GetIconByName(bonusName);
        var amount = 1;
        bonusName = bonusName + ',' + amount.ToString();
    }


    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1) * speedRotate);
    }
    public void SetData(string name, Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        bonusName = name;
    }

}
