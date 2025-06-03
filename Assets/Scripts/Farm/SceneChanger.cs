using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 這個方法會在按鈕被點擊時呼叫
    public void LoadHyiScene()
    {
        SceneManager.LoadScene("hyi");
    }
}
