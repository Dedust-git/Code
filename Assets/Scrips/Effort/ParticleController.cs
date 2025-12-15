using System.Collections.Generic;
using UnityEngine;
// 粒子控制器组件
public class ParticleController : MonoBehaviour
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