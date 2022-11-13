using System;
using System.Security.Cryptography;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance;

    [SerializeField] private float startRadius;
    [SerializeField] private float endRadius;
    [SerializeField] private float startHeight;
    [SerializeField] private float endHeight;
    [SerializeField] private float speed;

    public float RadiusAtScale(float scale) => Mathf.Lerp(startRadius, endRadius, 1 - scale);
    public float HeightAtScale(float scale) => Mathf.Lerp(startHeight, endHeight, 1 - scale);
    public float Speed => speed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}