using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour {
    
    // 
    
    
    
    class Baker : Baker<ProjectileAuthoring> {
        public override void Bake(ProjectileAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Projectile());
			if (auth.GetComponent<Rigidbody>() != null) {
                Debug.LogError($"ERROR: The projectile on object \"{auth.name}\" does not have a rigidbody! Ensure that authoring is done on the component with the rigidbody.");
            }
        }
    }
    
}



/// <summary>
/// Component for all projectiles.<br/>
/// Collision detection is done rearward from the projectile so that collisions look better.
/// </summary>
public struct Projectile : IComponentData {
    public CollisionFilter Filter;
    /// <summary>
    /// The radius of this projectile. Used for spherecast collision detection.<br/>
    /// This value must be greater than 0.
    /// </summary>
    public float Radius;
    /// <summary>
    /// Determines if this projectile detected a hit this frame.
    /// </summary>
    public bool DidHitThisFrame;
    /// <summary>
    /// ColliderCastHit information for the collision.
    /// </summary>
    public ColliderCastHit Hit;

    [BurstCompile]
    public void SetFilter(in uint collidesWith) {
        Filter = new() {
            BelongsTo = ~0u, // Ray itself isn't part of a layer
            CollidesWith = collidesWith,
            GroupIndex = 0
        };
    }
}