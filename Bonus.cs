using UnityEngine;

public class Bonus : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] float speedRotate;
    [SerializeField] public string bonusName;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var bonusList = GameLoader.instance.GetAllBonusIcons();
        var randomBonus = Random.Range(0, bonusList.Length);
        var randomAmount = Random.Range(15, 80);
        if (bonusList[randomBonus].name == "cylinder" || bonusList[randomBonus].name == "item_health") { randomAmount = 1; }
        string nameBonus = bonusList[randomBonus].name + ',' + randomAmount.ToString();
        spriteRenderer.sprite = bonusList[randomBonus];
        bonusName = bonusList[randomBonus].name + ',' + randomAmount.ToString();
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
