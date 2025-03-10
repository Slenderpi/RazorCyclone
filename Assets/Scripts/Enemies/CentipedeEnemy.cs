using System.Collections;
using UnityEngine;

public class CentipedeEnemy : EnemyBase {

    enum MissileAction {
        Waiting,
        OpeningDoor,
        FiringMissile,
        LeaveOpen,
        ClosingDoor
    }
    
    [Header("Centipede Config")]
    [Tooltip("Set this to true in the inspector.\nWhen the head spawns centipede body pieces, it will automatically make sure those pieces have this value set to false.")]
    public bool SpawnAsHead = false;
    [Tooltip("Number of body segments to spawn.\nA value of 1 means there will be 1 body segment and 1 head (this object), resulting in a total length of 2.")]
    public int BodyLength = 10; // Only matters if this piece is spawned as a head. A value of 1 means there will be a head (this obj) and 1 body piece.
    // public int SamplingFrequency = 1; // Number of times to sample per second
    [Tooltip("Movement speed the head will move at.")]
    public float MoveSpeed = 5;
    [Tooltip("Distance the body segments will maintain from the previous segment.")]
    public float FollowOffset = 1; // Distance to keep from previous
    [Tooltip("The time between each missile firing.")]
    public float MissileFireDelay = 5;
    [Tooltip("Additional time to prevent the entire centipede from firing at once.\nExample: using 0.1, the head will fire after MissileFireDelay, the first body will fire after MissileFireDelay + 0.1, the next at MissileFireDelay + 0.2, etc.")]
    public float MissileFireDelayOffset = 0.1f;
    [Tooltip("The length of time the missile door animates opening for.\nDoes not affect MissileFireDelay.")]
    public float MissileDoorAnimTime = 0.2f;
    [Tooltip("The length of the time the missile door stays open right after firing the missile.\nDoes not affect MissileFireDelay.")]
    public float MissileDoorLeaveOpenTime = 0.2f;
    [SerializeField]
    [Tooltip("Prefab of missile. Needs to be of type CentipedeMissile.")]
    CentipedeMissile missilePrefab;
    [SerializeField]
    [Tooltip("VFX to use when a missile gets launched.")]
    GameObject missileFireEffectPrefab;
    [SerializeField]
    [Tooltip("Transform that the missile will be launched form. Make sure the forward (-z) direction of this transform is set correctly, because missiles will spawn with this orientation.")]
    Transform missileLaunchPoint;
    [SerializeField]
    Transform doorHinge;
    [SerializeField]
    MeshRenderer modelMeshRenderer;
    [SerializeField]
    Material headMaterial;
    
    Vector3[] sampledPositions;
    Quaternion[] sampledRots;
    int sampleHead = 0; // Index of most-recently sampled position. The sampledPositions array will be used like a looping array
    int samplingLength;
    
    CentipedeEnemy head = null;
    CentipedeEnemy cePre = null;
    CentipedeEnemy ceAft = null;
    int bodyIndex;
    float samplingDelay;
    float lastSampleTime;
    WaitForSeconds sampleWaiter;
    WaitForSeconds missileWaiter;
    float lastMissileActionTime;
    MissileAction currMissileAction;
    CentipedeMissile pooledMissile;
    GameObject pooledFireEffect;
    
    
    
