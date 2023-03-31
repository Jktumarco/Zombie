using UnityEngine;
using System;

public class BatSetup : UnitSetup
{

    [SerializeField] float _damage;
    [SerializeField] string _name;
    [SerializeField] float _timeBetweenAttack;
    public float Damage => this._damage;
    public string Name => this._name;
    public float TimeBetweenAttack => this._timeBetweenAttack;
}