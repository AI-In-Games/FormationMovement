using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ProjectilePrefab;

    [SerializeField]
    private Transform m_SpawnTransform;

    [SerializeField]
    private float m_InitialVelocity = 20f;

    [SerializeField]
    private float m_InitialUpVelocity = 2f;

    [SerializeField]
    private int m_Width = 20;

    [SerializeField]
    private int m_Height = 20;

    [SerializeField]
    private float m_SpreadY = 60f;

    [SerializeField]
    private float m_SpreadX = 30f;

    [SerializeField]
    private float m_Lifetime = 5f;
    
    private EntityManager m_EntityManager;

    private Entity m_ProjectileEntityPrefab;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        m_EntityManager = world.EntityManager;

        m_ProjectileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_ProjectilePrefab, new GameObjectConversionSettings { DestinationWorld = World.DefaultGameObjectInjectionWorld });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Spawn();
    }

    private void Spawn()
    {
        NativeArray<Entity> projectiles = new NativeArray<Entity>(m_Width * m_Height, Allocator.TempJob);
        m_EntityManager.Instantiate(m_ProjectileEntityPrefab, projectiles);

        var spawnPosition = m_SpawnTransform.position;
        var spawnRotation = m_SpawnTransform.rotation;
        
        var rotYStart = -m_SpreadY / 2f;
        var rotXStart = -m_SpreadX / 2f;
        var rotYStep = m_SpreadY / m_Width;
        var rotXStep = m_SpreadX / m_Height;
        for (int y = 0; y < m_Width; y++)
        {
            for (int x = 0; x < m_Height; x++)
            {
                var rotation = spawnRotation * Quaternion.Euler(rotXStart + x * rotXStep, rotYStart + y * rotYStep, 0f);

                var projectile = projectiles[y + x * m_Width];
                m_EntityManager.SetComponentData(projectile, new Translation { Value = spawnPosition });
                m_EntityManager.SetComponentData(projectile, new Rotation { Value = rotation });
                m_EntityManager.AddComponentData(projectile, new Velocity { Value = math.mul(rotation, new float3(0,0,1)) * m_InitialVelocity + new float3(0, m_InitialUpVelocity,0) });
                m_EntityManager.AddComponentData(projectile, new TimeToLive { Value = m_Lifetime });
                m_EntityManager.AddComponentData(projectile, new Projectile());
                m_EntityManager.AddComponentData(projectile, new Gravity());
            }
        }
        projectiles.Dispose();
    }
}
