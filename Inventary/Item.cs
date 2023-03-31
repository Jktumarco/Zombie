using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public  class Item: MonoBehaviour
{
    [SerializeField] public string itemName;
    [SerializeField] public Transform transformPoint;
    [SerializeField] public Transform pointObject;
    public string GetItemName() { return itemName; }
    public Vector3 GetItemPoint() { return transformPoint.position; }
}
