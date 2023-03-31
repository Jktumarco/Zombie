using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
using Pathfinding;


public class Riffle : State, Enemy.IEnemyTargetable
{
    RiffleSetup _riffleSetup;
    CharacterSetup _characterSetup;
    private bool isEnter = false;
    private Transform transform;
    private bool IsSelectionCharacter = false;


    private Animator animator;
    private InputsController mousInput;
    [SerializeField] Transform textPOs;
    [SerializeField] Vector3 offset;
    private AILerp _aiLerp;

    private EnemyVision _enemyVision;
    private Enemy _enemyTarget;
    private float _timeBetweenShoot;

    private Transform _target;
    private float _smooth = 56f;
    bool isBusy;
    Character character;

    bool _isTryChaking;
    bool isOverUi;

    public Riffle(CharacterSetup characterSetup, RiffleSetup riffleSetup)
    {
        this.animator = characterSetup.animator;
        this._riffleSetup = riffleSetup;
        this.transform = characterSetup.characterTransform;
        this._aiLerp = characterSetup.aiLerp;
        this._enemyVision = characterSetup.enemyVision;
        this._target = characterSetup.wayTargetPoint;
        this._characterSetup = characterSetup;
        this.character = characterSetup.character;
    }


    public override void Update()
    {
        if (!InputsController.instance.IsMouseOverUIWithIgnores())
        {
            MainLogic();   
        }
    }
    public override void Enter()
    {
        animator.Play("Idle_riffle");
        base.Enter();  
    }
    public override void Exit()
    {
        base.Exit();
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
                    _timeBetweenShoot = _riffleSetup.TimeBetweenShoot();
                    if (enemyStriker != null) { _enemyTarget = enemyStriker; }
                }
            }
        }
        if (_enemyTarget != null)
        {
            var hit
                = _enemyVision.RayToScan();

            if (hit != null)
            {
                _aiLerp.enableRotation = false;
                LookAtMouse(_enemyTarget.GetPosition());
                var curEnemy = hit.Find(x => x.collider.GetComponent<Enemy>()).collider?.GetComponent<Enemy>();
                if (curEnemy != null && curEnemy == _enemyTarget)
                {
                    if (_timeBetweenShoot <= 0 && character.inventary.CheckingBullet("magazine_riffle"))
                    {
                        OnShoot(_enemyTarget, _enemyTarget.GetPosition());
                        Character.OnShoot?.Invoke();
                        AudioController.instance.PlaySFX("riffleAttack_new_1", 1,GetRandom(0.95f,1f));
                        transform.Find("ShootLightStrike").gameObject.SetActive(true);
                        FunctionTimer.Create(() => transform.Find("ShootLightStrike").gameObject.SetActive(false), .02f,
                        "ShootLightStrike", false, true);
                        _timeBetweenShoot = _riffleSetup.TimeBetweenShoot();
                    }
                }
            }
            var hitSecond = _enemyVision.RayToScan();
            if (hitSecond != null)
            {
                var anyEnemy = hitSecond.Find(x => x.collider?.GetComponent<Enemy>());
                Enemy curEnemy = anyEnemy.collider?.GetComponent<Enemy>();
                if (curEnemy != null && curEnemy == _enemyTarget) { return; }
            }
        }
    }
    float GetRandom(float min, float max) { return Random.Range(min, max); }
    void LookAtMouse(Vector3 hitPos)
    {
        Vector3 characterDir = (hitPos - transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _smooth / 5 * Time.deltaTime);
    }

    void OnShoot(Enemy enemyStriker, Vector2 raycastHit)
    {
        animator.Play("Shoot_riffle");
        CinemachineShake.Instance.ShakeCamera(_riffleSetup.CameraShakeSetup().intensity, _riffleSetup.CameraShakeSetup().time);
        enemyStriker.Damage(this, _riffleSetup.Damage(), raycastHit);
        _aiLerp.enableRotation = false;
        _aiLerp.canMove = false;
    }
    void ChangeTargetPointPos(float speed)
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        animator.Play("Walk_riffle");
        var go = GameObject.Find("Cursor");
        go.transform.position = pos;
        go.GetComponentInChildren<Animator>().Play("Cursor");
        _aiLerp.speed = speed;
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
        if (_isTryChaking) { if (_aiLerp.reachedEndOfPath == true) { _aiLerp.enableRotation = false; animator.Play("Idle_riffle"); _isTryChaking = false; isBusy = false; } }
        if (_timeBetweenShoot > 0)
        {
            _timeBetweenShoot -= Time.deltaTime;
        }
        if (_aiLerp.reachedEndOfPath == true) { Debug.Log(_aiLerp.reachedEndOfPath); AutomaticStrike(); }
        if (Input.GetMouseButtonUp(0))
        {
            _enemyTarget = null;
            var m = UtilsClass.GetMouseWorldPosition();
            RaycastHit2D rayHit = Physics2D.Raycast(m, m);
            if (rayHit.collider != null)
            {
                Debug.DrawRay(transform.position, rayHit.collider.transform.position - transform.position, Color.red, _riffleSetup.FiringRange());
            }
            if (rayHit.collider != null)
            {
                Enemy enemyStriker = rayHit.collider.GetComponent<Enemy>();
                if (enemyStriker != null)
                {
                    Debug.Log(enemyStriker);
                    var distance = Vector2.Distance(transform.position, enemyStriker.GetPosition());
                    if (distance <= _riffleSetup.FiringRange())
                    {
                        _target.position = transform.position; _aiLerp.enableRotation = false;
                        _enemyTarget = enemyStriker; _timeBetweenShoot = _riffleSetup.TimeBetweenShoot(); Debug.Log(rayHit.collider.name);
                        //var go = GameObject.Find("CursorAttack");
                        //go.transform.position = m;
                        //go.SetActive(true);
                        //go.GetComponentInChildren<Animator>().Play("CursorAttack");
                    }
                    else if (distance >= _riffleSetup.FiringRange()) { ChangeTargetPointPos(8f); _isTryChaking = true; }
                }
                else ChangeTargetPointPos(8f); _isTryChaking = true;
            }
            else ChangeTargetPointPos(8f); _isTryChaking = true;
            //TextWriter.AddWriter_Static(bubleText, "So...I'm craze....Yeee", .03f, false, false, () => { });       }
        }
    }
}

