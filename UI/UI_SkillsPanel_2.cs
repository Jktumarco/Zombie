using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UI_SkillsPanel_2 : UI_Base
{
    [SerializeField] int timeMAX;
    [SerializeField] float timeFloat;
    [SerializeField] int timeValue;
    [SerializeField] bool isFull;
    static public Action<bool> OnAvailable;
    void Start()
    {
        timeFloat = (float)timeValue;
    }
    public void UseSkill(Image imageSkill)
    {
        if (isFull)
        {
            StartCoroutine(TimeFillAmountDown(imageSkill));
        }
        else StartCoroutine(TimeFillAmountUp(imageSkill));
    }
    IEnumerator TimeFillAmountUp(Image image)
    {
        timeFloat = 0;
        while (timeFloat < timeMAX)
        {
            timeFloat += Time.deltaTime / 2;
            image.fillAmount = timeFloat / timeValue;
            yield return null;
        }
        isFull = true;
        StopCoroutine(TimeFillAmountUp(image));

    }
    IEnumerator TimeFillAmountDown(Image image)
    {

        while (timeFloat > 0)
        {
            timeFloat -= Time.deltaTime / 2;
            image.fillAmount = timeFloat / timeValue;
            yield return null;
        }
        isFull = false;
        StopCoroutine(TimeFillAmountDown(image));

    }

}