    protected override void Init() {
        // ConsiderForRicochet = true;
        if (!SpawnAsHead) return;
        SpawnAsHead = false;
        samplingDelay = FollowOffset / MoveSpeed;
        samplingLength = BodyLength + 1;
        Vector3[] samposs = new Vector3[samplingLength];
        samposs[BodyLength] = transform.position;
        Quaternion[] samrots = new Quaternion[samplingLength];
        samrots[BodyLength] = transform.rotation;
        CentipedeEnemy prev = this;
        for (int i = 0; i < BodyLength; i++) {
            // samposs[i] = transform.position - transform.forward * FollowOffset * i;
            // samrots[i] = transform.rotation;
            CentipedeEnemy ce = Instantiate(this);
            ce.head = this;
            ce.cePre = prev;
            prev.ceAft = ce;
            ce.bodyIndex = i + 1; // First body piece is at 1. I'm considering the head as 0
            ce.sampleWaiter = new WaitForSeconds(samplingDelay);
            ce.missileWaiter = new WaitForSeconds(MissileFireDelay);
            ce.samplingDelay = samplingDelay;
            prev = ce;
        }
        prev.ceAft = null; // Make sure the last segment has an aft of null
        sampleWaiter = new WaitForSeconds(samplingDelay);
        missileWaiter = new WaitForSeconds(MissileFireDelay);
        sampledPositions = samposs;
        sampledRots = samrots;
        modelMeshRenderer.material = headMaterial;
        SpawnAsHead = true;
    }
    
    protected override void LateInit() {
        base.LateInit();
        if (head == null) {
            setStartSamples();
            lastSampleTime = Time.time;
            StartCoroutine(sampleTransform());
        }
        poolMissile();
        lastMissileActionTime = Time.time + MissileFireDelayOffset * bodyIndex;
        currMissileAction = MissileAction.Waiting;
    }
    
