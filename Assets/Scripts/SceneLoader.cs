using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void GoToFarm()
    {
        SceneManager.LoadScene("Farm"); // 📝 確保場景名稱正確拼寫
    }
}

