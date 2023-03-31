using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    Image healthBar;
    [SerializeField] CanvasGroup canvasGroup;
    void Awake()
    {
        healthBar = GetComponent<Image>();
    }
    private void Start()
    {
        UI_Controller.instance.OnDamage += HealthDown;
        UI_Controller.instance.OnHealth += UpdateUpHealth;
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) { UpdateUpHealth(10); }
        //if (Input.GetKeyDown(KeyCode.Q)) { UpdateDownHealth(10); }
    }
    void UpdateUpHealth(float damage) { StartCoroutine(TimeFillAmountUp(damage)); }
    void UpdateDownHealth(float damage) { StartCoroutine(TimeFillAmountDown(damage)); }
    IEnumerator TimeFillAmountUp(float damage)
    {

        var healthresault = healthBar.fillAmount + damage/100;
        Debug.Log(healthBar.fillAmount);
        Debug.Log(healthresault);
        // float timeValue = 100f;
        float timeFloat = 0;
        while (healthBar.fillAmount <= healthresault)
        {
            timeFloat += Time.deltaTime;
            healthBar.fillAmount += timeFloat/ 100;
            yield return null;
        }
        //isFull = true;
        StopCoroutine(TimeFillAmountUp(damage));
    }
    IEnumerator TimeFillAmountDown(float damage)
    {
        if (canvasGroup.alpha > 0)
        {
            var healthresault = canvasGroup.alpha - damage;
            Debug.Log(healthBar.fillAmount);
            Debug.Log(healthresault);
            if (healthresault >= 0)
            {
                float timeFloat = 0;
                while (canvasGroup.alpha >= healthresault)
                {
                    timeFloat += Time.deltaTime * 40f;
                    canvasGroup.alpha -= timeFloat / 100;
                    yield return null;
                }
                healthresault = canvasGroup.alpha + damage;
                while (canvasGroup.alpha <= healthresault)
                {
                    timeFloat += Time.deltaTime * 40f;
                    canvasGroup.alpha += timeFloat / 100;
                    yield return null;
                }
                StopCoroutine(TimeFillAmountDown(damage));
            }
        }
    }
    void HealthDown(float damage)
    {
        StopAllCoroutines();
        //UpdateDownHealth(damage);
        healthBar.fillAmount -= damage / 100;
    }
    void HealthUp(float damage)
    {
        UpdateDownHealth(damage);
        healthBar.fillAmount += damage / 100;
    }
    private void OnDisable()
    {
        UI_Controller.instance.OnDamage -= HealthDown;
        UI_Controller.instance.OnHealth -= UpdateUpHealth;
    }

}
