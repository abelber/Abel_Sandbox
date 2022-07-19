using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerData : IComponentData
{
    public int countX;
    public int countY;
    public Entity prefab;
}
