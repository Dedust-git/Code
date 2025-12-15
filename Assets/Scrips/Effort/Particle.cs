using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [System.Serializable]
    public class GroundType
    {
        public string typeName;
        public Color baseColor = Color.white;
        public float colorVariation = 0.1f;
        public float particleLifetime = 1f;
        public float upwardForce = 5f;
        public float horizontalForceRange = 2f;
    }

    [Header("粒子设置")]
    public Sprite particleSprite;
    public Vector2 particleSize = new Vector2(0.1f, 0.1f);
    public int maxParticles = 50;

    [Header("地面类型配置")]
    public GroundTypeEnums groundType;
    public GroundType grassType;
    public GroundType dirtType;
    public GroundType defaultType;
    
    [Header("震动程度映射")]
    public AnimationCurve intensityToParticleCount = AnimationCurve.Linear(0, 1, 1, 10);
    public float minIntensity = 0.1f;
    
    private Queue<GameObject> particlePool = new Queue<GameObject>();
    private List<GameObject> activeParticles = new List<GameObject>();
    private GroundType currentGroundType;
    
    // 内部粒子实例控制器类
    private class ParticleUnit : MonoBehaviour
    {
        private Particle parentSystem;
        private float lifetime;
        private float timer;
        private SpriteRenderer spriteRenderer;
        
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void StartParticleLifecycle(float lifeTime, Particle system)
        {
            this.lifetime = lifeTime;
            this.parentSystem = system;
            this.timer = 0f;
            
            // 重置透明度
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }
        
        void Update()
        {
            if (timer < lifetime)
            {
                timer += Time.deltaTime;
                
                // 渐变消失效果
                if (spriteRenderer != null && timer > lifetime * 0.7f)
                {
                    float alpha = 1f - ((timer - lifetime * 0.7f) / (lifetime * 0.3f));
                    Color color = spriteRenderer.color;
                    color.a = alpha;
                    spriteRenderer.color = color;
                }
            }
            else
            {
                // 生命周期结束，回收粒子
                if (parentSystem != null)
                {
                    parentSystem.ReturnParticleToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    
    void Start()
    {
        InitializeParticlePool();
        currentGroundType = groundType switch
        {
            GroundTypeEnums.grass => grassType,
            GroundTypeEnums.dirt => dirtType,
            _ => defaultType
        };
    }
    
    void Update()
    {
        CleanupFinishedParticles();
    }
    
    // 初始化粒子对象池
    private void InitializeParticlePool()
    {
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject particle = CreateParticleObject();
            particlePool.Enqueue(particle);
        }
    }

    // 创建单个粒子对象
    private GameObject CreateParticleObject()
    {
        GameObject particle = new GameObject("FootstepParticle");
        particle.transform.SetParent(transform, true);//设置父亲
        particle.SetActive(false);

        // 添加SpriteRenderer
        SpriteRenderer renderer = particle.AddComponent<SpriteRenderer>();
        renderer.sprite = particleSprite;
        renderer.sortingOrder = 10;

        // 添加Rigidbody2D
        Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2f; // 较高的重力
        rb.drag = 0.5f; // 空气阻力
        rb.angularDrag = 0.8f; // 角阻力，增加粗糙感

        // 添加碰撞器（可选，根据需要）
        CircleCollider2D collider = particle.AddComponent<CircleCollider2D>();
        collider.radius = 0.05f;

        // 添加粒子控制器组件
        particle.AddComponent<ParticleUnit>();

        return particle;
    }
    
    // 公开方法：触发粒子效果（从玩家脚本调用）

    ///<summary>
    /// 接受玩家传入的力和位置参数制造粒子效果
    ///</summary>
    ///<param name="percentage">力：force，位置：position</param> 
    public void MakeParticle(float force, Vector3 position)
    {
        if (force < minIntensity) return;
        
        int particleCount = Mathf.RoundToInt(intensityToParticleCount.Evaluate(force));
        particleCount = Mathf.Clamp(particleCount, 1, maxParticles);
        
        for (int i = 0; i < particleCount; i++)
        {
            SpawnParticle(position, force);
        }
    }
    
    // 生成单个粒子
    private void SpawnParticle(Vector3 position, float force)
    {
        if (particlePool.Count == 0) return;
        
        GameObject particle = particlePool.Dequeue();
        particle.SetActive(true);
        activeParticles.Add(particle);
        
        // 设置位置
        particle.transform.position = position + new Vector3(Random.Range(-0.1f, 0.1f), 0.1f, 0f);
        particle.transform.localScale = particleSize * Random.Range(0.8f, 1.2f);
        
        // 设置颜色（带随机色差）
        SpriteRenderer renderer = particle.GetComponent<SpriteRenderer>();
        Color particleColor = GetRandomizedColor(currentGroundType);
        renderer.color = particleColor;
        
        // 应用物理力（考虑传入的force参数）
        Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
        ApplyParticleForces(rb, currentGroundType, force);
        
        // 开始粒子生命周期
        ParticleUnit controller = particle.GetComponent<ParticleUnit>();
        controller.StartParticleLifecycle(currentGroundType.particleLifetime, this);
    }
    
    // 获取带随机色差的颜色
    private Color GetRandomizedColor(GroundType groundType)
    {
        Color baseColor = groundType.baseColor;
        float variation = groundType.colorVariation;
        
        return new Color(
            Mathf.Clamp01(baseColor.r + Random.Range(-variation, variation)),
            Mathf.Clamp01(baseColor.g + Random.Range(-variation, variation)),
            Mathf.Clamp01(baseColor.b + Random.Range(-variation, variation)),
            baseColor.a
        );
    }
    
    // 应用粒子受力（考虑force参数）
    private void ApplyParticleForces(Rigidbody2D rb, GroundType groundType, float force)
    {
        // 重置速度
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // 向上的力（受force参数影响）
        float upwardForce = groundType.upwardForce * Random.Range(0.8f, 1.2f) * force;
        rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        
        // 随机水平力（受force参数影响）
        float horizontalForce = Random.Range(-groundType.horizontalForceRange, groundType.horizontalForceRange) * force;
        rb.AddForce(Vector2.right * horizontalForce, ForceMode2D.Impulse);
        
        // 随机旋转
        float torque = Random.Range(-5f, 5f) * force;
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }
    
    // 回收粒子
    public void ReturnParticleToPool(GameObject particle)
    {
        if (activeParticles.Contains(particle))
        {
            activeParticles.Remove(particle);
            particle.SetActive(false);
            particlePool.Enqueue(particle);
        }
    }
    
    // 清理已完成生命周期的粒子
    private void CleanupFinishedParticles()
    {
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            if (!activeParticles[i].activeInHierarchy)
            {
                ReturnParticleToPool(activeParticles[i]);
            }
        }
    }
    
    // 在Inspector中设置默认值
    void Reset()
    {
        // 设置默认的地面类型
        grassType = new GroundType()
        {
            typeName = "Grass",
            baseColor = new Color(0.2f, 0.8f, 0.2f, 1f),
            colorVariation = 0.15f,
            particleLifetime = 1.2f,
            upwardForce = 4f,
            horizontalForceRange = 1.5f
        };
        
        dirtType = new GroundType()
        {
            typeName = "Dirt",
            baseColor = new Color(0.6f, 0.4f, 0.2f, 1f),
            colorVariation = 0.1f,
            particleLifetime = 1f,
            upwardForce = 5f,
            horizontalForceRange = 2f
        };
        
        defaultType = new GroundType()
        {
            typeName = "Default",
            baseColor = Color.gray,
            colorVariation = 0.05f,
            particleLifetime = 0.8f,
            upwardForce = 6f,
            horizontalForceRange = 2.5f
        };
    }
}