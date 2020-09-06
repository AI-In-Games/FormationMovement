//using Unity.Entities;
//using Unity.Jobs;

//[UpdateAfter(typeof(CollisionSystem))]
//public class CullingSystem : SystemBase
//{
//    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

//    protected override void OnCreate()
//    {
//        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//    }
//    protected override void OnUpdate()
//    {
//        float deltaTime = Time.DeltaTime;

//        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

//        Entities.ForEach((Entity entity, int entityInQueryIndex, ref TimeToLive timeToLive) =>
//        {
//            timeToLive.Value -= deltaTime;
//            if (timeToLive.Value <= 0f)
//                ecb.DestroyEntity(entityInQueryIndex, entity);
//        }).ScheduleParallel();

//        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Health health) => {
//            if (health.Value <= 0f)
//                ecb.DestroyEntity(entityInQueryIndex, entity);
//        }).ScheduleParallel();

//        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
//    }
//}
