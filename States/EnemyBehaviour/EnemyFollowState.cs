using UnityEngine;
using Pathfinding;

public class EnemyFollowState : State
{
    Animator animator;
    AIPath aiPath;
    EnemyVision enemyVision;
    Character targetFollow;
    Transform enemyPos;
    float damage = 10f;
    public EnemyFollowState(CharacterSetup characterSetup, Character target)
    { 
        this.animator = characterSetup.Animator; 
        this.aiPath = characterSetup.AIPath;
        this.enemyVision = characterSetup.EnemyVision;
        this.targetFollow = target;
        this.enemyPos = characterSetup.CharacterTransform;
        
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Enter()
    {
        animator.Play("Walk");
        
        if (targetFollow != null)
        {
            aiPath.target.position = targetFollow.transform.position;
        }
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
