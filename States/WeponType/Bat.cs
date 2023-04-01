using CodeMonkey.Utils;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : State
{
    private Animator animator;
    private BatSetup kniffeSetup;
    private Transform transform;
    private AILerp _aiLerp;
    private Transform _wayPoint;
    private CharacterSetup _characterSetup;
    private bool _isTryChaking;

    private Enemy target;
    private float _timeBetweenAttack = 1f;

    public Bat(CharacterSetup characterSetup, BatSetup knifeSetup)
    {
        this.animator = characterSetup.Animator;
        this.kniffeSetup = knifeSetup;
        this.transform = characterSetup.CharacterTransform;
        this._aiLerp = characterSetup.AiLerp;
        this._wayPoint = characterSetup.WayTargetPoint;
        this._characterSetup = characterSetup;
        

    }
    public override void Enter()
    {
        animator.Play("Idle_bat");
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {

        if (_timeBetweenAttack > 0)
        {
            Debug.Log(_timeBetweenAttack);
            _timeBetweenAttack -= Time.deltaTime;
        }
       
        if (target!=null){

            Debug.Log(target);
            if (CanAttack(target.GetPosition()) && _timeBetweenAttack <= 0f)
            {
                LookAtMouse(UtilsClass.GetMouseWorldPosition());
                AudioController.instance.PlaySFX("batAttack");
                animator.Play("AttackBat");
                target.Damage(10f);
                _timeBetweenAttack = 1f;
                return;
                //target = null;
            }
        }
        MainLogic();
        base.Update();
    }

    void MainLogic()
    {
        if (_aiLerp.reachedEndOfPath == true) { _aiLerp.enableRotation = false; animator.Play("Idle_bat"); }
        if (!InputsController.instance.IsMouseOverUIWithIgnores())
        {
            if (Input.GetMouseButtonDown(0))
            {
                var m = UtilsClass.GetMouseWorldPosition();
                RaycastHit2D rayHit = Physics2D.Raycast(m, m);
                if (rayHit.collider != null)
                {
                    Enemy enemyStriker = rayHit.collider.GetComponent<Enemy>();
                    if (enemyStriker != null)
                    {
                        ChangeTargetPointPos(8f); _isTryChaking = false;
                        target = enemyStriker;
                    }
                    else ChangeTargetPointPos(8f); _isTryChaking = false;
                }
                else ChangeTargetPointPos(8f); _isTryChaking = false;
            }
        }
    }
    void ChangeTargetPointPos(float speed)
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        animator.Play("WalkBat");
        var go = GameObject.Find("Cursor");
        go.transform.position = pos;
        go.GetComponentInChildren<Animator>().Play("Cursor");
        _aiLerp.speed = speed;
        _wayPoint.position = pos;
        _aiLerp.canMove = true;
        _aiLerp.enableRotation = true;
        target = null;
    }
    bool CanAttack(Vector3 enemyPosition)
    {
        var distance = Vector2.Distance(this.transform.position, enemyPosition);
        Debug.Log(distance);
        if (distance >= 1f && distance <= 3f)
        {
            _aiLerp.canMove = false;
            return true;
        }
        else return false;
    }
    void LookAtMouse(Vector3 hitPos)
    {
        Vector3 characterDir = (hitPos - transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 40 / 10 * Time.deltaTime);
    }
}
