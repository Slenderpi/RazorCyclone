using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidObject : MonoBehaviour {
    
    [Header("Boid Parameters")]
    [Tooltip("Choose the steering behaviour for this Boid:\n - Seek: steer towards the target's position\n - Flee: exact opposite of Seek\n - Pursuit: steer towards a predicted future position of the target\n - Evade: exact opposite of Pursuit\n - Wander: randomly moves around. The Boid will not track any target")]
    public BoidBehaviour BehaviourForTarget = BoidBehaviour.Seek;
    [Tooltip("Not implemented yet")] // TODO
    public bool EnableFlight = false;
    [Tooltip("If left empty, this Boid's target is the player")]
    public GameObject SpecialTarget;
    [Tooltip("A sort of maximum speed for this Boid. Increasing this allows the Boid to reach higher speeds and sometimes accelerate faster.")]
    public float MaxSteeringVelocity = 15;
    [Tooltip("Determines the steering capability for this Boid. A higher maximum steering force allows sharper turns. If changing this value doesn't quite get the movement you want, consider adjusting the maximum velocity as well.")]
    public float MaxSteeringForce = 10;
    [Tooltip("This might not be kept. Provides an additional front-facing force that scales with how lined up the Boid's velocity is towards the target. If directly facing towards the target, the thrust is ApproachingForwardThrust. If directly facing away from target, the thrust is exactly LeavingForwardThrust")]
    public float ApproachingFowardThrust = 0;
    [Tooltip("This might not be kept. Provides an additional front-facing force that scales with how lined up the Boid's velocity is towards the target. If directly facing away from target, the thrust is exactly LeavingForwardThrust. If directly facing target, the thrust is exactly ApproachingForwardThrust.")]
    public float LeavingFowardThrust = 0;
    
    [Header("References")]
    [SerializeField]
    [Tooltip("The transform of the model to be rotated by this Boid script. If left null, BoidObject will use the transform it is placed on.")]
    Transform ModelToRotate;
    
    Transform modelTransform;
    Rigidbody rb;
    Transform targetTransform;
    Rigidbody targetRigidbody;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        modelTransform = ModelToRotate ? ModelToRotate : transform;
    }
    
    void Start() {
        if (SpecialTarget) {
            targetTransform = SpecialTarget.transform;
            targetRigidbody = SpecialTarget.GetComponent<Rigidbody>();
        } else if (GameManager.CurrentPlayer) {
            setTargetToPlayer(GameManager.CurrentPlayer);
        } else {
            GameManager.A_PlayerSpawned += setTargetToPlayerOnSpawn;
        }
        // GameManager.A_PlayerDestroying += onPlayerDestroying;
    }
    
    void Update() {
        // TODO: Create more parameters to allow easier control over how rotation acts
        Vector3 forward = rb.velocity;
        if (forward.x == 0 && forward.y == 0)
            forward = transform.forward;
        forward.y = 0;
        modelTransform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    void FixedUpdate() {
        Vector3 steer = Vector3.zero;
        Vector3 forwardThrust = rb.velocity.normalized;
        if (targetTransform) {
            switch (BehaviourForTarget) {
            case BoidBehaviour.Seek:
                steer = Seek(targetTransform.position);
                break;
            case BoidBehaviour.Flee:
                steer = Flee(targetTransform.position);
                break;
            case BoidBehaviour.Pursuit:
                steer = Pursuit(targetTransform.position, targetRigidbody.velocity);
                break;
            case BoidBehaviour.Evade:
                steer = Evade(targetTransform.position, targetRigidbody.velocity);
                break;
            case BoidBehaviour.Wander:
                steer = Wander();
                break;
            }
            forwardThrust *= Mathf.Lerp(
                LeavingFowardThrust,
                ApproachingFowardThrust,
                (Vector3.Dot(rb.velocity, targetTransform.position - transform.position) + 1) / 2f
            );
        } else {
            steer = Wander();
            forwardThrust *= ApproachingFowardThrust;
        }
        
        // // If within fleeDist and negative dot, set passTime to now
        // // If time - passTime < fleeDuration, flee, else seek
        // float fleeDist = 6f;
        // float fleeDuration = 1;
        // if (Time.fixedTime > passTime) {
        //     steer = Pursuit(targetTransform.position, targetRigidbody.velocity);
        //     Vector3 toTarget = targetTransform.position - transform.position;
        //     if (toTarget.magnitude <= fleeDist && Vector3.Dot(rb.velocity, toTarget) < 0) {
        //         passTime = Time.fixedTime + fleeDuration;
        //         print("Pass time: " + passTime);
        //     }
        // } else {
        //     // steer = Flee(targetTransform.position);
        //     steer = Evade(targetTransform.position, targetRigidbody.velocity);
        // }
        // rb.AddForce(steer, ForceMode.Acceleration);
        rb.AddForce(steer + forwardThrust, ForceMode.Acceleration);
    }
    
    // float passTime = -500;
    
    public Vector3 Seek(Vector3 targetPos) {
        // Find desired velocity via |targ - pos| * maxSteerVel
        // Steer force is then desired - currVel, clamped by maxSteerForce
        return Vector3.ClampMagnitude((targetPos - transform.position).normalized * MaxSteeringVelocity - rb.velocity, MaxSteeringForce);
    }
    
    public Vector3 Flee(Vector3 targetPos) {
        // NOTE: Verify that flee is actually -seek()
        return -Seek(targetPos);
    }
    
    public Vector3 Pursuit(Vector3 targetPos, Vector3 targetVel) {
        float predictTime = (targetPos - transform.position).magnitude / rb.velocity.magnitude;
        return Seek(targetPos + targetVel * predictTime);
    }
    
    public Vector3 Evade(Vector3 targetPos, Vector3 targetVel) {
        // NOTE: Verify that evade is actually -pursuit()
        return -Pursuit(targetPos, targetVel);
    }
    
    public Vector3 Wander() {
        // TODO
        return Vector3.forward;
    }
    
    void setTargetToPlayer(PlayerCharacterCtrlr plr) {
        targetTransform = plr.transform;
        targetRigidbody = plr.rb ? plr.rb : plr.GetComponent<Rigidbody>();
    }
    
    void setTargetToPlayerOnSpawn(PlayerCharacterCtrlr plr) {
        GameManager.A_PlayerSpawned -= setTargetToPlayer;
        setTargetToPlayer(plr);
    }

    // void onPlayerDestroying(PlayerCharacterCtrlr plr) {
    //     BehaviourForTarget = BoidBehaviour.Wander;
    // }
    
}

public enum BoidBehaviour {
    Seek,
    Flee,
    Pursuit,
    Evade,
    Wander
}