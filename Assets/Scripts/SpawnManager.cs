using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private string spawnPointName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetSpawnPoint(string pointName)
    {
        spawnPointName = pointName;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        // 如果 spawnPointName 沒有設定，就用 "StartPoint" 當預設出生點
        string pointName = string.IsNullOrEmpty(spawnPointName) ? "StartPoint" : spawnPointName;
        GameObject spawnPoint = GameObject.Find(pointName);

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
