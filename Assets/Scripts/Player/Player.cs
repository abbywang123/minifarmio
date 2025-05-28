using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 嘗試取得 SpawnManager 中設置的出生點名稱
        if (SpawnManager.Instance != null)
        {
            string spawnPoint = SpawnManager.Instance.SpawnPointName;
            Debug.Log($"[Player] Loaded with spawn point: {spawnPoint}");

            MoveToSpawnPoint(); // 呼叫移動方法
        }
        else
        {
            Debug.LogWarning("[Player] SpawnManager not found!");
        }
    }

    private void Update()
    {
        // 接收鍵盤輸入
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        // 移動玩家
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void MoveToSpawnPoint()
    {
        string spawnName = SpawnManager.Instance.SpawnPointName;
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
}