    void Update() {
        if (head == null) {
            if (!GameManager.CurrentPlayer) return;
            Vector3 toplr = GameManager.CurrentPlayer.transform.position - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toplr), 60 * Time.deltaTime);
            transform.position += MoveSpeed * Time.deltaTime * transform.forward;
        } else {
            transform.SetPositionAndRotation(
                head.getLerpedPosition(bodyIndex),
                head.getLerpedRotation(bodyIndex)
            );
        }
        handleMissileActionState();
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        if (damageType == EDamageType.Vacuum) {
            GameManager.CurrentPlayer.AddFuel(FuelAmount);
        } else {
            DropFuel();
        }
        if (ceAft)
            ceAft.becomeNewHead(head? head : this);
        if (head != null) {
            cePre.ceAft = null;
            head.updateHeadAboutDefeat(bodyIndex);
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    protected override void OnDestroying() {
        base.OnDestroying();
        if (pooledMissile)
            Destroy(pooledMissile.gameObject);
    }

    void updateHeadAboutDefeat(int index) {
        // When a segment is defeated, shrink the sampling arrays
        Vector3[] newPoss = new Vector3[index];
        Quaternion[] newRots = new Quaternion[index];
        for (int i = 0; i < index; i++) {
            int ind = mod(sampleHead + i, samplingLength);
            newPoss[i] = sampledPositions[ind];
            newRots[i] = sampledRots[ind];
        }
        sampledPositions = newPoss;
        sampledRots = newRots;
        // Reset sampleHead
        sampleHead = 0;
        // The index of the body defeated is also the new sampling length
        samplingLength = index;
    }
    
    void onNewHeadWasCreated(CentipedeEnemy newHead, int newIndex) {
        bodyIndex = newIndex;
        head = newHead;
        if (ceAft)
            ceAft.onNewHeadWasCreated(newHead, newIndex + 1);
    }
    
    void becomeNewHead(CentipedeEnemy headRef) {
        head = null;
        cePre = null;
        // Create sample array
        samplingLength = headRef.samplingLength - bodyIndex;
        sampledPositions = new Vector3[samplingLength];
        sampledRots = new Quaternion[samplingLength];
        for (int i = 0; i < samplingLength; i++) {
            int ind = mod(headRef.sampleHead + bodyIndex + i, headRef.samplingLength);
            sampledPositions[i] = headRef.sampledPositions[ind];
            sampledRots[i] = headRef.sampledRots[ind];
        }
        bodyIndex = 0;
        modelMeshRenderer.material = headMaterial;
        if (ceAft)
            ceAft.onNewHeadWasCreated(this, 1);
        StartCoroutine(sampleTransform());
    }
    
    void handleMissileActionState() {
        switch (currMissileAction) {
        case MissileAction.Waiting:
            if (Time.time - lastMissileActionTime >= MissileFireDelay - MissileDoorAnimTime * 2 - MissileDoorLeaveOpenTime) {
                lastMissileActionTime += MissileFireDelay - MissileDoorAnimTime * 2 - MissileDoorLeaveOpenTime;
                currMissileAction = MissileAction.OpeningDoor;
            }
            break;
        case MissileAction.OpeningDoor:
            float alpha = (Time.time - lastMissileActionTime) / MissileDoorAnimTime;
            if (alpha >= 1) {
                animateDoors(1);
                lastMissileActionTime += MissileDoorAnimTime;
                currMissileAction = MissileAction.FiringMissile;
            } else {
                animateDoors(alpha);
            }
            break;
        case MissileAction.FiringMissile:
            pooledMissile.transform.SetPositionAndRotation(missileLaunchPoint.position, missileLaunchPoint.rotation);
            pooledMissile.gameObject.SetActive(true);
            pooledFireEffect.SetActive(true);
            currMissileAction = MissileAction.LeaveOpen;
            break;
        case MissileAction.LeaveOpen:
            if (Time.time - lastMissileActionTime >= MissileDoorLeaveOpenTime) {
                lastMissileActionTime += MissileDoorLeaveOpenTime;
                currMissileAction = MissileAction.ClosingDoor;
            }
            break;
        case MissileAction.ClosingDoor:
            alpha = (Time.time - lastMissileActionTime) / MissileDoorAnimTime;
            if (alpha >= 1) {
                animateDoors(0);
                lastMissileActionTime += MissileDoorAnimTime;
                poolMissile();
                currMissileAction = MissileAction.Waiting;
            } else {
                animateDoors(1 - alpha);
            }
            break;
        }
    }
    
    void animateDoors(float alpha) {
        float angle = Mathf.Lerp(0, -120, alpha * (2 - alpha));
        doorHinge.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
    }
    
    void poolMissile() {
        pooledMissile = Instantiate(missilePrefab, transform.position, transform.rotation);
        pooledMissile.gameObject.SetActive(false);
        pooledFireEffect = Instantiate(missileFireEffectPrefab, missileLaunchPoint.transform);
        pooledFireEffect.SetActive(false);
    }
    
    Vector3 getLerpedPosition(int index) {
        Vector3 from = sampledPositions[(sampleHead + index) % samplingLength];
        Vector3 to = sampledPositions[mod(sampleHead + index - 1, samplingLength)];
        float a = (Time.time - lastSampleTime) / samplingDelay;
        return Vector3.Lerp(from, to, a);
    }
    
    Quaternion getLerpedRotation(int index) {
        Quaternion from = sampledRots[(sampleHead + index) % samplingLength];
        Quaternion to = sampledRots[mod(sampleHead + index - 1, samplingLength)];
        float a = (Time.time - lastSampleTime) / samplingDelay;
        return Quaternion.Slerp(from, to, a);
    }
    
    IEnumerator sampleTransform() {
        yield return sampleWaiter;
        lastSampleTime = Time.time;
        sampleHead = mod(sampleHead - 1, samplingLength);
        sampledPositions[sampleHead] = transform.position;
        sampledRots[sampleHead] = transform.rotation;
        // GameManager.D_DrawPoint(sampledPositions[sampleHead], Color.white, samplingDelay);
        // for (int i = 1; i < samplingLength; i++) {
        //     GameManager.D_DrawPoint(sampledPositions[(sampleHead + i) % samplingLength], Color.green, samplingDelay);
        // }
        StartCoroutine(sampleTransform());
    }
    
    void setStartSamples() {
        for (int i = 0; i < BodyLength; i++) {
            sampledPositions[i] = transform.position;
            sampledRots[i] = transform.rotation;
        }
    }
    
    int mod(float a, float b) {
        return Mathf.FloorToInt(a - b * Mathf.Floor(a / b));
    }
    
}