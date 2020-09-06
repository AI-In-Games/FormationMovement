//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;

//[UpdateAfter(typeof(MoveSystem))]
//public class CollisionSystem : SystemBase
//{
//    EntityQuery m_SoldierGroup;
//    EntityQuery m_ProjectileGroup;

//    protected override void OnCreate()
//    {
//        m_SoldierGroup = GetEntityQuery(ComponentType.ReadOnly<Health>(), ComponentType.ReadOnly<Translation>());
//        m_ProjectileGroup = GetEntityQuery(ComponentType.ReadOnly<Projectile>(), ComponentType.ReadOnly<TimeToLive>(), ComponentType.ReadOnly<Translation>());
//    }

//    protected override void OnUpdate()
//    {
//        var radiusToCheckCollision = 1f;

//        var job = new CollisionJob()
//        {
//            radius = radiusToCheckCollision * radiusToCheckCollision,
//            healthType = GetArchetypeChunkComponentType<Health>(),
//            translationType = GetArchetypeChunkComponentType<Translation>(),
//            translationsToTestAgainst = m_ProjectileGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
//        };

//        this.Dependency = job.Schedule(m_SoldierGroup, this.Dependency);
//    }

//    [BurstCompile]
//    struct CollisionJob : IJobChunk
//    {
//        public float radius;

//        public ArchetypeChunkComponentType<Health> healthType;
//        public ArchetypeChunkComponentType<Translation> translationType;

//        [DeallocateOnJobCompletion]
//        [ReadOnly]
//        public NativeArray<Translation> translationsToTestAgainst;

//        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//        {
//            var chunkHealths = chunk.GetNativeArray(healthType);
//            var chunkTranslations = chunk.GetNativeArray(translationType);

//            for (int i = 0; i < chunk.Count; i++)
//            {
//                int damage = 0;
//                var health = chunkHealths[i];
//                var posA = chunkTranslations[i].Value;

//                for (int j = 0; j < translationsToTestAgainst.Length; j++)
//                {
//                    var posB = translationsToTestAgainst[j].Value;

//                    if (CheckCollision(posA, posB, radius))
//                    {
//                        damage += 1;
//                    }
//                }

//                health.Value -= damage;
//                chunkHealths[i] = health;
//            }
//        }

//        public static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
//        {
//            float3 delta = posA - posB;
//            float distanceSquare = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

//            return distanceSquare <= radiusSqr;
//        }
//    }
//}
