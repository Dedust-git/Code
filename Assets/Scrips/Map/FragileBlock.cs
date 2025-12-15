using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FragileBlock : MonoBehaviour
{
    [Header("摧毁事件")]
    public System.Action onBreak; // 可选的摧毁事件回调
    
    [Header("破坏设置")]
    public float destroyDelay = 0.5f; // 旋转后多久销毁
    public float rotationAngle = 5f; // 旋转角度
    
    [Header("分裂动画设置")]
    public float jumpForce = 4f;          // 弹跳力度
    public float splitForce = 2f;        // 分裂力度
    public float gravity = 12f;          // 重力
    public float fadeDuration = 1.2f;    // 淡出时间
    public GameObject fragmentPrefab;    // 碎片预制体（可选）
    
    private bool isBroken = false;
    private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;
    private TilemapCollider2D tilemapCollider;
    
    private void Start()
    {
        // 获取Tilemap相关组件
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }
    
    public void OnBreak()
    {
        if (isBroken) return;
        isBroken = true;
        
        // 触发摧毁事件
        onBreak?.Invoke();
        
        // 开始破坏序列
        StartCoroutine(BreakSequence());
    }
    
    // 重载方法：在指定位置破坏
    public void OnBreakAtPosition(Vector3 worldPosition)
    {
        if (isBroken) return;
        
        // 将世界坐标转换为Tilemap的单元格坐标
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        
        if (tilemap.HasTile(cellPosition))
        {
            isBroken = true;
            onBreak?.Invoke();
            StartCoroutine(BreakTileAtPosition(cellPosition, worldPosition));
        }
    }
    
    private IEnumerator BreakSequence()
    {
        // 第一阶段：整个Tilemap的旋转和位移效果
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        
        // 应用旋转和位移
        transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z);
        transform.Rotate(0, 0, rotationAngle);
        
        yield return new WaitForSeconds(destroyDelay * 0.3f);
        
        // 第二阶段：为每个有tile的位置创建碎片
        CreateFragmentsForAllTiles();
        
        // 隐藏整个Tilemap
        tilemapRenderer.enabled = false;
        if (tilemapCollider != null)
            tilemapCollider.enabled = false;

        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(destroyDelay * 2.4f);
        
        // 销毁游戏对象
        Destroy(gameObject);
    }
    
    private IEnumerator BreakTileAtPosition(Vector3Int cellPosition, Vector3 worldPosition)
    {
        // 移除指定位置的tile
        tilemap.SetTile(cellPosition, null);
        
        // 创建该位置的碎片
        CreateFragmentAtPosition(worldPosition);
        
        yield return new WaitForSeconds(destroyDelay);
    }
    
    private void CreateFragmentsForAllTiles()
    {
        // 获取所有有tile的位置
        BoundsInt bounds = tilemap.cellBounds;
        
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                CreateFragmentAtPosition(worldPos);
            }
        }
    }
    
    private void CreateFragmentAtPosition(Vector3 worldPosition)
    {
        // 获取tile的精灵
        Sprite tileSprite = GetTileSpriteAtPosition(worldPosition);
        if (tileSprite == null) return;
        
        // 创建左右两个碎片
        CreateFragment(worldPosition, true, tileSprite);
        CreateFragment(worldPosition, false, tileSprite);
    }
    
    private void CreateFragment(Vector3 position, bool isLeftHalf, Sprite tileSprite)
    {
        if (fragmentPrefab != null)
        {
            // 使用预制体创建碎片
            GameObject fragment = Instantiate(fragmentPrefab, position, Quaternion.identity);
            SetupFragmentPhysics(fragment, isLeftHalf);
            StartCoroutine(AnimateFragment(fragment));
        }
        else
        {
            // 动态创建碎片
            GameObject fragment = new GameObject(isLeftHalf ? "LeftFragment" : "RightFragment");
            fragment.transform.position = position;
            
            // 添加精灵渲染器
            SpriteRenderer renderer = fragment.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateHalfSprite(isLeftHalf, tileSprite);
            renderer.sortingOrder = 10;
            
            SetupFragmentPhysics(fragment, isLeftHalf);
            StartCoroutine(AnimateFragment(fragment));
        }
    }
    
    private Sprite GetTileSpriteAtPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        TileBase tile = tilemap.GetTile(cellPosition);
        
        if (tile is Tile)
        {
            return ((Tile)tile).sprite;
        }
        else if (tile is RuleTile)
        {
            // 对于RuleTile，获取默认精灵
            RuleTile ruleTile = tile as RuleTile;
            if (ruleTile.m_DefaultSprite != null)
            {
                return ruleTile.m_DefaultSprite;
            }
        }
        
        return null;
    }
    
    private void SetupFragmentPhysics(GameObject fragment, bool isLeftHalf)
    {
        // 添加刚体
        Rigidbody2D rb = fragment.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // 使用自定义重力
        
        // 添加碰撞体（简单矩形）
        BoxCollider2D collider = fragment.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 1f);
        collider.isTrigger = true;
        
        // 施加力
        Vector2 force = new Vector2(
            isLeftHalf ? -splitForce : splitForce, 
            jumpForce + Random.Range(-0.5f, 0.5f)
        );
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    private Sprite CreateHalfSprite(bool isLeftHalf, Sprite original)
    {
        if (original == null) return null;
        
        // 创建纹理副本（避免修改原始纹理）
        Texture2D originalTexture = original.texture;
        Rect originalRect = original.rect;
        
        // 计算半边纹理的矩形
        Rect halfRect = originalRect;
        if (isLeftHalf)
        {
            halfRect.width /= 2;
        }
        else
        {
            halfRect.x += originalRect.width / 2;
            halfRect.width /= 2;
        }
        
        // 创建新的精灵
        return Sprite.Create(originalTexture, halfRect, new Vector2(0.5f, 0.5f), original.pixelsPerUnit);
    }
    
    private IEnumerator AnimateFragment(GameObject fragment)
    {
        SpriteRenderer renderer = fragment.GetComponent<SpriteRenderer>();
        Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
        
        if (renderer == null || rb == null) yield break;
        
        Vector2 velocity = rb.velocity;
        float timer = 0f;
        float rotationSpeed = 180f * (rb.velocity.x > 0 ? 1 : -1);
        
        while (timer < fadeDuration && fragment != null)
        {
            if (fragment == null) yield break;
            
            // 应用自定义重力
            velocity.y -= gravity * Time.deltaTime;
            rb.velocity = velocity;
            
            // 检查是否离开屏幕
            if (IsOffScreen(fragment.transform.position))
            {
                Destroy(fragment);
                yield break;
            }
            
            // 淡出效果
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
            
            // 旋转效果
            fragment.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        // 动画结束，销毁碎片
        if (fragment != null)
        {
            Destroy(fragment);
        }
    }
    
    private bool IsOffScreen(Vector3 position)
    {
        if (Camera.main == null) return false;
        
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.x < -0.2f || screenPoint.x > 1.2f || 
               screenPoint.y < -0.2f || screenPoint.y > 1.2f;
    }
    
    // 可选：碰撞检测触发破坏
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 示例：当玩家从上方碰撞时破坏整个Tilemap
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y < -0.5f) // 从上方碰撞
            {
                OnBreak();
            }
        }
    }
    
    // 可选：在特定位置触发破坏
    private void OnCollisionEnter2DWithPosition(Collision2D collision, Vector3 contactPoint)
    {
        // 示例：只在碰撞位置破坏单个tile
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y < -0.5f) // 从上方碰撞
            {
                OnBreakAtPosition(contactPoint);
            }
        }
    }
    
    // 手动触发破坏的方法
    public void Break()
    {
        OnBreak();
    }
    
    // 在指定位置破坏的方法
    public void BreakAtPosition(Vector3 worldPosition)
    {
        OnBreakAtPosition(worldPosition);
    }
}