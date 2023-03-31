using UnityEngine;
using CodeMonkey.Utils;
using Pathfinding;


public class GoToPointOrStrikeGun: State, Enemy.IEnemyTargetable
{
    GunSetup GunSetup;
    CharacterSetup CharacterSetup;
    private bool _isEnter = false;
    private Transform Transform;
    private bool IsSelectionCharacter = false;


    private Animator Animator;
    private InputsController MousInput;
    [SerializeField] Transform textPOs;
    [SerializeField] Vector3 _offset;
    private AILerp AiLerp;

    private EnemyVision EnemyVision;
    private Enemy EnemyTarget;
    private float _timeBetweenShoot;

    private Transform Target;
    private float _smooth = 56f;
    bool isBusy;
    Character Character;
    
    bool _isTryChaking;
    bool _isOverUi;

    public GoToPointOrStrikeGun(CharacterSetup character_Setup, GunSetup gun_Setup)
    {
        this.Animator = character_Setup.animator;
        this.GunSetup = gun_Setup;
        this.Transform = character_Setup.characterTransform;
        this.AiLerp = character_Setup.aiLerp;
        this.EnemyVision = character_Setup.enemyVision;
        this.Target = character_Setup.wayTargetPoint;
        this.CharacterSetup = character_Setup;
        this.Character = character_Setup.character;
    }
    

    public override void Update()
    {
        if (!InputsController.instance.IsMouseOverUIWithIgnores())
        {
            if (_isEnter)
            {
                MainLogic();
            }
        }
    }
    public override void Enter()
    {
        Animator.Play("IdleGun");
        base.Enter();
        _isEnter = true;
    }
    public override void Exit()
    {
        base.Exit();
        _isEnter = false;
    }

    void AutomaticStrike()
    {
        if (EnemyTarget == null)
        {
            var anyCollsion = EnemyVision.RayToScan();
            if (anyCollsion != null)
            {
                var enemy = anyCollsion.Find(x => x.collider?.GetComponent<Enemy>());
                if (enemy.collider != null)
                {
                    Enemy enemyStriker = enemy.collider.GetComponent<Enemy>();
                    _timeBetweenShoot = GunSetup.TimeBetweenShoot();
                    if (enemyStriker != null) { EnemyTarget = enemyStriker; }
                }
            }
        }
        if (EnemyTarget != null)
        {
            var hit = EnemyVision.RayToScan();

            if (hit != null)
            {
                AiLerp.enableRotation = false;
                LookAtMouse(EnemyTarget.GetPosition());
                var curEnemy = hit.Find(x => x.collider.GetComponent<Enemy>()).collider?.GetComponent<Enemy>();
                if (curEnemy !=null && curEnemy == EnemyTarget)
                {
                    if (_timeBetweenShoot <= 0 && Character.inventary.CheckingBullet("imagazine_gun"))
                    {
                        OnShoot(EnemyTarget, EnemyTarget.GetPosition());
                        Character.OnShoot?.Invoke();
                        AudioController.instance.PlaySFX("gunStrike",0.6f);
                        Transform.Find("ShootLightStrike").gameObject.SetActive(true);
                        FunctionTimer.Create(() => Transform.Find("ShootLightStrike").gameObject.SetActive(false), .05f,
                        "ShootLightStrike", false, true);
                        _timeBetweenShoot = GunSetup.TimeBetweenShoot();
                    }
                }
            }
            var hitSecond = EnemyVision.RayToScan();
            if (hitSecond != null)
            {
                var anyEnemy = hitSecond.Find(x => x.collider?.GetComponent<Enemy>());
                Enemy curEnemy = anyEnemy.collider?.GetComponent<Enemy>();  
                if (curEnemy != null && curEnemy == EnemyTarget) { return; } 
            }
        }
    }
    void LookAtMouse(Vector3 hitPos)
    {
        Vector3 characterDir = (hitPos - Transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this.Transform.rotation = Quaternion.Slerp(Transform.rotation, rotation, _smooth / 10 * Time.deltaTime);
    }

    void OnShoot(Enemy enemyStriker, Vector2 raycastHit)
    {
        Animator.Play("ShootGun");
        CinemachineShake.Instance.ShakeCamera(GunSetup.CameraShakeSetup().intensity, GunSetup.CameraShakeSetup().time);
        enemyStriker.Damage(this, GunSetup.Damage(), raycastHit);
        AiLerp.enableRotation = false;
        AiLerp.canMove = false;
    }
    void ChangeTargetPointPos(float speed)
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        Animator.Play("WalkGun");
        var go = GameObject.Find("Cursor");
        go.transform.position = pos;
        go.GetComponentInChildren<Animator>().Play("Cursor");
        AiLerp.speed = speed;
        Target.position = pos;
        AiLerp.canMove = true;
        AiLerp.enableRotation = true;
    }
   
    public Vector3 GetPosition()
    {
        return Transform.position;
    }
    public void Damage(Enemy attacker, Vector2 hit)
    {
        throw new System.NotImplementedException();
    }
    void MainLogic()
    {
       
            if (_isTryChaking) { if (AiLerp.reachedEndOfPath == true) { AiLerp.enableRotation = false; Animator.Play("IdleGun"); _isTryChaking = false; isBusy = false; } }
        if (_timeBetweenShoot > 0)
        {
            _timeBetweenShoot -= Time.deltaTime;
        }
        if (AiLerp.reachedEndOfPath == true) { AutomaticStrike(); }
        if (Input.GetMouseButtonUp(0))
        {
            EnemyTarget = null;
            var m = UtilsClass.GetMouseWorldPosition();
            RaycastHit2D rayHit = Physics2D.Raycast(m, m);
            if (rayHit.collider != null)
            {
                Debug.DrawRay(Transform.position, rayHit.collider.transform.position - Transform.position, Color.red, GunSetup.FiringRange());
            }
            if (rayHit.collider != null)
            {
                Enemy enemyStriker = rayHit.collider.GetComponent<Enemy>();
                if (enemyStriker != null)
                {
                    var distance = Vector2.Distance(Transform.position, enemyStriker.GetPosition());
                    if (distance <= GunSetup.FiringRange() && Character.inventary.CheckingBullet("imagazine_gun"))
                    {
                        Target.position = Transform.position; AiLerp.enableRotation = false;
                        EnemyTarget = enemyStriker; _timeBetweenShoot = GunSetup.TimeBetweenShoot(); Debug.Log(rayHit.collider.name);
                    }
                    else if (!Character.inventary.CheckingBullet("imagazine_gun")) { AudioController.instance.PlaySFX("isNotBullet"); EnemyTarget = enemyStriker; }
                    else if (distance >= GunSetup.FiringRange()) { ChangeTargetPointPos(8f); _isTryChaking = true; }
                }
                else  ChangeTargetPointPos(8f); _isTryChaking = true;
            }
            else ChangeTargetPointPos(8f); _isTryChaking = true;
            //TextWriter.AddWriter_Static(bubleText, "So...I'm craze....Yeee", .03f, false, false, () => { });       }
        }
    }
}

