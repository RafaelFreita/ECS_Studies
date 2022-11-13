using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GraveyardProperties : IComponentData
{
    public float2 FieldDimensions;
    public int TombstonesToSpawn;
    public Entity TombstonePrefab;
    
    public Entity ZombiePrefab;
    public float ZombieSpawnRate;
}

public struct ZombieSpawnTimer : IComponentData
{
    public float Value;
}