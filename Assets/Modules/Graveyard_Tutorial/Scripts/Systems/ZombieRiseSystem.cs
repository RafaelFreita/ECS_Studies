using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(SpawnZombieSystem))]
public partial struct ZombieRiseSystem : ISystem
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
        new ZombieRiseJob()
        {
            DeltaTime = deltaTime,
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieRiseJob : IJobEntity
{
    public float DeltaTime;
    // ReSharper disable once InconsistentNaming
    public EntityCommandBuffer.ParallelWriter ECB;

    [BurstCompile]
    private void Execute(ZombieRiseAspect zombieRise, [EntityInQueryIndex] int sortKey)
    {
        zombieRise.Rise(DeltaTime);
        if (!zombieRise.IsAboveGround) return;
        zombieRise.SetAtGroundLevel();
        ECB.RemoveComponent<ZombieRiseRate>(sortKey, zombieRise.Entity);
        ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieRise.Entity, true);
    }
}