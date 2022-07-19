using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("DOTS/SpawnAsteroids/Spawner")]
public class AsteroidsSpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab;
    public int countX;
    public int countY;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem gameObjectConversionSystem)
    {
        var spawnerData = new SpawnerData
        {
            prefab = gameObjectConversionSystem.GetPrimaryEntity(prefab),
            countX = countX,
            countY = countY
        };

        entityManager.AddComponentData(entity, spawnerData);
    }
}
