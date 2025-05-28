using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 嘗試移動到出生點（由 SpawnManager 控制）
        MoveToSpawnPoint();
    }

    void Update()
    {
        // 接收鍵盤輸入（舊輸入系統）
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 印出輸入狀態（用於除錯）
        Debug.Log("Move input: " + movement);
    }

    void FixedUpdate()
    {
        // 移動玩家
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void MoveToSpawnPoint()
    {
        if (SpawnManager.Instance != null)
        {
            string spawnName = SpawnManager.Instance.SpawnPointName; // 你應該有這個 getter
            GameObject spawnPoint = GameObject.Find(spawnName);

            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
                Debug.Log($"玩家移動到出生點：{spawnName}");
            }
            else
            {
                Debug.LogWarning($"找不到出生點：{spawnName}");
            }
        }
        else
        {
            Debug.LogWarning("SpawnManager.Instance 為 null！");
        }
    }
}
