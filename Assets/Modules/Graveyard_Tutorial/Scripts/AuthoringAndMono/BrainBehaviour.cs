using Unity.Entities;
using UnityEngine;

public class BrainBehaviour : MonoBehaviour
{
    public float BrainHealth;
}

public class BrainBaker : Baker<BrainBehaviour>
{
    public override void Bake(BrainBehaviour authoring)
    {
        AddComponent<BrainTag>();
        AddComponent(new BrainHealth()
        {
            Value = authoring.BrainHealth,
            Max = authoring.BrainHealth,
        });
        AddBuffer<BrainDamageBufferElement>();
    }
}