using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MoveSystem))]
public class SteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float speed = GameManager.Instance.AgentSpeed;
        float slowingDistance = GameManager.Instance.AgentSlowingDistance;

            Entities
                .ForEach((ref Velocity velocity, in Translation translation, in SteeringAgent steeringAgent) =>
            {
                float3 targetOffset = steeringAgent.TargetPosition - translation.Value;
                var dist = math.length(targetOffset);
                var rampedSpeed = speed * dist / slowingDistance;
                var clippedSpeed = math.min(rampedSpeed, speed);

                var steeringVelocity = clippedSpeed / (dist + 0.001f) * targetOffset - velocity.Value;
                velocity.Value += steeringVelocity; //TODO clamp result to maxSpeed
            }).ScheduleParallel();
    }
}
