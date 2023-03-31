using UnityEngine;
using Pathfinding;
using CodeMonkey.Utils;
public class EnemyAttackState : State
{
    Animator animator;
    AIPath aiPath;
    EnemyVision enemyVision;
    Transform target;
    Transform enemyTransform;
    Character character;
    float damage = 10;
    float _timeBetweenAttack = 1f;
    public EnemyAttackState(CharacterSetup characterSetup, Transform target, Character character)
    { 
        this.animator = characterSetup.animator; 
        this.aiPath = characterSetup.aIPath;
        this.enemyVision = characterSetup.enemyVision;
        this.target = target;
        this.enemyTransform = characterSetup.characterTransform;
        this.character = character;
    }
    public override void Update()
    {
        if (_timeBetweenAttack > 0)
        {
            _timeBetweenAttack -= Time.deltaTime;
        }
        if (_timeBetweenAttack <= 0)
        {
            if (character != null)
            {
                AudioController.instance.PlaySFX("zombeAttack");
                character.Damage(enemyTransform, damage); 
                _timeBetweenAttack = 1f;
            }
        }
        aiPath.target.position = enemyTransform.position;
        LookAtMouse(target.position);
        base.Update();
    }
    public override void Enter()
    { 
        animator.Play("Attack");
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }

   void LookAtMouse(Vector3 hitPos)
    {
        Vector3 characterDir = (hitPos - enemyTransform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, rotation, 50 / 10 * Time.deltaTime);
    }

}
