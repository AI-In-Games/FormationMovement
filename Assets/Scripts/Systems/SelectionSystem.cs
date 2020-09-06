using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;
using System.Collections.Generic;

public class SelectionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_MoveEcbSystem;
    private EndSimulationEntityCommandBufferSystem m_SelectEcbSystem;

    private float3 m_MouseStartPosition;

    private float m_LastClickTime;

    private Plane m_GroundPlane;

    private static float3 m_CenterPosition;

    private EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_MoveEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_SelectEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        m_GroundPlane = new Plane(Vector3.zero, Vector3.forward, Vector3.right);
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_LastClickTime = (float)Time.ElapsedTime;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (m_GroundPlane.Raycast(ray, out dist))
            {
                float3 selectedPosition = ray.GetPoint(dist);
                m_MouseStartPosition = selectedPosition;

                GameManager.Instance.UpdateSelectionArea(m_MouseStartPosition, m_MouseStartPosition);
                GameManager.Instance.ToggleSelection(true);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (m_GroundPlane.Raycast(ray, out dist))
            {
                float3 currentPosition = ray.GetPoint(dist);

                GameManager.Instance.UpdateSelectionArea(m_MouseStartPosition, currentPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (m_GroundPlane.Raycast(ray, out dist))
            {
                float3 currentPosition = ray.GetPoint(dist);

                var currentTime = (float)Time.ElapsedTime;
                if (currentTime - m_LastClickTime < GameManager.Instance.m_ClickTime)
                {
                    Move(currentPosition);
                }
                else
                {
                    Vector3 center, size;
                    GameManager.Instance.GetBounds(m_MouseStartPosition, currentPosition, out center, out size);
                    GameManager.Instance.ToggleSelection(false);

                    AABB selectionBounds = new AABB();
                    selectionBounds.Center = center;
                    selectionBounds.Extents = size / 2f;
                    
                    Select(selectionBounds);
                }
            }
        }
    }

    private void Select(AABB selectionBounds)
    {
        Debug.Log("Select");
        m_CenterPosition = float3.zero;

        var ecb = m_SelectEcbSystem.CreateCommandBuffer().AsParallelWriter();
        var formGroup = new FormationGroup() { LeaderId = -1 };
        Entities
            //.WithoutBurst()
            .WithAll<FormationGroup>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation/*, in RenderBounds bounds*/) =>
        {
            //ecb.RemoveComponent<FormationIndex>(entityInQueryIndex, entity);
            if (selectionBounds.Contains(translation.Value))
            {
            //ecb.SetSharedComponent(entityInQueryIndex, entity, formGroup);
                ecb.AddComponent<SelectedComponent>(entityInQueryIndex, entity);

                //m_CenterPosition += translation.Value;
            }
        }).ScheduleParallel();

        m_SelectEcbSystem.AddJobHandleForProducer(this.Dependency);
    }

    private void Move(float3 position)
    {
        Debug.Log("Move");
        List<FormationGroup> cohorts = new List<FormationGroup>();
        EntityManager.GetAllUniqueSharedComponentData<FormationGroup>(cohorts);
        foreach (FormationGroup cohort in cohorts)
        {
            if (cohort.LeaderId >= 0)
                continue;

            var ecb = m_MoveEcbSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.WithSharedComponentFilter(cohort)
                .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, ref SteeringAgent agent) => 
                {
                    //var localPositionInFormation = translation.Value - center;
                    var targetPosition = position;// + localPositionInFormation;
                    agent.TargetPosition = targetPosition;

                    ecb.RemoveComponent<SelectedComponent>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<FormationGroup>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TestudoFormation>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<OrbFormation>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<WedgeFormation>(entityInQueryIndex, entity);
                })
                .ScheduleParallel();
        }

        /*
        var ecb = m_MoveEcbSystem.CreateCommandBuffer().AsParallelWriter();

        int count = m_Query.CalculateEntityCount();

        float3 center = m_CenterPosition / count;
        Entities
            .WithAll<SelectedComponent>()
            .WithStoreEntityQueryInField(ref m_Query)
            .ForEach((Entity entity, int entityInQueryIndex, ref SteeringAgent agent, in Translation translation) =>
        {
            var localPositionInFormation = translation.Value - center;
            var targetPosition = position;// + localPositionInFormation;
            agent.TargetPosition = targetPosition;
            
            ecb.RemoveComponent<SelectedComponent>(entityInQueryIndex, entity);
            ecb.RemoveComponent<FormationGroup>(entityInQueryIndex, entity);
            ecb.RemoveComponent<TestudoFormation>(entityInQueryIndex, entity);
            ecb.RemoveComponent<OrbFormation>(entityInQueryIndex, entity);
            ecb.RemoveComponent<WedgeFormation>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        m_MoveEcbSystem.AddJobHandleForProducer(this.Dependency);*/
    }
}
