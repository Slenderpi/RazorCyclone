using System.Collections;
using UnityEngine;

public class CentipedeEnemy : EnemyBase {
    
    enum MissileAction {
        Waiting,
        OpeningDoor,
        WaitOpen,
        FiringMissile,
        LeaveOpen,
        ClosingDoor,
        NONE
    }
    
    [Header("Centipede Config")]
    public CentipedeEnemySO CentConfig;
    [Tooltip("Number of body segments to spawn.\nA value of 1 means there will be 1 body segment and 1 head (this object), resulting in a total length of 2.")]
    public int BodyLength = 10; // Only matters if this piece is spawned as a head. A value of 1 means there will be a head (this obj) and 1 body piece.
    protected int shrunkenLengthUse; // Length of a Centipede for when it's been divided. Used by rear half
    protected int trackedShrunkenLength; // Length of Centipede for when it's been divided. Used by front half
    [SerializeField]
    [Tooltip("Transform that the missile will be launched form. Make sure the forward (-z) direction of this transform is set correctly, because missiles will spawn with this orientation.")]
    Transform missileLaunchPoint;
    [SerializeField]
    Transform doorHinge;
    [SerializeField]
    MeshRenderer modelMeshRenderer;
    
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
    float randRoamOffset;
    WaitForSeconds sampleWaiter;
    float lastMissileActionTime;
    MissileAction currMissileAction;
    CentipedeMissile pooledMissile;
    GameObject pooledFireEffect;
    
    Vector3 startPos;
    Quaternion startRot;
    
    protected bool headDoneInitializing = false;
    
    
    
    protected override void Init() {
        if (head) return;
        startPos = transform.position;
        startRot = transform.rotation;
        samplingDelay = CentConfig.FollowOffset / CentConfig.MoveSpeed;
        shrunkenLengthUse = BodyLength;
        trackedShrunkenLength = BodyLength;
    }
    
    protected override void LateInit() {
        base.LateInit();
        // StartCoroutine(initCentipede());
        randRoamOffset = Random.Range(0, GameManager.Instance.currentSceneRunner.centipedeCircleCompleteTime);
        samplingLength = BodyLength + 1;
        if (head == null) {
            StartCoroutine(staggerSpawnAndStartBody());
        }
        poolMissile();
        currMissileAction = MissileAction.NONE;
    }
    
    IEnumerator staggerSpawnAndStartBody() {
        CentipedeEnemy prev = this;
        int counter = 0;
        for (int i = 0; i < BodyLength; i++) {
            CentipedeEnemy ce = Instantiate(this);
            ce.transform.SetPositionAndRotation(startPos, startRot);
            ce.head = this;
            ce.cePre = prev;
            prev.ceAft = ce;
            ce.bodyIndex = i + 1; // First body piece is at 1. I'm considering the head as 0
            ce.samplingDelay = samplingDelay;
            ce.sampleWaiter = new WaitForSeconds(samplingDelay);
            ce.headDoneInitializing = true;
            prev = ce;
            if (++counter >= 1) {
                counter = 0;
                yield return null;
            }
        }
        sampledPositions = new Vector3[samplingLength];
        sampledPositions[BodyLength] = transform.position;
        sampledRots = new Quaternion[samplingLength];
        sampledRots[BodyLength] = transform.rotation;
        sampleWaiter = new WaitForSeconds(samplingDelay);
        setStartSamples();
        lastSampleTime = Time.time;
        headDoneInitializing = true;
        StartCoroutine(sampleTransform());
        startMissileTimers(Time.time);
        modelMeshRenderer.material = CentConfig.HeadMaterial;
    }
    
