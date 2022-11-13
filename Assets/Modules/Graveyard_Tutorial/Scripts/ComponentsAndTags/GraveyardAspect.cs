using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct GraveyardAspect : IAspect
{
    public readonly Entity Self;
    
    private const float BrainSafetyRadiusSq = 100;
    private const float MinRandomRotation = -0.25f;
    private const float MaxRandomRotation = 0.25f;

    private readonly RefRO<GraveyardProperties> _graveyardProperties;
    private readonly RefRW<GraveyardRandom> _graveyardRandom;
    private readonly RefRW<ZombieSpawnPoint> _zombieSpawnPoints;
    private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;
    private readonly TransformAspect _transformAspect;

    private float3 MaxCorner => _transformAspect.Position + HalfDimensions;
    private float3 MinCorner => _transformAspect.Position - HalfDimensions;

    public int TombstonesToSpawn => _graveyardProperties.ValueRO.TombstonesToSpawn;
    public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;

    public NativeArray<float3> ZombieSpawnPoints
    {
        get => _zombieSpawnPoints.ValueRO.Value;
        set => _zombieSpawnPoints.ValueRW.Value = value;
    }

    public float ZombieSpawnTimer
    {
        get => _zombieSpawnTimer.ValueRO.Value;
        set => _zombieSpawnTimer.ValueRW.Value = value;
    }

    public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;
    public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

    public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0f;

    public float3 Position => _transformAspect.Position;

    #region Tombstones

    public UniformScaleTransform GetRandomTombstoneTransform()
    {
        return new UniformScaleTransform()
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(_transformAspect.Position, randomPosition) <= BrainSafetyRadiusSq);

        return randomPosition;
    }

    private quaternion GetRandomRotation() =>
        quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(MinRandomRotation, MaxRandomRotation));

    private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min, 1f);

    private float3 HalfDimensions => new()
    {
        x = _graveyardProperties.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = _graveyardProperties.ValueRO.FieldDimensions.y * 0.5f,
    };

    #endregion

    #region Zombies

    public UniformScaleTransform GetZombieSpawnTransform()
    {
        var position = GetRandomZombiePosition();
        return new UniformScaleTransform()
        {
            Position = position,
            Rotation = quaternion.RotateY(MathExtensions.GetHeading(position, _transformAspect.Position)),
            Scale = 1f
        };
    }

    private float3 GetRandomZombiePosition()
    {
        return ZombieSpawnPoints[_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPoints.Length)];
    }
    #endregion
}