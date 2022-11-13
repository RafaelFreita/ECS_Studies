using Unity.Mathematics;
using UnityEngine;

public static class MathExtensions
{
    public static float GetHeading(float3 objectPosition, float3 targetPosition)
    {
        var x = objectPosition.x - targetPosition.x;
        var z = objectPosition.z - targetPosition.z;
        return math.atan2(x, z) + math.PI;
    }
}