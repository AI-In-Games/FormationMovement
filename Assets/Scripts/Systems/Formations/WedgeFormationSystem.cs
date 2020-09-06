using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(AlignWithLeader))]
public class WedgeFormationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var distance = GameManager.Instance.AgentDistance;

        Entities
            .WithAll<WedgeFormation>()
           .ForEach((ref SteeringAgent agent, in FormationIndex formationIndex) =>
           {
               var index = formationIndex.Index + 2;
               var z = index / 2 * distance;
               var x = -z + 2 * z * (index % 2);
               var pos = new float3(x, 0, -z);
               agent.TargetPosition = pos;
           }).ScheduleParallel();
    }
}
