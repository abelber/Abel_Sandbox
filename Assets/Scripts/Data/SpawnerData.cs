using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnerData : IComponentData
{
    public Entity prefab;
}
