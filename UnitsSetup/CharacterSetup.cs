using UnityEngine;
using Pathfinding;

public class CharacterSetup: MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private Transform bloodPoint;
    [SerializeField] private AILerp aiLerp;
    [SerializeField] private AIPath aIPath;
    [SerializeField] private Transform wayTargetPoint;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private GameObject selectedGameObject;

    public Character Character { get => character; set => character = value; }
    public Transform BloodPoint { get => bloodPoint; set => bloodPoint = value; }
    public AILerp AiLerp { get => aiLerp; set => aiLerp = value; }
    public AIPath AIPath { get => aIPath; set => aIPath = value; }
    public Transform WayTargetPoint { get => wayTargetPoint; set => wayTargetPoint = value; }
    public Transform CharacterTransform { get => characterTransform; set => characterTransform = value; }
    public Animator Animator { get => animator; set => animator = value; }
    public EnemyVision EnemyVision { get => enemyVision; set => enemyVision = value; }
    public GameObject SelectedGameObject { get => selectedGameObject; set => selectedGameObject = value; }
}
