using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
//using Unity.Physics;
using UnityEngine;
//using RaycastHit = Unity.Physics.RaycastHit;

namespace Landfill
{
    public static class Raycast
    {
        public static Entity ResultAsEntity(World world, float3 from, float3 to)
        {
            /*var physicsWorldSystem = world.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Start = from,
                End = to,
                Filter = CollisionFilter.Default
            };

            RaycastHit hit = new Unity.Physics.RaycastHit();
            bool haveHit = collisionWorld.CastRay(input, out hit);
            if (haveHit)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                return e;
            }
            return Entity.Null;*/
            return Entity.Null;
        }

        public static RaycastHit ResultAsHit(World world, float3 from, float3 to)
        {
            /*var physicsWorldSystem = world.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Start = from,
                End = to,
                Filter = CollisionFilter.Default
            };

            RaycastHit hit = new Unity.Physics.RaycastHit();
            bool haveHit = collisionWorld.CastRay(input, out hit);
            if (!haveHit)
            {
                Debug.LogError("MISSED!");
                return default(RaycastHit);
            }
            else
            {
                Debug.LogError("HIT!!");
            }*/
            RaycastHit hit = new RaycastHit();
            return hit;
        }
    }
}


