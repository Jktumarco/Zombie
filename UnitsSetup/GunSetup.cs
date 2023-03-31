using UnityEngine;
using System;

public class GunSetup : UnitSetup, IDefaultSetup
{

    [SerializeField] float _damage;
    [SerializeField] float _timeBetweenShoot;
    [SerializeField] string _name;
    [SerializeField] float _firingRange;
    //[SerializeField] float cameraShakeIntensity;
    //[SerializeField] float cameraShakeTime;


    [SerializeField] CameraShakeSet cameraShakeSet;


    public float FiringRange()
    {
        return this._firingRange;
    }
    public float Damage()
    {
        return this._damage;
    }

    public string Name()
    {
        return this._name;
    }

    public float TimeBetweenShoot()
    {
        return this._timeBetweenShoot;
    }
    //public float CameraShakeIntensity() { return cameraShakeIntensity; }
    public CameraShakeSet CameraShakeSetup() { return cameraShakeSet; }

    [Serializable]
    public class CameraShakeSet { public float intensity; public float time; } 

}
