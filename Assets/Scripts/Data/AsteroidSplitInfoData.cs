using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AsteroidSplitInfoData : IComponentData
{
    public Entity prefab;
}
