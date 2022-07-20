using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnerPowerUpData : IComponentData
{
    public Entity prefab1;
    public Entity prefab2;
    public Entity prefab3;
}
