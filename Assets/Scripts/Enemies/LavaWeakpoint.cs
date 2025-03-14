using UnityEngine;

public class LavaWeakpoint : EnemyWeakpoint {
    
    [Header("Lava Weakpoint Config")]
    public LavaWPEnemySO LavaWPConfig;
    
    public Transform weakpointTransform;
    
    // NOTE: Likely temporary
    [SerializeField]
    Transform lid;
    [SerializeField]
    LineRenderer line;
    
    bool shouldAnimate = false;
    bool isExposed = false;
    float startY;
    // float startLid;
    float lastBeginTime = -100f;
    
    
    
    protected override void Init() {
        base.Init();
        // If start enabled, immediate go to disabled state
        weakpointTransform.gameObject.SetActive(false);
        startY = weakpointTransform.localPosition.y;
    }
    
    protected override void LateInit() {
        base.LateInit();
        ConsiderForRicochet = false;
    }
    
    void Update() {
        if (shouldAnimate) {
            float t = (Time.time - lastBeginTime) / LavaWPConfig.AnimationDuration;
            if (isExposed) {
                if (t >= 1) {
                    t = 1;
                    shouldAnimate = false;
                }
            } else {
                t = 1 - t;
                if (t <= 0) {
                    t = 0;
                    shouldAnimate = false;
                    ConsiderForRicochet = false;
                    weakpointTransform.gameObject.SetActive(false);
                }
            }
            float y = interpHeight(t);
            float r = interpLid(t);
            weakpointTransform.position = new(weakpointTransform.position.x, y, weakpointTransform.position.z);
            lid.localRotation = Quaternion.AngleAxis(r, Vector3.right);
            updateLine();
        } else if (isExposed) {
            weakpointTransform.position = new(
                weakpointTransform.position.x,
                lava.MaxLavaHeight + LavaWPConfig.WeakpointHeightAboveLava,
                weakpointTransform.position.z
            );
            updateLine();
        }
    }
    
    void updateLine() {
        line.SetPosition(1, transform.position);
        line.SetPosition(0, weakpointTransform.position);
    }
    
    protected override void OnDefeated(EDamageType damageType) {
        // Same logic as normal OnDeafeated(), but drop fuel at actual weakpoint position
        if (damageType == EDamageType.Vacuum) {
            GameManager.CurrentPlayer.AddFuel(100);
        } else {
            DropFuel(weakpointTransform.position);
        }
        gameObject.SetActive(false);
        A_WeakpointDefeated.Invoke(damageType);
    }
    
    public void BeginExpose() {
        lastBeginTime = Time.time;
        isExposed = true;
        shouldAnimate = true;
        ConsiderForRicochet = true;
        weakpointTransform.gameObject.SetActive(true);
    }
    
    public void BeginHide() {
        lastBeginTime = Time.time;
        isExposed = false;
        shouldAnimate = true;
    }
    
    float interpHeight(float t) {    
        return Mathf.Lerp(
            transform.position.y + startY,
            lava.MaxLavaHeight + LavaWPConfig.WeakpointHeightAboveLava,
            smoothstep4(t)
        );
    }
    
    float interpLid(float t) {
        float c = 0.4f;
        float x = 1;
        if (t < c)
            x = smoothstep4(t / c);
        return Mathf.Lerp(0, -90, x);
    }
    
    float smoothstep4(float t) {
        if (t <= 0.5f) {
            // x^4 / 0.5^3
            return t * t * t * t / 0.125f;
        } else {
            float x = 1 - t;
            return 1 - x * x * x * x / 0.125f;
        }
    }
    
}