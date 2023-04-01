using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Pathfinding;

public class ThrowGranata : State
{
    private Transform _start, _center, _end;
    private int _countLineObj = 15;
    private List<Vector3> listPositions = new List<Vector3>();
    private Queue<Vector3> QueuePos = new Queue<Vector3>();
    private LineRenderer lineRenderer;
    private List<GameObject> prefabTrillLst;
    private float smooth = 10f;
    private float limit = 26f;

    Vector3 mouseFinalPos;
    private Vector3 offset;
    private Vector3 TargetPoint;
    private GameObject prefGranate;
   
    private bool canThrow = false;
    private Transform transform;
    private AILerp aILerp;
    private bool canSecondThow = true;
    private Character character;
    public ThrowGranata(CharacterSetup character_Setup, LineRenderer line_Renderer) {
        this.lineRenderer = line_Renderer;
        this.aILerp = character_Setup.AiLerp;
        this.transform = character_Setup.CharacterTransform;
        this.character = character_Setup.Character;
        _start = transform;
        _center = transform.Find( "CentrTraectory");
        prefabTrillLst = new List<GameObject>();
        prefGranate = Factorys.instance.FactoryGranate.GetNewInstance().gameObject;
        granataPointStart = transform.Find("Granate").transform;
      
    }
   
    public override void Enter()
    {
        aILerp.enableRotation = false;
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    [SerializeField] Transform granataPointStart;
    public override void Update()
    {
        if (character.Inventary.CheckingBullet("cylinder"))
        {
            base.Update();
            if (canThrow)
            {
                if (QueuePos.Count >= 0)
                {
                    prefGranate.transform.position = Vector3.Lerp(granataPointStart.position, TargetPoint, 1f);
                    prefGranate.transform.Rotate(new Vector3(0, 0, 1));
                    ThrowGranate(prefGranate.transform);
                }
            }


            if (!InputsController.instance.IsMouseOverUIWithIgnores() && canSecondThow)
            {
                if (Input.GetMouseButtonDown(0)) { Factorys.instance.FactoryExplossionGranate.GetNewInstance(transform); }

                if (Input.GetMouseButtonUp(0))
                {
                    if (prefabTrillLst.Count > 0)
                    {
                        foreach (var objLineRend in prefabTrillLst)
                        {
                            Factorys.instance.DestroyObj(objLineRend.gameObject);
                        }
                        prefabTrillLst.Clear();
                        foreach (var item in listPositions)
                        {
                            QueuePos.Enqueue(item);
                        }
                        AudioController.instance.PlaySFX("granateThrow",2f);
                        prefGranate.gameObject.SetActive(true);
                        canSecondThow = false;
                        canThrow = true;
                    }
                   
                }

                if (Input.GetMouseButton(0))
                {
                    LookAtMouse();
                    if (prefabTrillLst.Count == 0)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            var go = Factorys.instance.FactoryTrailGranate.GetNewInstance(transform);
                            prefabTrillLst.Add(go.gameObject);
                        }
                    }
                    var mousPos = UtilsClass.GetMouseWorldPosition();
                    float dist = Vector3.Distance(transform.position, UtilsClass.GetMouseWorldPosition());
                    var direction = (mousPos - transform.position).normalized;
                    var targetPos = transform.position + direction * limit;
                    if (mouseFinalPos != null)
                    {
                        if (mouseFinalPos != mousPos && dist < limit) { EvaluateSlerpPoints(_start.position, mousPos, _center.position, _countLineObj); }
                        if (mouseFinalPos != mousPos && dist > limit) { EvaluateSlerpPoints(_start.position, targetPos, _center.position, _countLineObj); }
                        else return;
                    }
                    else EvaluateSlerpPoints(_start.position, mousPos, _center.position, _countLineObj);
                }
            }
        }
    }
    void EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center, int count = 10)
    {
        listPositions.Clear();
        var startRelativeCenter = (start + offset) - center;
        var endRelativeCenter = end - center;

        var f = 1f / count;
        
        for (var i = 0f; i < 1 + f; i += f)
        {
            Vector3 pointTraectory = Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + center;
            listPositions.Add(pointTraectory);   
        } 
            for (int i = 0; i < listPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, listPositions[i]);
                if (prefabTrillLst != null) { prefabTrillLst[i].transform.position = lineRenderer.GetPosition(i); }
            }
    }
    void LookAtMouse()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 characterDir = (mousePosition - transform.position);
        float angle = Mathf.Atan2(characterDir.y, characterDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smooth * Time.deltaTime);
    }
    void ThrowGranate(Transform transform)
    {
        if(QueuePos.Count > 0)
        {
            TargetPoint = QueuePos.Dequeue();
        }
        else if(TargetPoint == transform.position)
        {
            if (QueuePos.Count > 0)
            {
                TargetPoint = QueuePos.Dequeue();
            }
            else FunctionTimer.Create(() => {
                var exs = Factorys.instance.FactoryExplossionGranate.GetNewInstance();
                exs.transform.position = TargetPoint; exs.gameObject.SetActive(true); AudioController.instance.PlaySFX("granata");
                FunctionTimer.Create(() => Factorys.instance.DestroyObj(exs.gameObject), 3f); prefGranate.gameObject.SetActive(false); 
                TargetPoint = granataPointStart.position; QueuePos.Clear(); prefGranate.transform.position = granataPointStart.position;  canSecondThow = true;
            }, 1f);
            canThrow = false;
            Character.OnShoot.Invoke();
        }
    }
}