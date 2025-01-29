using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoidObject : MonoBehaviour {
    
    [Header("Boid Parameters")]
    [Tooltip("If false, steering forces will have their Y value set to 0.")]
    public bool AllowFlight = false;
    [Tooltip("NOT YET IMPLEMENTED.\nIf enabled, this Boid will also add Wander steering. This is useful for adding extra noise to the Boid's movement.\nNote: if AddWander is enabled and the BoidTargetList is empty, the Boid will not Wander extra.")]
    public bool AddWander = false;
    [Tooltip("A sort of maximum speed for this Boid. Increasing this allows the Boid to reach higher speeds and sometimes accelerate faster.")]
    public float MaxSteeringVelocity = 15;
    [Tooltip("Determines the steering capability for this Boid. A higher maximum steering force allows sharper turns. If changing this value doesn't quite get the movement you want, consider adjusting the maximum velocity as well.")]
    public float MaxSteeringForce = 10;
    [Tooltip("List of targets to track and specific behaviours for each. If left empty, this Boid will enter Wander behaviour.")]
    public BehaviourItem[] BoidTargetList = null;
    [Tooltip("THIS MIGHT NOT BE KEPT.\nProvides an additional front-facing force that scales with how lined up the Boid's velocity is towards the target. If directly facing towards the target, the thrust is ApproachingForwardThrust. If directly facing away from target, the thrust is exactly LeavingForwardThrust")]
    public float ApproachingFowardThrust = 0;
    [Tooltip("THIS MIGHT NOT BE KEPT.\nProvides an additional front-facing force that scales with how lined up the Boid's velocity is towards the target. If directly facing away from target, the thrust is exactly LeavingForwardThrust. If directly facing target, the thrust is exactly ApproachingForwardThrust.")]
    public float LeavingFowardThrust = 0;
    
    [Header("References")]
    [SerializeField]
    [Tooltip("The transform of the model to be rotated by this Boid script. If left null, BoidObject will use the transform it is placed on.")]
    Transform ModelToRotate;
    
    bool includeWander = false;
    Transform modelTransform;
    Rigidbody rb;
    
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        modelTransform = ModelToRotate ? ModelToRotate : transform;
    }
    
    void Start() {
        if (BoidTargetList == null || BoidTargetList.Length == 0) {
            // print("No targets in listing for this Boid.");
        } else if (GameManager.CurrentPlayer) {
            initBoidListReferences(GameManager.CurrentPlayer);
        } else {
            GameManager.A_PlayerSpawned += setTargetsOnPlayerSpawn;
        }
        if (BoidTargetList == null || BoidTargetList.Length == 0 || AddWander)
            includeWander = true;
    }
    
    void Update() {
        // TODO: Create more parameters to allow easier control over how rotation acts
        Vector3 forward = rb.velocity;
        if (forward.x == 0 && forward.y == 0)
            forward = transform.forward;
        forward.y = 0;
        if (forward.sqrMagnitude == 0) return; // If vel/forward is 0, maintain prev rot
        modelTransform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    void FixedUpdate() {
        if (!GameManager.CurrentPlayer) return;
        
        Vector3 totalSteer = includeWander ? Wander() : Vector3.zero;
        if (BoidTargetList != null && BoidTargetList.Length > 0) {
            foreach (BehaviourItem item in BoidTargetList) {
                totalSteer += calcSteerForTargetItem(item);
            }
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
        // rb.AddForce(totalSteer + forwardThrust, ForceMode.Acceleration);
        if (!AllowFlight) totalSteer.y = 0;
        rb.AddForce(totalSteer, ForceMode.Acceleration);
    }
    
    // float passTime = -500;
    
    public Vector3 Seek(Vector3 targetPos) {
        // Find desired velocity via |targ - pos| * maxSteerVel
        // Steer force is then desired - currVel, clamped by maxSteerForce
        return Vector3.ClampMagnitude((targetPos - transform.position).normalized * MaxSteeringVelocity - rb.velocity, MaxSteeringForce);
    }
    
    public Vector3 Flee(Vector3 targetPos) {
        return -Seek(targetPos);
    }
    
    /// <summary>
    /// Exact opposite of Seek(). Internally just returns -Seek().
    /// </summary>
    /// <param name="targetPos">Position to flee from.</param>
    /// <param name="fleeTriggerDist">A special value of 0 means this steering is forcefully enabled.</param>
    /// <returns></returns>
    public Vector3 Flee(Vector3 targetPos, float fleeTriggerDist) {
        if (fleeTriggerDist == 0 || (targetPos - transform.position).sqrMagnitude <= fleeTriggerDist * fleeTriggerDist)
            return Flee(targetPos);
        else
            return Vector3.zero;
    }
    
    public Vector3 Pursuit(Vector3 targetPos, Vector3 targetVel) {
        float predictTime = Mathf.Sqrt((targetPos - transform.position).sqrMagnitude / (rb.velocity - targetVel).sqrMagnitude);
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
    
    void initBoidListReferences(PlayerCharacterCtrlr plr) {
        foreach (BehaviourItem item in BoidTargetList) {
            item.init(plr);
        }
    }
    
    Vector3 calcSteerForTargetItem(BehaviourItem item) {
        Vector3 steer = Vector3.zero;
        if (item.BehaviourType == BoidBehaviour.Wander)
            steer = Wander();
        else if (item.TestTriggerDistance(transform.position)) {
            switch (item.BehaviourType) {
            case BoidBehaviour.Seek:
                steer = Seek(item.trans.position);
                break;
            case BoidBehaviour.Flee:
                steer = Flee(item.trans.position);
                break;
            case BoidBehaviour.Pursuit:
                steer = Pursuit(item.trans.position, item.rb.velocity);
                break;
            case BoidBehaviour.Evade:
                steer = Evade(item.trans.position, item.rb.velocity);
                break;
            case BoidBehaviour.Wander:
                break;
            }
        }
        return steer;
    }
    
    void setTargetsOnPlayerSpawn(PlayerCharacterCtrlr plr) {
        GameManager.A_PlayerSpawned -= setTargetsOnPlayerSpawn;
        initBoidListReferences(plr);
    }
    
}

public enum BoidBehaviour {
    Seek,
    Flee,
    Pursuit,
    Evade,
    Wander
}

[Serializable]
public class BehaviourItem {
    [Tooltip("Choose the steering behaviour for this Boid:\n - Seek: steer towards the target's position\n - Flee: exact opposite of Seek\n - Pursuit: steer towards a predicted future position of the target\n - Evade: exact opposite of Pursuit\n - Wander: randomly moves around. The Boid will not track any target")]
    public BoidBehaviour BehaviourType;
    [Tooltip("The target to apply this behaviour towards. If left empty, this Target will be set as the Player.")]
    public GameObject Target;
    [Tooltip("Distance at which this behaviour is enabled (if 0, the behaviour is always enabled regardless of distance).\nFor example, a BehaviourType of Flee and TriggerDistance of 10 means this Boid will only Flee when within 10 units of the Target.\nNote: if the BehaviourType is Wander, this value doesn't matter.")]
    public float TriggerDistance;
    [Tooltip("If false, this Behaviour is used when the distance to the Target is <= TriggerDistance.\nIf true, this Behaviour is used when the distance to the Target is > TriggerDistance.\nNotice that true means >, while false means <=")]
    public bool CheckAsGreater = false;
    
    [HideInInspector]
    public Transform trans;
    [HideInInspector]
    public Rigidbody rb;
    
    public delegate bool TriggerDistanceTester(Vector3 boidPosition);
    public TriggerDistanceTester TestTriggerDistance;
    
    
    
    public void init(PlayerCharacterCtrlr plr) {
        if (Target) {
            trans = Target.transform;
            rb = Target.GetComponent<Rigidbody>();
        } else {
            trans = plr.transform;
            rb = plr.rb;
        }
        if (TriggerDistance == 0)
            TestTriggerDistance = triggerDistAnywhere;
        else if (CheckAsGreater)
            TestTriggerDistance = triggerDistGreater;
        else
            TestTriggerDistance = triggerDistLEQ;
    }
    
    bool triggerDistAnywhere(Vector3 boidPosition) {
        return true;
    }
    
    bool triggerDistGreater(Vector3 boidPosition) {
        return (trans.position - boidPosition).sqrMagnitude > TriggerDistance * TriggerDistance;
    }
    
    bool triggerDistLEQ(Vector3 boidPosition) {
        return (trans.position - boidPosition).sqrMagnitude <= TriggerDistance * TriggerDistance;
    }
    
}