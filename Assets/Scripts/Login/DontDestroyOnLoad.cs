// Assets/Scripts/DontDestroyBoot.cs
using UnityEngine;
public class DontDestroyBoot : MonoBehaviour
{
    void Awake() => DontDestroyOnLoad(gameObject);
}

