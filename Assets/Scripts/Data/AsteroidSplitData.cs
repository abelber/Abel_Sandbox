using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AsteroidSplitData : IComponentData
{
    public bool split;
}
