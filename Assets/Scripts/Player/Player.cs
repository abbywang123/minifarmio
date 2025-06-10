using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
public class Player : NetworkBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;

    [Header("動畫設定")]
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // ✅ 加入本地玩家檢查，僅本地玩家綁定攝影機
        if (IsOwner)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                CameraFollow follow = cam.GetComponent<CameraFollow>();
                if (follow != null)
                {
                    follow.SetTarget(transform);
                }
                else
                {
                    Debug.LogWarning("⚠ Main Camera 沒有掛 CameraFollow 腳本！");
                }
            }
        }
    }

    // ✅ Unity Input System - 由 PlayerInput 自動呼叫
    public void OnMove(InputValue value)
    {
        // ✅ 僅本地玩家能處理輸入（防止非Owner角色移動）
        if (!IsOwner) return;

        movementInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
            animator.SetFloat("Speed", movementInput.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.MovePosition(rb.position + movementInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
