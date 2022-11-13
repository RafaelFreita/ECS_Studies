using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ZombieSpawnPoint : IComponentData
{
    public NativeArray<float3> Value;
}