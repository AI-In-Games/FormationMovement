using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(MoveSystem))]
public class GravitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float3 gravity = new float3(0, -9.81f, 0f);
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithAll<Gravity>()
            .ForEach((ref Velocity velocity) => {
                velocity.Value += gravity * deltaTime;
        }).ScheduleParallel();
    }
}
