using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("DOTS/SpawnAsteroids/SpawnerMono")]
public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int countX;
    public int countY;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(true)
        {
            var _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            var _prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, _settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var instance = entityManager.Instantiate(_prefab);
            var position = Vector3.zero;

            entityManager.SetComponentData(instance, new Translation { Value = position });

            yield return new WaitForSeconds(2f);
        }
    }
}
