using UnityEngine;
using CodeMonkey.Utils;
using Pathfinding;


public class GoToPointOrStrikeGun: State, Enemy.IEnemyTargetable
{
    private GunSetup _gunSetup;
    private CharacterSetup _characterSetup;
 
    private Transform _transform;
    private Animator _animator;
    private AILerp _aiLerp;

    private EnemyVision _enemyVision;
    private Enemy _enemyTarget;
    private float _timeBetweenShoot;

    private Transform Target;
    private float _smooth = 56f;
    private Character _character;
    private bool _isTryChaking;

    public GoToPointOrStrikeGun(CharacterSetup character_Setup, GunSetup gun_Setup)
    {
        this._animator = character_Setup.Animator;
        this._gunSetup = gun_Setup;
        this._transform = character_Setup.CharacterTransform;
        this._aiLerp = character_Setup.AiLerp;
        this._enemyVision = character_Setup.EnemyVision;
        this.Target = character_Setup.WayTargetPoint;
        this._characterSetup = character_Setup;
        this._character = character_Setup.Character;
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
        _animator.Play("IdleGun");
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
                    _timeBetweenShoot = _gunSetup.TimeBetweenShoot();
                    if (enemyStriker != null) { _enemyTarget = enemyStriker; }
                }
            }
        }
        if (_enemyTarget != null)
        {
            var hit = _enemyVision.RayToScan();

            if (hit != null)
            {
                _aiLerp.enableRotation = false;
                LookAtMouse(_enemyTarget.GetPosition());
                var curEnemy = hit.Find(x => x.collider.GetComponent<Enemy>()).collider?.GetComponent<Enemy>();
                if (curEnemy !=null && curEnemy == _enemyTarget)
                {
                    if (_timeBetweenShoot <= 0 && _character.Inventary.CheckingBullet("imagazine_gun"))
                    {
                        OnShoot(_enemyTarget, _enemyTarget.GetPosition());
                        Character.OnShoot?.Invoke();
                        AudioController.instance.PlaySFX("gunStrike",0.6f);
                        _transform.Find("ShootLightStrike").gameObject.SetActive(true);
                        FunctionTimer.Create(() => _transform.Find("ShootLightStrike").gameObject.SetActive(false), .05f,
                        "ShootLightStrike", false, true);
                        _timeBetweenShoot = _gunSetup.TimeBetweenShoot();
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
    void LookAtMouse(Vector3 hitPos)
    {
        Vector3 characterDir = (hitPos - _transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this._transform.rotation = Quaternion.Slerp(_transform.rotation, rotation, _smooth / 10 * Time.deltaTime);
    }

    void OnShoot(Enemy enemyStriker, Vector2 raycastHit)
    {
        _animator.Play("ShootGun");
        CinemachineShake.Instance.ShakeCamera(_gunSetup.CameraShakeSetup().intensity, _gunSetup.CameraShakeSetup().time);
        enemyStriker.Damage(this, _gunSetup.Damage(), raycastHit);
        _aiLerp.enableRotation = false;
        _aiLerp.canMove = false;
    }
    void ChangeTargetPointPos(float speed)
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        _animator.Play("WalkGun");
        var go = GameObject.Find("Cursor");
        go.transform.position = pos;
        go.GetComponentInChildren<Animator>().Play("Cursor");
        _aiLerp.speed = speed;
        Target.position = pos;
        _aiLerp.canMove = true;
        _aiLerp.enableRotation = true;
    }
   
    public Vector3 GetPosition()
    {
        return _transform.position;
    }
    public void Damage(Enemy attacker, Vector2 hit)
    {
        throw new System.NotImplementedException();
    }
    void MainLogic()
    {
       
            if (_isTryChaking) { if (_aiLerp.reachedEndOfPath == true) { _aiLerp.enableRotation = false; _animator.Play("IdleGun"); _isTryChaking = false; /*isBusy = false; */} }
        if (_timeBetweenShoot > 0)
        {
            _timeBetweenShoot -= Time.deltaTime;
        }
        if (_aiLerp.reachedEndOfPath == true) { AutomaticStrike(); }
        if (Input.GetMouseButtonUp(0))
        {
            _enemyTarget = null;
            var m = UtilsClass.GetMouseWorldPosition();
            RaycastHit2D rayHit = Physics2D.Raycast(m, m);
            if (rayHit.collider != null)
            {
                Debug.DrawRay(_transform.position, rayHit.collider.transform.position - _transform.position, Color.red, _gunSetup.FiringRange());
            }
            if (rayHit.collider != null)
            {
                Enemy enemyStriker = rayHit.collider.GetComponent<Enemy>();
                if (enemyStriker != null)
                {
                    var distance = Vector2.Distance(_transform.position, enemyStriker.GetPosition());
                    if (distance <= _gunSetup.FiringRange() && _character.Inventary.CheckingBullet("imagazine_gun"))
                    {
                        Target.position = _transform.position; _aiLerp.enableRotation = false;
                        _enemyTarget = enemyStriker; _timeBetweenShoot = _gunSetup.TimeBetweenShoot(); Debug.Log(rayHit.collider.name);
                    }
                    else if (!_character.Inventary.CheckingBullet("imagazine_gun")) { AudioController.instance.PlaySFX("isNotBullet"); _enemyTarget = enemyStriker; }
                    else if (distance >= _gunSetup.FiringRange()) { ChangeTargetPointPos(8f); _isTryChaking = true; }
                }
                else  ChangeTargetPointPos(8f); _isTryChaking = true;
            }
            else ChangeTargetPointPos(8f); _isTryChaking = true;
            //TextWriter.AddWriter_Static(bubleText, "So...I'm craze....Yeee", .03f, false, false, () => { });       }
        }
    }
}

