using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Update only runs if there is at least one Entity w/ this component
        state.RequireForUpdate<GraveyardProperties>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; // Disable system after initialization
        var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
        var graveyard = SystemAPI.GetAspectRW<GraveyardAspect>(graveyardEntity);

        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var spawnPoints = new NativeList<float3>(Allocator.Temp);
        var tombstoneOffset = new float3(0f, -2f, 1f);
        for (var idx = 0; idx < graveyard.TombstonesToSpawn; idx++)
        {
            var tombstoneEntity = entityCommandBuffer.Instantiate(graveyard.TombstonePrefab);
            var tombstoneTransform = graveyard.GetRandomTombstoneTransform();
            entityCommandBuffer.SetComponent(tombstoneEntity,
                new LocalToWorldTransform() { Value = tombstoneTransform });

            var zombieSpawnPoint = tombstoneTransform.Position + tombstoneOffset;
            spawnPoints.Add(zombieSpawnPoint);
        }

        graveyard.ZombieSpawnPoints = spawnPoints.ToArray(Allocator.Persistent);
        entityCommandBuffer.Playback(state.EntityManager);
    }
}