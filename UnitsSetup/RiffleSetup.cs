using UnityEngine;
using System;

public class RiffleSetup : UnitSetup, IDefaultSetup
{

    [SerializeField] float damage;
    [SerializeField] float timeBetweenShoot;
    [SerializeField] string _name;
    [SerializeField] float firingRange;
   


    [SerializeField] CameraShakeSet cameraShakeSet;


    public float FiringRange()
    {
        return this.firingRange;
    }
    public float Damage()
    {
        return this.damage;
    }

    public string Name()
    {
        return this._name;
    }

    public float TimeBetweenShoot()
    {
        return this.timeBetweenShoot;
    }
    public CameraShakeSet CameraShakeSetup() { return cameraShakeSet; }

    [Serializable]
    public class CameraShakeSet { public float intensity; public float time; }

}