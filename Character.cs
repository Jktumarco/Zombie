using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
public enum characterState { goToPoinOrStrike, automaticStrike, goToTable, isShock, granate, riffle, knife }

public class Character : MonoBehaviour, Enemy.IEnemyTargetable
{
    public static string curWeapon;
    public static Action OnShoot;
    [SerializeField] float health;
    [SerializeField] FunctionUpdater updater;
    [SerializeField] GameObject prefabGranata;
    public Item item;
    public Inventary inventary;
    [SerializeField] CharacterSetup characterSetup;
    [SerializeField] List<State> states = new List<State>();
    [SerializeField] List<UnitSetup> unitSetupList = new List<UnitSetup>();
    public characterState _characterState = new characterState();
    public bool IsSelection;
    StateMachine _stateMachine;
    [SerializeField] LineRenderer linerenderer;
    [SerializeField] float power;
    [SerializeField] bool bach = false;
    Rigidbody2D rigidbody2D;
    Character targetForEnemy;

    public Character TargetForEnemy { get => targetForEnemy; }

    private void Awake()
    {
        characterSetup = GetComponent<CharacterSetup>();
        linerenderer = GetComponent<LineRenderer>();
        targetForEnemy = this;
        states.Add(new GoToPointOrStrikeGun(characterSetup, GetUnitSetupByType<GunSetup>(unitSetupList) as GunSetup));
        states.Add(new Riffle(characterSetup, GetUnitSetupByType<RiffleSetup>(unitSetupList) as RiffleSetup));
        states.Add(new AutomaticStrikeGun(characterSetup, GetUnitSetupByType<GunSetup>(unitSetupList) as GunSetup));
        states.Add(new Bat(characterSetup, GetUnitSetupByType<BatSetup>(unitSetupList) as BatSetup));
        states.Add(new ShockWave(UtilsClass.GetMouseWorldPosition(), transform, 3f));
        _stateMachine = new StateMachine();
        _stateMachine.Initialize(GetStateByType<AutomaticStrikeGun>(states) as AutomaticStrikeGun);
        _characterState = characterState.goToPoinOrStrike;
        rigidbody2D = GetComponent<Rigidbody2D>();
        StateUpdate();
    }

    private void Start()
    {
        UI_Controller.instance.OnHealth += Healthing;
        UI_Controller.instance.OnStart += DefaultCharacter;
        states.Add(new ThrowGranata(characterSetup, linerenderer));
    }
    private void Update()
    {
        var target = UtilsClass.GetMouseWorldPosition();
        
        _stateMachine.CurrentState.Update();  
    }
    public void OnChangeState(string state) {
        Debug.Log(state);
        switch (state)
        {
            case "gun": _characterState = characterState.goToPoinOrStrike;
                break;
            case "cylinder": _characterState = characterState.granate;
                break;
            case "riffle":
                _characterState = characterState.riffle;
                break;
            case "bat":
                _characterState = characterState.knife;
                break;
        }
        StateUpdate();
    }
    void StateUpdate()
    {
        switch (_characterState)
        {
            case characterState.automaticStrike:
                _stateMachine.ChangeState(GetStateByType<AutomaticStrikeGun>(states) as AutomaticStrikeGun);
                curWeapon = "gun";
                break;
            case characterState.goToPoinOrStrike:
                _stateMachine.ChangeState(GetStateByType<GoToPointOrStrikeGun>(states) as GoToPointOrStrikeGun);
                curWeapon = "gun";
                break;
            case characterState.knife:
                _stateMachine.ChangeState(GetStateByType<Bat>(states) as Bat);
                curWeapon = "knife";
                break;
            case characterState.granate:
                _stateMachine.ChangeState(GetStateByType<ThrowGranata>(states) as ThrowGranata);
                curWeapon = "cylinder";
                break;
            case characterState.riffle:
                _stateMachine.ChangeState(GetStateByType<Riffle>(states) as Riffle);
                curWeapon = "riffle";
                break;
        }
    }
    public UnitSetup GetUnitSetupByType<T>( List<UnitSetup> listunits)
    {
        return listunits.Find(x => x.GetType() == typeof(T) );
    }

    public State GetStateByType<T>(List<State> listunits)
    {
        return listunits.Find(x => x.GetType() == typeof(T));
    }

    public void SetSelectedVisible(bool visible) { characterSetup.selectedGameObject.SetActive(visible); }
   
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Damage(Enemy attacker, Vector2 hit)
    {

    }
    public void Damage(float speedShock, Vector3 target)
    {
        //ShockWave(speedShock, target);
    }
    public void Damage(Transform enemyPos, float damage)
    {
        if (IsDead())
        {
            characterSetup.animator.Play("Death");
            AudioController.instance.PlaySFX("characterDeath");
            UI_Controller.instance.OnDie.Invoke();
        }
        Vector3 bloodDir = (enemyPos.position - GetPosition()).normalized;
        BloodParticleSystemHandler.Instance.SpawnBlood(transform.position, bloodDir);
        health -= damage;
        UI_Controller.instance.OnDamage.Invoke(damage);
        
    }
    public bool IsDead()
    {
        return health <=0f;
    }
    void ShockWave(float speedShock, Vector3 target)
    {
        var shock = GetStateByType<ShockWave>(states) as ShockWave;
        shock.isComplite = false;
        shock.speed = speedShock;
        shock.target = target;
        shock._transform = transform;
        _characterState = characterState.isShock;
    }
    public void TakeItem() {

        _characterState = characterState.goToTable;
        StateUpdate();
    }
    void ChangeTargetPointPos(float speed)
    {
        var pos = UtilsClass.GetMouseWorldPosition();
        characterSetup.animator.Play("WalkGun");
        var go = GameObject.Find("Cursor");
        go.transform.position = pos;
        go.GetComponentInChildren<Animator>().Play("Cursor");
        characterSetup.aiLerp.speed = speed;
        characterSetup.aiLerp.target.position = pos;
        characterSetup.aiLerp.canMove = true;
        characterSetup.aiLerp.enableRotation = true;
    }
    public void Healthing(float health) {
        var point = transform.Find("PointParticleHealth");
        var go = Factorys.instance.FactoryParticleHealth.GetNewInstance();
        go.transform.position = point.position;
        go.gameObject.SetActive(true);
        if(this.health <= 100f) { this.health += health; }
        if(this.health > 100f) { this.health = 100f; }
        
    }
    public void DefaultCharacter() 
    {
        health = 100f; characterSetup.animator.Play("IdleGun"); _characterState = characterState.goToPoinOrStrike; StateUpdate();
    }
    private void OnDisable()
    {
        UI_Controller.instance.OnStart -= DefaultCharacter;
    }

}
