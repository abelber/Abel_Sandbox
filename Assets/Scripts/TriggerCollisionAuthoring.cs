using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TriggerCollisionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float GravityFactor = 0;
    public float DampingFactor = 0.9f;

    void OnEnable() { }

    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            dstManager.AddComponentData(entity, new TriggerCollisionData()
            {
                GravityFactor = GravityFactor,
                DampingFactor = DampingFactor
            });
        }
    }
}
