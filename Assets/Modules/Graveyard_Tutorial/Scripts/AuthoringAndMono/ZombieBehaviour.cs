using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieBehaviour : MonoBehaviour
{
    public float RiseRate;
    public float WalkSpeed;
    public float WalkAmplitude;
    public float WalkFrequency;
    public float EatDamagePerSecond;
    public float EatAmplitude;
    public float EatFrequency;
}

public class ZombieBaker : Baker<ZombieBehaviour>
{
    public override void Bake(ZombieBehaviour authoring)
    {
        AddComponent(new ZombieRiseRate()
        {
            Value = authoring.RiseRate
        });
        AddComponent(new ZombieWalkProperties()
        {
            WalkSpeed = authoring.WalkSpeed,
            WalkAmplitude = authoring.WalkAmplitude,
            WalkFrequency = authoring.WalkFrequency,
        });
        AddComponent(new ZombieEatProperties()
        {
            EatDamagePerSecond = authoring.EatDamagePerSecond,
            EatAmplitude = authoring.EatAmplitude,
            EatFrequency = authoring.EatFrequency,
        });
        AddComponent<ZombieTimer>();
        AddComponent<ZombieHeading>();
        AddComponent<NewZombieTag>();
    }
}