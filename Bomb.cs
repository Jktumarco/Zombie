using UnityEngine;
using CodeMonkey.Utils;
public class Bomb : MonoBehaviour
{
    [SerializeField] float _power;
    [SerializeField] float _speedShockWave;
    [SerializeField] Transform Transform;
    [SerializeField] GameObject GameObject;

    private void Start()
    {
        CinemachineShake.Instance.ShakeCamera(27f, .4f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Enemy enemyStriker = collision.GetComponent<Enemy>();

        if (enemyStriker != null)
        {
            var dir = enemyStriker.transform.position - transform.position;
            var distVessionNew = Vector3.Distance(transform.position, enemyStriker.GetPosition());
            
            var endPos = enemyStriker.transform.position + (dir.normalized * (_power / distVessionNew));
            enemyStriker.Damage(_speedShockWave, endPos);
            //FunctionTimer.Create(() => gameObject.SetActive(false),3f,"Bomb",false,true);
        }
        //if (enemyStriker == null)
        //{
        //    FunctionTimer.Create(() => gameObject.SetActive(false), 3f, "Bombm", false, true);
        //}

    }
}
