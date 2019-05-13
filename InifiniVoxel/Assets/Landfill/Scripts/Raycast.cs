using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Landfill
{
    public static class Raycast
    {
        public static Entity ResultAsEntity(float3 from, float3 to)
        {
            var physicsWorldSystem = Unity.Entities.World.Active.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Ray = new Unity.Physics.Ray
                {
                    Origin = from,
                    Direction = to - from
                },
                Filter = new CollisionFilter
                {
                    CategoryBits = ~0u,
                    MaskBits = ~0u,
                    GroupIndex = 0
                }
            };

            RaycastHit hit = new Unity.Physics.RaycastHit();
            bool haveHit = collisionWorld.CastRay(input, out hit);
            if (haveHit)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                return e;
            }
            return Entity.Null;
        }

        public static RaycastHit ResultAsHit(float3 from, float3 to)
        {
            var physicsWorldSystem = Unity.Entities.World.Active.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Ray = new Unity.Physics.Ray
                {
                    Origin = from,
                    Direction = to - from
                },
                Filter = new CollisionFilter
                {
                    CategoryBits = ~0u,
                    MaskBits = ~0u,
                    GroupIndex = 0
                }
            };

            RaycastHit hit = new Unity.Physics.RaycastHit();
            bool haveHit = collisionWorld.CastRay(input, out hit);
            if (!haveHit)
                return default(RaycastHit);
            return hit;
        }
    }
}


