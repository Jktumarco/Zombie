using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D != null)
        {
            Debug.Log(collider2D.name);
            Bonus bonus = collider2D.gameObject.GetComponent<Bonus>();
            BonusWeapon bonusWeapon = collider2D.gameObject.GetComponent<BonusWeapon>();
            if (bonus != null)
            {
                GetComponent<Character>().inventary.AddToMainInventary(bonus.bonusName);
                UI_Controller.instance.OnInventaryUpdate.Invoke();
                Destroy(collider2D.gameObject);
                AudioController.instance.PlaySFX("onDrop_2");
            }
            if (bonusWeapon != null)
            {
                GetComponent<Character>().inventary.AddToMainInventary(bonusWeapon.bonusName);
                UI_Controller.instance.OnInventaryUpdate.Invoke();
                Destroy(collider2D.gameObject);
                AudioController.instance.PlaySFX("onDrop_2");
            }
        }
    }
}
