using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections.Generic;

[UpdateBefore(typeof(SteeringSystem))]
public class AlignWithLeader : SystemBase
{
    private EntityQuery m_LeaderQuery;

    protected override void OnCreate()
    {
        m_LeaderQuery = this.GetEntityQuery
           (
           ComponentType.ReadOnly<Translation>(),
           ComponentType.ReadOnly<Rotation>(),
           ComponentType.ReadOnly<FormationLeader>()
           );
    }

    protected override void OnUpdate()
    {
        var leaders = m_LeaderQuery.ToComponentDataArray<FormationLeader>(Allocator.TempJob);
        var leaderPositions = m_LeaderQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var leaderRotations = m_LeaderQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);

        List<FormationGroup> formationGroups = new List<FormationGroup>();
        EntityManager.GetAllUniqueSharedComponentData(formationGroups);

        foreach (var formationGroup in formationGroups)
        {
            if (formationGroup.LeaderId < 0)
                continue;
            float3 leaderPos = float3.zero;
            quaternion leaderRot = quaternion.identity;
            for (int i = 0; i < leaders.Length; i++)
            {
                if (leaders[i].Id == formationGroup.LeaderId)
                {
                    leaderPos = leaderPositions[i].Value;
                    leaderRot = leaderRotations[i].Value;
                }
            }
            Entities
                .WithSharedComponentFilter(formationGroup)
                .ForEach((ref SteeringAgent steeringAgent) =>
                {
                    var targetPosition = math.mul(leaderRot, steeringAgent.TargetPosition);
                    steeringAgent.TargetPosition = targetPosition + leaderPos;
                }).ScheduleParallel();
        }
        leaders.Dispose();
        leaderPositions.Dispose();
        leaderRotations.Dispose();
    }
}
