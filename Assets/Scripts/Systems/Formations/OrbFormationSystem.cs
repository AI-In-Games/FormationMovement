using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(AlignWithLeader))]
public class OrbFormationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float distance = GameManager.Instance.AgentDistance;
        const float twoPi = 2 * math.PI;

        Entities
            .WithAll<OrbFormation>()
            .ForEach((ref SteeringAgent agent, in FormationIndex formationIndex) =>
        {
            var index = formationIndex.Index;
            var entityCount = formationIndex.Count;
            var radius = distance * entityCount / twoPi;
            var radians = twoPi * index / entityCount;
            var x = math.sin(radians) * radius;
            var z = math.cos(radians) * radius;
            agent.TargetPosition = new float3(x, 0, z);
        }).ScheduleParallel();
    }
}
