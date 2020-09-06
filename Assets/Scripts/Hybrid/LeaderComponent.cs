using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LeaderComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private int m_Id;
    public int ID {  get { return m_Id; } }

    private Entity m_Entity;
    private EntityManager entityManager;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.SetComponentData(entity, new Translation { Value = transform.position });
        dstManager.SetComponentData(entity, new Rotation { Value = transform.rotation });
        dstManager.AddComponentData(entity, new FormationLeader { Id = m_Id });
        dstManager.AddComponentData(entity, new SteeringAgent { TargetPosition = transform.position });
        dstManager.AddComponentData(entity, new Velocity { Value =float3.zero});
        //dstManager.AddComponent<PerceptionThreatComponent>(entity);

        entityManager = dstManager;
        m_Entity= entity;

        GetComponent<MeshRenderer>().enabled = false;
    }

    public void Move(Vector3 position)
    {
        entityManager.SetComponentData(m_Entity, new SteeringAgent { TargetPosition = position });
    }

    private void Update()
    {
        transform.position = entityManager.GetComponentData<Translation>(m_Entity).Value;
        transform.rotation = entityManager.GetComponentData<Rotation>(m_Entity).Value;
    }
}
