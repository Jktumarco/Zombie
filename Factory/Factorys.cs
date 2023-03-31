using UnityEngine;
using System.Collections.Generic;
public class Factorys : MonoBehaviour
{
    public static Factorys instance;
    private void Awake()
    {
        instance = this;
    }
    [SerializeField] List<GenericFactory<Transform>> listFactory;
    [SerializeField] private FactoryExplossionGranate factoryExplossionGranate;
    [SerializeField] private FactoryTrailGranate factoryTrailGranate;
    [SerializeField] private FactoryGranate factoryGranate;
    [SerializeField] private FactoryBonus factoryBonus;
    [SerializeField] private FactoryInventaryItem factoryInventaryItem;
    [SerializeField] private FactoryParticleHealth factoryParticleHealth;

    [SerializeField] private FactorySimpleZomby factorySimpleZomby;
    [SerializeField] private FactoryMonstr factoryMonstr;
    [SerializeField] private FactoryMonstrVariation factoryMonstrVariation;
    public FactoryParticleHealth FactoryParticleHealth { get => factoryParticleHealth; }
    public FactoryInventaryItem FactoryInventaryItem { get => factoryInventaryItem;  }
    public FactoryBonus FactoryBonus { get => factoryBonus;  }
    public FactoryGranate FactoryGranate { get => factoryGranate;  }
    public FactoryTrailGranate FactoryTrailGranate { get => factoryTrailGranate; }
    public FactoryExplossionGranate FactoryExplossionGranate { get => factoryExplossionGranate;  }
    public FactorySimpleZomby FactorySimpleZomby { get => factorySimpleZomby; }
    public FactoryMonstr FactoryMonstr { get => factoryMonstr; }
    public FactoryMonstrVariation FactoryMonstrVariation { get => factoryMonstrVariation; }

    public void DestroyObj(GameObject gameobject) { Destroy(gameobject); }
    public GenericFactory<Transform> GetFactoryByType<T>(List<GenericFactory<Transform>> listunits)
    {
        return listunits.Find(x => x.GetType() == typeof(T));
    }
}