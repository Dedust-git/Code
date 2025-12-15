using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestroyAfterJumpEvent : MonoBehaviour
{
    [Header("弹跳参数")]
    public float initialVerticalSpeed = 5f; // 初始垂直速度
    public float gravity = -15f;           // 重力加速度（比真实重力更强）
    public float bounceFactor = 0.7f;      // 反弹系数 (0-1)
    public float horizontalRandomness = 0.5f; // 水平随机偏移范围
    
    [Header("消失参数")]
    public float fadeDuration = 0.8f;      // 消失时间
    public float destroyDelay = 1.5f;      // 销毁延迟

    private TextMeshProUGUI textMesh;
    private Vector3 velocity;               // 当前速度
    private float currentVerticalSpeed;
    private float fadeTimer;
    private Vector3 startPosition;
    private bool isBouncing;

    public void OnDestroy()
    {
        isBouncing = true;
        textMesh = GetComponent<TextMeshProUGUI>();

        fadeTimer = fadeDuration;
        
        // 设置初始速度
        currentVerticalSpeed = initialVerticalSpeed;
        
        // 添加随机水平偏移
        float randomX = Random.Range(-horizontalRandomness, horizontalRandomness);
        velocity = new Vector3(randomX, currentVerticalSpeed, 0);
        
        // 记录起始位置
        startPosition = transform.position;
        
        // 设置自动销毁
        Destroy(gameObject, destroyDelay);
    }
    void Update()
    {
        if (!isBouncing) return;
        
        // 应用重力
        currentVerticalSpeed += gravity * Time.deltaTime;
        velocity.y = currentVerticalSpeed;
        
        // 更新位置
        transform.position += velocity * Time.deltaTime;
        
        // 渐变消失
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            textMesh.alpha = alpha;
        }
    }
}
