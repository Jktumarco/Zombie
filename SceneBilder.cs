using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class SceneBilder : MonoBehaviour
{
    [SerializeField] GameObject prefBlood;
    [SerializeField] List<Transform> pointsEnemysCreateList = new List<Transform>();
    [SerializeField] List<Transform> wayPointsEnemysList = new List<Transform>();
    [SerializeField] List<Transform> enemys = new List<Transform>();
    private GameObject bloodSceneObject;
    private void OnEnable()
    {
        MakeBloodObjectInScene();
    }
    private void Start()
    {
        LoadScene();
        UI_Controller.instance.OnInventaryUpdate.Invoke();
    }
    public void LoadScene()
    {
        CreateEnemys();
    }
    void CreateEnemys()
    {
        for (int i = 0; i < wayPointsEnemysList.Count; i++)
        {
            if (i < 8) { CreateEnemy(Factorys.instance.FactoryMonstrVariation.GetNewInstance(),i); continue; }
            if (i > 8 && i < 16) { CreateEnemy(Factorys.instance.FactoryMonstr.GetNewInstance(), i); continue; }
            else CreateEnemy(Factorys.instance.FactorySimpleZomby.GetNewInstance(), i); continue;   
        }
    }
    void CreateEnemy(Transform enemy, int wayPoint) {
        enemy.position = pointsEnemysCreateList[wayPoint].position;
        enemy.GetComponent<AIDestinationSetter>().target = wayPointsEnemysList[wayPoint];
        enemys.Add(enemy);
    }
    void MakeBloodObjectInScene()
    {
        bloodSceneObject = Instantiate(prefBlood);
        bloodSceneObject.name = "BloodParticleSystemHandler";
    }
    void UpdateBlood() {
        Destroy(bloodSceneObject);
        MakeBloodObjectInScene();
    }
    void DestroyAllEnemy()
    {
        if(enemys != null) {
            foreach (var item in enemys)
            {
                Destroy(item.gameObject);            
            }
            enemys.Clear();
        }
    }
    GameObject CreateWayPoint() { GameObject wayPoint = new GameObject("wayPoint"); return wayPoint; }

}
