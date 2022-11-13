using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GraveyardBehaviour : MonoBehaviour
{
    public float2 fieldDimensions;
    public int tombstonesToSpawn;
    public GameObject tombstonePrefab;
    public uint randomSeed;
    public GameObject zombiePrefab;
    public float zombieSpawnRate;
}

public class GraveyardBehaviourBaker : Baker<GraveyardBehaviour>
{
    public override void Bake(GraveyardBehaviour authoring)
    {
        AddComponent(new GraveyardProperties()
        {
            FieldDimensions = authoring.fieldDimensions,
            TombstonesToSpawn = authoring.tombstonesToSpawn,
            TombstonePrefab = GetEntity(authoring.tombstonePrefab),
            ZombiePrefab = GetEntity(authoring.zombiePrefab),
            ZombieSpawnRate = authoring.zombieSpawnRate
        });
        AddComponent(new GraveyardRandom()
        {
            Value = Random.CreateFromIndex(authoring.randomSeed)
        });
        AddComponent<ZombieSpawnPoint>();
        AddComponent<ZombieSpawnTimer>();
    }
}