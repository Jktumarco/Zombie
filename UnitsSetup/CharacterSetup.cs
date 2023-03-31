using UnityEngine;
using Pathfinding;

public class CharacterSetup: MonoBehaviour
{
    [SerializeField] public Character character;
    [SerializeField] public Transform bloodPoint;
    [SerializeField] public AILerp aiLerp;
    [SerializeField] public AIPath aIPath;
    [SerializeField] public Transform wayTargetPoint;
    [SerializeField] public Transform characterTransform;
    [SerializeField] public Animator animator;
    [SerializeField] public EnemyVision enemyVision;
    [SerializeField] public GameObject selectedGameObject;
}
