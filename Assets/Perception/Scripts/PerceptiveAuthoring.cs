using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PerceptiveAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AlertComponent { AlertType = 0 });
        dstManager.AddComponentData(entity, new VisionPerceptionComponent { Angle = 90, Distance = 5f }) ;
        dstManager.AddComponentData(entity, new HearingPerceptionComponent { Distance = 5f});
        dstManager.AddComponentData(entity, new CustomColor { Value = new float4(1,0,0,1) });
    }
}
