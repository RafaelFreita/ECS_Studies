using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SpawnZombieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

        new SpawnZombieJob()
        {
            DeltaTime = deltaTime,
            Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }
}

[BurstCompile]
public partial struct SpawnZombieJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    private void Execute(GraveyardAspect graveyard)
    {
        graveyard.ZombieSpawnTimer -= DeltaTime;
        if (!graveyard.TimeToSpawnZombie) return;
        if (graveyard.ZombieSpawnPoints.Length == 0) return;

        graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
        var newZombie = Ecb.Instantiate(graveyard.ZombiePrefab);
        var newZombieTransform = graveyard.GetZombieSpawnTransform();
        Ecb.SetComponent(newZombie, new LocalToWorldTransform()
        {
            Value = newZombieTransform
        });

        var zombieHeading = MathExtensions.GetHeading(newZombieTransform.Position, graveyard.Position);
        Ecb.SetComponent(newZombie, new ZombieHeading() { Value = zombieHeading });
    }
}