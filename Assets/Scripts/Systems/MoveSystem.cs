using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(SteeringSystem))]
public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;      

        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Velocity velocity) => {
            translation.Value += velocity.Value * deltaTime;
            if(math.lengthsq(velocity.Value) > 0.1f)
                rotation.Value = quaternion.LookRotationSafe(velocity.Value, new float3(0f, 1f, 0f));
        }).ScheduleParallel();
    }
}
