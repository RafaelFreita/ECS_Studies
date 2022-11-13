﻿using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(ZombieRiseSystem))]
public partial struct ZombieWalkSystem : ISystem
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
        var brainRadius = brainScale * 5f + 0.5f;

        new ZombieWalkJob()
        {
            DeltaTime = deltaTime,
            BrainRadiusSq = brainRadius * brainRadius,
            Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieWalkJob : IJobEntity
{
    public float DeltaTime;
    public float BrainRadiusSq;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    private void Execute(ZombieWalkAspect zombie, [EntityInQueryIndex] int sortKey)
    {
        zombie.Walk(DeltaTime);
        if (!zombie.IsInStoppingRange(float3.zero, BrainRadiusSq)) return;
        Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, false);
        Ecb.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
    }
}