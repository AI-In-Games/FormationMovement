using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PerceptionThreatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager m_EntityManager;

    private Entity m_EntityReference;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PerceptionThreatComponent { ThreatType = (int)(ThreatType.Hearing | ThreatType.Vision) });

        m_EntityReference = entity;

        m_EntityManager = dstManager;
    }

    private void Update()
    {
        m_EntityManager.SetComponentData(m_EntityReference, new Translation { Value = transform.position });
    }
}
