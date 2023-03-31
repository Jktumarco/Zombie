using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ShockWave : State
{
    public Transform _transform;
    public Vector3 target;
    public float speed;

    public bool isComplite = false;
    public ShockWave(Vector3 target, Transform transform, float speed) {
        this._transform = transform;
        this.target = target;
        this.speed = speed;
    }
    public ShockWave(Transform transform) {
        this._transform = transform;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
       
    }


    public override void Update()
    {
        if (isComplite == false)
        {
            if(_transform.position == target) { isComplite = true; target = default; }
            Debug.Log("I workin");
            Vector3 a = _transform.position;
            Vector3 b = target;
            _transform.position = Vector3.MoveTowards(a, b, speed);
        }
    }
   
   
}
