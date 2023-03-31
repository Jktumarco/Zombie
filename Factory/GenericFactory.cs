using UnityEngine;
using System.Collections.Generic;
public class GenericFactory<T> : MonoBehaviour where T : Transform
{
    [SerializeField] List<GenericFactory<Transform>> listFactory;
    [SerializeField] private T prefab;


    public T GetNewInstance(Transform transform)
    {
        return Instantiate(prefab, transform);
    }
    public T GetNewInstance()
    {
        return Instantiate(prefab);
    }
    public void DestroyObj(GameObject gameobject) { Destroy(gameobject); }
}