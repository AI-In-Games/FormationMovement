using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(AlignWithLeader))]
public class TestudoFormationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float distance = GameManager.Instance.AgentDistance;
        int width = GameManager.Instance.TestudoWidth;

        Entities
            .WithAll<TestudoFormation>()
           .ForEach((ref SteeringAgent agent, in FormationPlacement formationIndex) =>
           {
               var index = formationIndex.Index;
               float midX = (width - 1) / 2f;
               var x = index % width - midX;
               var z = index / width + 1;
               agent.TargetPosition = new float3(x * distance, 0, -z * distance); 
           }).ScheduleParallel();
    }
}
