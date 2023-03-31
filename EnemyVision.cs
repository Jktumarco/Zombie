using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyVision : MonoBehaviour
{

   
    [SerializeField] private int _rays = 8;
    [SerializeField] private int _distance = 33;
    [SerializeField] private float _angle = 40;
    [SerializeField] private Vector3 _offset;
    [SerializeField] Transform _target;


    [SerializeField] Vector3 dirTemp;

     RaycastHit2D[] GetRaycast(Vector3 dir)
    {
        Vector3 pos = transform.position + _offset;
        RaycastHit2D[] raycastHit = Physics2D.RaycastAll(pos, dir, _distance) ;
       
        if (raycastHit != null)
        {
            
            Debug.DrawRay(pos, dir * _distance, Color.red);
            return raycastHit;
        }
        else
        {
            Debug.DrawRay(pos, dir * _distance, Color.red);
        }
        return raycastHit;
    }

    public List<RaycastHit2D> RayToScan()
    {
        List<RaycastHit2D> result = new List<RaycastHit2D>();
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < _rays; i++)
        {
            dirTemp.x = Mathf.Sin(j);
            dirTemp.y = Mathf.Cos(j);

            j += _angle * Mathf.Rad2Deg / _rays; 

            Vector3 dir = transform.TransformDirection(new Vector3(dirTemp.x, dirTemp.y, 0));
            if (GetRaycast(dir) !=null) { ConvertArray(GetRaycast(dir), result); }

            if (dirTemp.x != 0)
            {
                dir = transform.TransformDirection(new Vector3(-dirTemp.x, dirTemp.y, 0));
                if (GetRaycast(dir) != null) ConvertArray(GetRaycast(dir), result);
            }
        }

        
        return result;
    }

    void ConvertArray(RaycastHit2D[] raycastHit, List<RaycastHit2D> result)
    {
        for (int i = 0; i < raycastHit.Length; i++)
        {
            if (raycastHit[i].collider.gameObject.layer == 11) {  break; }
            else result.Add(raycastHit[i]);
        }
    }
}