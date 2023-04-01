using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using Pathfinding;


public class AutomaticStrikeGun : State, Enemy.IEnemyTargetable
{
    private bool isEnter = false;
    private Transform transform;
    private bool IsSelectionCharacter = false;
    private GunSetup _gunSetup;

    private Animator animator;
    [SerializeField] Transform textPOs;
    [SerializeField] Vector3 offset;
    private AILerp _aiLerp;

    private EnemyVision _enemyVision;
    private Enemy _enemyTarget;
    private float _timeBetweenShoot;

    private Transform _target;
    private float _smooth = 56f;
    bool _isTryChaking;
   

    public AutomaticStrikeGun(CharacterSetup characterSetup, GunSetup gunSetup)
    {
        this.animator = characterSetup.Animator;
        this.transform = characterSetup.CharacterTransform;
        this._aiLerp = characterSetup.AiLerp;
        this._enemyVision = characterSetup.EnemyVision;
        this._target = characterSetup.WayTargetPoint;
        this._gunSetup = gunSetup;
    }

    public override void Update()
    {
        if (isEnter)
        {
         
            MainLogic();
        }
    }
    public override void Enter()
    {
        
        base.Enter();
        isEnter = true;
    }
    public override void Exit()
    {
       
        base.Exit();
        isEnter = false;
    }

    void AutomaticStrike()
    {
        if (_enemyTarget == null)
        {
            var anyCollsion = _enemyVision.RayToScan();
            if (anyCollsion != null)
            {
                var enemy = anyCollsion.Find(x => x.collider?.GetComponent<Enemy>());
                if (enemy.collider != null)
                {
                    Enemy enemyStriker = enemy.collider.GetComponent<Enemy>();
                    _timeBetweenShoot = _gunSetup.TimeBetweenShoot();
                    if (enemyStriker != null) { _enemyTarget = enemyStriker; }
                    //else  _enemyTarget = null; }
                }
            }
        }

        if (_enemyTarget != null)
        {
            var scan = _enemyVision.RayToScan();

            if (scan != null)
            {
                _aiLerp.enableRotation = false;
                LookAtEnemy(_enemyTarget.GetPosition());
                var curEnemy = scan.Find(x => x.collider.GetComponent<Enemy>()).collider?.GetComponent<Enemy>();
                if (curEnemy != null && curEnemy == _enemyTarget)
                {

                    if (_timeBetweenShoot <= 0)
                    {
                        OnShoot(_enemyTarget, _enemyTarget.GetPosition());
                        transform.Find("ShootLightStrike").position = this.transform.position;
                        transform.Find("ShootLightStrike").gameObject.SetActive(true);
                        FunctionTimer.Create(() => transform.Find("ShootLightStrike").gameObject.SetActive(false), .05f, "ShootLightStrike", false, true); _timeBetweenShoot = _gunSetup.TimeBetweenShoot();
                    }
                }

                var enemy = _enemyVision.RayToScan();
                if (enemy != null)
                {
                    var enemy1 = enemy.Find(x => x.collider?.GetComponent<Enemy>());
                    Enemy enemyStriker = enemy1.collider?.GetComponent<Enemy>();
                    //EnemyStriker enemyStriker = anyColl.collider?.GetComponent<EnemyStriker>();
                    if (enemyStriker != null && enemyStriker == _enemyTarget) { return; }
                    //else _enemyTarget = null;
                    if (enemyStriker != null && enemyStriker != _enemyTarget) { /*_enemyTarget = enemyStriker; */}
                }
            }
        }
    }
    bool LookAtEnemy(Vector3 hitPos)
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 characterDir = (hitPos - transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _smooth / 10 * Time.deltaTime);
        var r1 = rotation.z;
        var r2 = transform.rotation.z;
        return Mathf.Round(r1) == Mathf.Round(r2);
    }

    void OnShoot(Enemy enemyStriker, Vector2 raycastHit)
    {
        animator.Play("ShootGun");
        //CinemachineShake.Instance.ShakeCamera(6f, .1f);
        enemyStriker.Damage(this, _gunSetup.Damage(),  raycastHit);
        _aiLerp.enableRotation = false;
        _aiLerp.canMove = false;
    }  
    void ChangeTargetPointPos()
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        animator.Play("WalkGun");
        _target.position = pos;
        _aiLerp.canMove = true;
        _aiLerp.enableRotation = true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public void Damage(Enemy attacker, Vector2 hit)
    {
        throw new System.NotImplementedException();
    }

    void MainLogic()
    {
        Debug.Log(IsSelectionCharacter);
        if (_isTryChaking) { if (_aiLerp.reachedEndOfPath == true) { _aiLerp.enableRotation = false; animator.Play("IdleGun"); _isTryChaking = false; /*isBusy = false;*/ } }
        if (_timeBetweenShoot > 0)
        {
            _timeBetweenShoot -= Time.deltaTime;
        }
        AutomaticStrike(); 
    }
}

