using Unity.Entities;
using Unity.Jobs;
using System.Collections.Generic;

public class ChangeFormationSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var newFormationType = GameManager.Instance.SelectedFormationType;
        if (newFormationType == FormationType.None)
            return;
        var selectedLeaders = GameManager.Instance.SelectedLeaders;
        if (selectedLeaders.Count == 0)
            return;

        ComponentType componentToAdd = null;
        switch(newFormationType)
        {
            case FormationType.Testudo:
                componentToAdd = new ComponentType(typeof(TestudoFormation));
                break;
            case FormationType.Orb:
                componentToAdd = new ComponentType(typeof(OrbFormation));
                break;
            case FormationType.Wedge:
                componentToAdd = new ComponentType(typeof(WedgeFormation));
                break;
        }
        
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        List<FormationGroup> formationGroups = new List<FormationGroup>();
        EntityManager.GetAllUniqueSharedComponentData<FormationGroup>(formationGroups);

        foreach (var formationGroup in formationGroups)
        {
            foreach(var leader in selectedLeaders)
            {
                if(formationGroup.LeaderId == leader)
                {
                    Entities
                        .WithAll<SteeringAgent, TestudoFormation>()
                        .WithSharedComponentFilter(formationGroup)
                        .ForEach((Entity entity, int entityInQueryIndex) => {
                            ecb.RemoveComponent(entityInQueryIndex, entity, ComponentType.ReadOnly<TestudoFormation>());
                            ecb.AddComponent(entityInQueryIndex, entity, componentToAdd);
                    }).ScheduleParallel();

                    Entities
                        .WithAll<SteeringAgent, OrbFormation>()
                        .WithSharedComponentFilter(formationGroup)
                        .ForEach((Entity entity, int entityInQueryIndex) => {
                            ecb.RemoveComponent(entityInQueryIndex, entity, ComponentType.ReadOnly<OrbFormation>());
                            ecb.AddComponent(entityInQueryIndex, entity, componentToAdd);
                        }).ScheduleParallel();

                    Entities
                        .WithAll<SteeringAgent, WedgeFormation>()
                        .WithSharedComponentFilter(formationGroup)
                        .ForEach((Entity entity, int entityInQueryIndex) => {
                            ecb.RemoveComponent(entityInQueryIndex, entity, ComponentType.ReadOnly<WedgeFormation>());
                            ecb.AddComponent(entityInQueryIndex, entity, componentToAdd);
                        }).ScheduleParallel();
                }
            }
        }

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
