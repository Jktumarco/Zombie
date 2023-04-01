using UnityEngine;
using UnityEngine.UI;

public class InventaryPlace : MonoBehaviour
{
    private Image itemImage;
    private string imageName;
    private Text textAmount;
   [SerializeField] private bool isFree;
   [SerializeField] private Transform transformPlace;

    public Image ItemImage { get => itemImage; set => itemImage = value; }
    public string ImageName { get => imageName; set => imageName = value; }
    public Text TextAmount { get => textAmount; set => textAmount = value; }
    public Transform TransformPlace { get => transformPlace; set => transformPlace = value; }
    public bool IsFree { get => isFree; set => isFree = value; }
}
