// Assets/Scripts/Input/PlayerInputController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerControls controls;

    public delegate void ClickAction(Vector2 screenPosition);
    public static event ClickAction OnClick;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Click.performed += ctx =>
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            OnClick?.Invoke(screenPos);
        };
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
