using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyState { isIdle, isShock, isAttack, isFollowing, isDeath }
public class Enemy : MonoBehaviour
{
    Action OnChangeState;
    [SerializeField] CharacterSetup enemySetup;
    [SerializeField] EnemyState _enemyState = new EnemyState();
    [SerializeField] float health = 10;
    [SerializeField] Transform bloodPoint;

    [SerializeField] List<State> statesEnemy = new List<State>();
    StateMachine _stateEnemyMachine;

    [SerializeField] Character character;
    [SerializeField] float distForAttack = 5f;
    [SerializeField] float damageAttack;

    [SerializeField] Behind behind;
    private float distantionToCharacter;
    private Transform selectTexture;

    public interface IEnemyTargetable
    {
        Vector3 GetPosition();
        void Damage(Enemy attacker, Vector2 hit);
    }
    private void OnEnable()
    {
        OnChangeState += SwithState;
    }
  
    private void Start()
    {
        UI_Controller.instance.OnDie += TargetNull;
        selectTexture = transform.Find("texture_2");
        character = FindObjectOfType<Character>().TargetForEnemy;
        enemySetup = GetComponent<CharacterSetup>();
        _stateEnemyMachine = new StateMachine();
        LoadAllState();
        _stateEnemyMachine.Initialize(GetStateByType<EnemyIdleState>(statesEnemy) as EnemyIdleState);
        _enemyState = EnemyState.isIdle;
    }

    private void Update()
    {
        if (character != null)
        {
            UpdateDistantionToCharacter();
        }
        
        if (!IsDead() && character != null)
        {
            if (CanFollow()) { _enemyState = EnemyState.isFollowing; OnChangeState.Invoke(); }
            if (CanAttack()) { _enemyState = EnemyState.isAttack; OnChangeState.Invoke(); }
            _stateEnemyMachine.CurrentState.Update();
        }
    }
    void LoadAllState()
    {
        statesEnemy.Add(new ShockWave(transform));
        statesEnemy.Add(new EnemyIdleState(enemySetup));
        statesEnemy.Add(new EnemyFollowState(enemySetup, character));
        statesEnemy.Add(new EnemyAttackState(enemySetup, character.transform, character));
    }
    bool CanAttack() { 
        if (distantionToCharacter >=0f && distantionToCharacter <=4f)
        {
            return true;
        }
        else return false;  
    }
    bool CanFollow()
    {
        if (distantionToCharacter >= 4f && distantionToCharacter <=21f)
        {
            return true;
        }
        else return false;
    }
    public void Damage(float damage)
    {
        Vector3 bloodDir = (Vector3.zero - GetPosition()).normalized;
        BloodParticleSystemHandler.Instance.SpawnBlood(transform.position, bloodDir);
        health -= damage;
        
        if (IsDead())
        {
            _enemyState = EnemyState.isDeath; OnChangeState.Invoke();
            enemySetup.AIPath.canMove = false;
            //selectTexture.gameObject.SetActive(false);
            Destroy(this);
            GetRandomBonus();
            AudioController.instance.PlaySFX("zombyDeath");
           
        }
    }
    public void Damage(float speedShock, Vector3 target)
    {
        Vector3 bloodDir = (target - GetPosition()).normalized;
        for (int i = 0; i < 6; i++)
        {
            BloodParticleSystemHandler.Instance.SpawnBlood(transform.position, bloodDir);
        }
        health -= 100f;
        ShockWave(speedShock, target);
        if (IsDead())
        {
            _enemyState = EnemyState.isDeath; OnChangeState.Invoke();
            enemySetup.AIPath.canMove = false;
            AudioController.instance.PlaySFX("zombyDeath");
            Destroy(this);
            GetRandomBonus(); 
        }
    }
    void IsVisibleSelectTexture()
    {
        if (distantionToCharacter < 22f)
        {
            selectTexture.gameObject.SetActive(true);
        }
        else selectTexture.gameObject.SetActive(false);
    }
    void UpdateDistantionToCharacter()
    {
        distantionToCharacter = Vector3.Distance(character.transform.position, enemySetup.CharacterTransform.position);
    }
    void SwithState()
    {
        switch (_enemyState)
        {
            case EnemyState.isDeath: enemySetup.Animator.Play("Death");
                break;
            case EnemyState.isIdle:
                break;
            case EnemyState.isFollowing:
                _stateEnemyMachine.ChangeState(GetStateByType<EnemyFollowState>(statesEnemy) as EnemyFollowState);
                break;
            case EnemyState.isAttack:
                _stateEnemyMachine.ChangeState(GetStateByType<EnemyAttackState>(statesEnemy) as EnemyAttackState);
                break;
            case EnemyState.isShock:
                var shock = GetStateByType<ShockWave>(statesEnemy) as ShockWave;
                _stateEnemyMachine.ChangeState(shock);
                break;
        }
    }

     void ShockWave(float speedShock, Vector3 target)
    {
        var shock = GetStateByType<ShockWave>(statesEnemy) as ShockWave;
        shock.isComplite = false;
        shock.speed = speedShock;
        shock.target = target;
        shock._transform = transform;
        _enemyState = EnemyState.isShock;
     }
    public void Damage(IEnemyTargetable attacker, float powerDamage, Vector2 hit)
    {
        
        Vector3 bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
        BloodParticleSystemHandler.Instance.SpawnBlood(hit, bloodDir);
        health-=powerDamage;
        if (IsDead())
        {
            _enemyState = EnemyState.isDeath; OnChangeState.Invoke();
            enemySetup.AIPath.canMove = false;
            AudioController.instance.PlaySFX("zombyDeath");
            Destroy(this);
            GetRandomBonus(); 
        }
    }
    void GetRandomBonus() {
        var random = UnityEngine.Random.Range(0, 100);
        if (random <= 100)
        {
            var go = Factorys.instance.FactoryBonus.GetNewInstance(); go.transform.position = transform.position;
        }
    }
    public Vector3 GetPosition()
    {
        if (bloodPoint != null)
        {
            return new Vector2(bloodPoint.position.x, bloodPoint.position.y);
        }
        return default;
    }
    public bool IsDead()
    {
        return health <= 0;
    }

    public State GetStateByType<T>(List<State> listunits)
    {
        return listunits.Find(x => x.GetType() == typeof(T));
    }
    void TargetNull() { character = null; }
    private void OnDisable()
    {
        UI_Controller.instance.OnDie -= TargetNull;
        OnChangeState -= SwithState;
    }
}
