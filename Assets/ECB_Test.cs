using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;
using System.Collections.Generic;


public class ECB_Test : MonoBehaviour
{

}

public class ECB_Test_System : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_SelectEcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_SelectEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        var ecb = m_SelectEcbSystem.CreateCommandBuffer().AsParallelWriter();
        var formGroup = new FormationGroup() { LeaderId = -1 };
        Entities
            //.WithoutBurst()
            .WithAll<FormationGroup>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation/*, in RenderBounds bounds*/) =>
            {
                ecb.AddComponent<SelectedComponent>(entityInQueryIndex, entity);
                ////ecb.RemoveComponent<FormationIndex>(entityInQueryIndex, entity);
                //if (selectionBounds.Contains(translation.Value))
                //{
                //    //ecb.SetSharedComponent(entityInQueryIndex, entity, formGroup);

                //    //m_CenterPosition += translation.Value;
                //}
            }).ScheduleParallel();

        m_SelectEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
