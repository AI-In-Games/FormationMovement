using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class SoldierFormation : MonoBehaviour
{
    [Header("Leader")]
    [SerializeField]
    private FormationType m_FormationType = FormationType.Testudo;

    [SerializeField]
    private LeaderComponent m_Leader;

    public int m_NumberOfSoldiers = 1000;

    public GameObject m_SoldierPrefab;
        
    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        GameObjectConversionSettings settings = new GameObjectConversionSettings(world, 0);
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_SoldierPrefab, settings);

        entityManager.AddComponentData(entity, new Health { Value = 1 });
        entityManager.AddComponentData(entity, new Velocity { Value = 0 });
        entityManager.AddComponentData(entity, new SteeringAgent { TargetPosition = float3.zero });
        entityManager.AddComponentData(entity, new CustomColor {  Value = new float4(1,0,0,1)});
        entityManager.AddSharedComponentData(entity, new FormationGroup { LeaderId = m_Leader.ID });
        entityManager.AddComponent(entity, GetComponentTypeFrom(m_FormationType));

        NativeArray<Entity> entities = new NativeArray<Entity>(m_NumberOfSoldiers, Allocator.Temp);
        entityManager.Instantiate(entity, entities);

        var translation = new Translation { Value = transform.position };
        for(int i = 0; i < m_NumberOfSoldiers; i++)
        {
            entityManager.SetComponentData(entities[i], translation);
            entityManager.AddComponentData(entities[i], new FormationIndex { Index = i, Count = m_NumberOfSoldiers });
        }
        entities.Dispose();

        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
    }

    private ComponentType GetComponentTypeFrom(FormationType formationType)
    {
        ComponentType componentType = null;
        switch (formationType)
        {
            case FormationType.Testudo:
                componentType = new ComponentType(typeof(TestudoFormation));
                break;
            case FormationType.Orb:
                componentType = new ComponentType(typeof(OrbFormation));
                break;
            case FormationType.Wedge:
                componentType = new ComponentType(typeof(WedgeFormation));
                break;
        }

        return componentType;
    }
}
