using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;

    [Header("動畫設定")]
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>(); // 可選
        }
    }

    void Update()
    {
        // 取得鍵盤輸入
        movement.x = Input.GetAxisRaw("Horizontal"); // 左右 A/D or ←/→
        movement.y = Input.GetAxisRaw("Vertical");   // 上下 W/S or ↑/↓

        // 更新動畫參數
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        // 實際移動角色
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
