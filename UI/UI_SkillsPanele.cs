using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UI_SkillsPanele : UI_Base
{
    [SerializeField] int timeMAX;
    [SerializeField] float timeFloat;
    [SerializeField] int timeValue;
    [SerializeField] bool isFull;
    [SerializeField] bool canUseSkill = true;
    static public Action<bool> OnAvailable;
    void Start()
    {
        timeFloat = (float)timeValue;
    }
    public void UseSkill(Image imageSkill)
    {
        if (canUseSkill)
        {
            UI_Controller.instance?.OnUseHealth.Invoke();
            UI_Controller.instance?.OnHealth.Invoke(40);
            //StartCoroutine(TimeFillAmountDown(imageSkill));
        }
    }
    IEnumerator TimeFillAmountUp(Image image)
    {
        canUseSkill = false;
        timeFloat = 0;
        while (timeFloat < timeMAX)
        {
            timeFloat += Time.deltaTime /2;
            image.fillAmount = timeFloat / timeValue;
            yield return null;
        }
        isFull = true;
        canUseSkill = true;
        StopCoroutine(TimeFillAmountUp(image));
        
    }
    IEnumerator TimeFillAmountDown(Image image)
    {
        canUseSkill = false;
        while (timeFloat > 0)
        {
            timeFloat -= Time.deltaTime / 2;
            image.fillAmount = timeFloat / timeValue;
            yield return null;
        }
        isFull = false;
        canUseSkill = true;
        StopCoroutine(TimeFillAmountDown(image));

    }

}