    void Update() {
        if (head == null) {
            if (!headDoneInitializing) return;
            if (!GameManager.CurrentPlayer) return;
            Vector3 roamPoint = GameManager.Instance.currentSceneRunner.mapCenter;
            float rad = Mathf.LerpUnclamped(
                GameManager.Instance.currentSceneRunner.minCentipedeRoamRadius,
                GameManager.Instance.currentSceneRunner.maxCentipedeRoamRadius,
                (float)shrunkenLengthUse / BodyLength
            );
            float height = Mathf.LerpUnclamped(
                GameManager.Instance.currentSceneRunner.minCentipedeRoamHeightRange,
                GameManager.Instance.currentSceneRunner.maxCentipedeRoamHeightRange,
                (float)shrunkenLengthUse / BodyLength
            );
            float t = (Time.time + randRoamOffset) * 2f * Mathf.PI / GameManager.Instance.currentSceneRunner.centipedeCircleCompleteTime;
            roamPoint.x += rad * Mathf.Cos(t);
            roamPoint.z += rad * Mathf.Sin(t);
            roamPoint.y += height * Mathf.Sin(Time.time * 2f * Mathf.PI / GameManager.Instance.currentSceneRunner.centipedeHeightCompletionTime);
#if UNITY_EDITOR && true
            GameManager.D_DrawPoint(roamPoint, Color.green, Time.deltaTime);
            Debug.DrawRay(transform.position, roamPoint - transform.position, Color.cyan, Time.deltaTime);
#endif
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(roamPoint - transform.position), 60 * Time.deltaTime);
            transform.position += CentConfig.MoveSpeed * Time.deltaTime * transform.forward;
        } else {
            if (!head.headDoneInitializing) return;
            transform.SetPositionAndRotation(
                head.getLerpedPosition(bodyIndex),
                head.getLerpedRotation(bodyIndex)
            );
        }
        handleMissileActionState();
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        if (ceAft)
            ceAft.becomeNewHead(head? head : this);
        if (head != null) {
            cePre.ceAft = null;
            head.updateHeadAboutDefeat(bodyIndex);
        }
        base.OnDefeated(damageType);
    }

    protected override void OnDestroying() {
        base.OnDestroying();
        if (pooledMissile)
            Destroy(pooledMissile.gameObject);
    }

    void updateHeadAboutDefeat(int index) {
        trackedShrunkenLength = index - 1;
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
    
    void onNewHeadWasCreated(CentipedeEnemy newHead, int newIndex, int newCurrBodLen) {
        bodyIndex = newIndex;
        head = newHead;
        trackedShrunkenLength = newCurrBodLen;
        if (ceAft)
            ceAft.onNewHeadWasCreated(newHead, newIndex + 1, newCurrBodLen);
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
        shrunkenLengthUse = trackedShrunkenLength - bodyIndex;
        trackedShrunkenLength = shrunkenLengthUse;
        randRoamOffset = Random.Range(0, GameManager.Instance.currentSceneRunner.centipedeCircleCompleteTime);
        bodyIndex = 0;
        modelMeshRenderer.material = CentConfig.HeadMaterial;
        if (ceAft)
            ceAft.onNewHeadWasCreated(this, 1, shrunkenLengthUse);
        StartCoroutine(sampleTransform());
    }
    
    void handleMissileActionState() {
        switch (currMissileAction) {
        case MissileAction.Waiting:
            if (Time.time - lastMissileActionTime >= CentConfig.MissileFireDelay - CentConfig.MissileDoorAnimTime * 2 - CentConfig.MissileDoorLeaveOpenTime - CentConfig.MissileDoorWaitOpenTime) {
                lastMissileActionTime += CentConfig.MissileFireDelay - CentConfig.MissileDoorAnimTime * 2 - CentConfig.MissileDoorLeaveOpenTime - CentConfig.MissileDoorWaitOpenTime;
                currMissileAction = MissileAction.OpeningDoor;
            }
            break;
        case MissileAction.OpeningDoor:
            float alpha = (Time.time - lastMissileActionTime) / CentConfig.MissileDoorAnimTime;
            if (alpha >= 1) {
                animateDoors(1);
                lastMissileActionTime += CentConfig.MissileDoorAnimTime;
                currMissileAction = MissileAction.WaitOpen;
            } else {
                animateDoors(alpha);
            }
            break;
        case MissileAction.WaitOpen:
            if (Time.time - lastMissileActionTime >= CentConfig.MissileDoorWaitOpenTime) {
                lastMissileActionTime += CentConfig.MissileDoorWaitOpenTime;
                currMissileAction = MissileAction.FiringMissile;
            }
            break;
        case MissileAction.FiringMissile:
            pooledMissile.transform.SetPositionAndRotation(missileLaunchPoint.position, missileLaunchPoint.rotation);
            pooledMissile.gameObject.SetActive(true);
            pooledMissile = null;
            pooledFireEffect.SetActive(true);
            currMissileAction = MissileAction.LeaveOpen;
            break;
        case MissileAction.LeaveOpen:
            if (Time.time - lastMissileActionTime >= CentConfig.MissileDoorLeaveOpenTime) {
                lastMissileActionTime += CentConfig.MissileDoorLeaveOpenTime;
                currMissileAction = MissileAction.ClosingDoor;
            }
            break;
        case MissileAction.ClosingDoor:
            alpha = (Time.time - lastMissileActionTime) / CentConfig.MissileDoorAnimTime;
            if (alpha >= 1) {
                animateDoors(0);
                lastMissileActionTime += CentConfig.MissileDoorAnimTime;
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
        pooledMissile = Instantiate(CentConfig.MissilePrefab, transform.position, transform.rotation);
        pooledMissile.gameObject.SetActive(false);
        pooledFireEffect = Instantiate(CentConfig.MissileFireEffectPrefab, missileLaunchPoint.transform);
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
    
    void startMissileTimers(float startTime) {
        lastMissileActionTime = startTime + CentConfig.MissileFireDelayOffset * bodyIndex;
        currMissileAction = MissileAction.Waiting;
        if (ceAft)
            ceAft.startMissileTimers(startTime);
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