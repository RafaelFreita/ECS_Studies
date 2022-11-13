using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering.Universal;

[BurstCompile]
[UpdateAfter(typeof(ZombieWalkSystem))]
public partial struct ZombieEatSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
        var brainScale = SystemAPI.GetComponent<LocalToWorldTransform>(brainEntity).Value.Scale;
        var brainRadius = brainScale * 5f + 1f;
        new ZombieEatJob()
        {
            DeltaTime = deltaTime,
            Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            BrainEntity = brainEntity,
            BrainRadiusSq = brainRadius * brainRadius
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieEatJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public Entity BrainEntity;
    public float BrainRadiusSq;

    private void Execute(ZombieEatAspect zombie, [EntityInQueryIndex] int sortKey)
    {
        if (zombie.IsInEatingRange(float3.zero, BrainRadiusSq))
        {
            zombie.Eat(DeltaTime, Ecb, sortKey, BrainEntity);
        }
        else
        {
            Ecb.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
            Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
        }
    }
}