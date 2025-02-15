using UnityEngine;

public class LavaWeakpoint : EnemyWeakpoint {
    
    [Header("Lava Weakpoint Config")]
    [Tooltip("The weakpoint's height will be at the lava's max height + this amount.")]
    public float WeakpointHeightAboveLava = 2;
    public float AnimationDuration = 1;
    
    public Transform weakpointTransform;
    
    // NOTE: Likely temporary
    [SerializeField]
    Transform lid;
    
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

    void Update() {
        if (shouldAnimate) {
            float t = (Time.time - lastBeginTime) / AnimationDuration;
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
                    weakpointTransform.gameObject.SetActive(false);
                }
            }
            float y = interpHeight(t);
            float r = interpLid(t);
            weakpointTransform.position = new(weakpointTransform.position.x, y, weakpointTransform.position.z);
            lid.localRotation = Quaternion.AngleAxis(r, Vector3.right);
        } else if (isExposed) {
            weakpointTransform.position = new(
                weakpointTransform.position.x,
                lava.MaxLavaHeight + WeakpointHeightAboveLava,
                weakpointTransform.position.z
            );
        }
    }

    protected override void OnDefeated(EDamageType damageType) {
        // Same logic as normal OnDeafeated(), but drop fuel at actual weakpoint position
        if (damageType == EDamageType.Vacuum) {
            GameManager.CurrentPlayer.AddFuel(FuelAmount);
        } else {
            DropFuel(weakpointTransform.position);
        }
        gameObject.SetActive(false);
        A_WeakpointDefeated.Invoke(damageType);
    }

    public void BeginExpose() {
        lastBeginTime = Time.time;
        weakpointTransform.gameObject.SetActive(true);
        isExposed = true;
        shouldAnimate = true;
    }
    
    public void BeginHide() {
        lastBeginTime = Time.time;
        isExposed = false;
        shouldAnimate = true;
    }
    
    float interpHeight(float t) {
        return Mathf.Lerp(transform.position.y + startY, lava.MaxLavaHeight + WeakpointHeightAboveLava, t);
    }
    
    float interpLid(float t) {
        return Mathf.Lerp(0, -90, t);
    }
    
}