//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;

//public class PerceptionSystem : SystemBase
//{
//    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

//    private EntityQuery m_ThreadQuery;

//    protected override void OnCreate()
//    {
//        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

//        m_ThreadQuery = GetEntityQuery(ComponentType.ReadOnly<PerceptionThreatComponent>(), ComponentType.ReadOnly<Translation>());
//    }

//    protected override void OnUpdate()
//    {
//        var threats = m_ThreadQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
//        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

//        Entities
//            .ForEach((Entity entity, int entityInQueryIndex, in AlertComponent alertComponent, in Translation translation, in Rotation rotation) =>
//            {
//                ThreatType threatType = 0;
//                for (int i = 0; i < threats.Length; i++)
//                {
//                    var threat = threats[i];
//                    var offset = threat.Value - translation.Value;
//                    if (HasComponent<VisionPerceptionComponent>(entity))
//                    {
//                        var vision = GetComponent<VisionPerceptionComponent>(entity);

//                        var agentForward = math.mul(rotation.Value, new float3(0, 0, 1));
//                        var visionAngle = math.cos(math.radians(vision.Angle));

//                        var offsetNorm = math.normalizesafe(offset);
//                        var angle = math.dot(agentForward, offsetNorm);
//                        if (angle < visionAngle && math.lengthsq(offset) < vision.Distance * vision.Distance)
//                            threatType |= ThreatType.Vision;
//                    }
//                    if (HasComponent<HearingPerceptionComponent>(entity))
//                    {
//                        var hearingDistance = GetComponent<HearingPerceptionComponent>(entity).Distance;

//                        if (math.lengthsq(offset) < hearingDistance * hearingDistance)
//                            threatType |= ThreatType.Hearing;
//                    }
//                }

//                ecb.SetComponent(entityInQueryIndex, entity, new AlertComponent { AlertType = (int)threatType });
//            })
//            .WithDeallocateOnJobCompletion(threats)
//            .ScheduleParallel();

//        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

//        float4 alertColor = new float4(0, 1, 0, 1);
//        float4 defaultColor = new float4(1, 0, 0, 1);
//        Entities
//           .ForEach((Entity entity, ref CustomColor color, in AlertComponent alert) =>
//           {
//               color.Value = alert.AlertType > 0 ? alertColor : defaultColor;
//           }).Schedule();
//    }
//}
