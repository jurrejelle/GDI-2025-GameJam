using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class CharacterControllerScript : MonoBehaviour

{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 50f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float worldSwitchCooldown = 1f;

    private CharacterController cc;
    private Vector3 velocity;
    private float xRotation = 0f;

    private float worldLastSwitched = -100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleWorldSwitching();
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        ApplyGravity();
        
        cc.Move((move * moveSpeed + velocity) * Time.deltaTime);
    }


    private void ApplyGravity()
    {
        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void HandleWorldSwitching()
    {
        if (Keyboard.current.fKey.isPressed)
        {
            if (Time.time <= worldLastSwitched + worldSwitchCooldown) return;
            GameManager.Get().nextWorld();
            worldLastSwitched = Time.time;
        }
    }
}
