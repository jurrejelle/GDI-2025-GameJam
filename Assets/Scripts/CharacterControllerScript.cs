using Mono.Cecil;
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
    private float lastShot = -100f;
    private float flashDuration = 0.1f;
    private LineRenderer lineRenderer;
    private Camera _camera;
    private GameObject gunFlashSpot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _camera = Camera.main;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        
        lineRenderer.SetPosition(0, new Vector3(0, 100, 0));
        lineRenderer.SetPosition(1, new Vector3(0, 100, 0));
        cc = GetComponent<CharacterController>();
        CacheGunFlashSpot();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGun();
        HandleMouseLook();
        HandleMovement();
        HandleWorldSwitching();
    }

    private void CacheGunFlashSpot()
    {
        for (int i = 0; i < _camera.transform.childCount; i++)
        {
            var child = _camera.transform.GetChild(i);
            if (child.CompareTag("Gun"))
            {
                for (int j = 0; j < child.childCount; j++)
                {
                    var gunChild = child.GetChild(j);
                    if (gunChild.name.Equals("ParticleSpot"))
                    {
                        gunFlashSpot = gunChild.gameObject;
                        return;
                    }
                }
            }
        }
        gunFlashSpot.SetActive(true);
        Material yellowMat = new Material(Shader.Find("Unlit/Color"));
        yellowMat.color = Color.yellow;
        gunFlashSpot.GetComponent<MeshRenderer>().material = yellowMat;
        gunFlashSpot.SetActive(false);
    }

    private void HandleGun()
    {
        if (Time.time > lastShot + flashDuration)
        {
            gunFlashSpot.SetActive(false);
        }

        lineRenderer.enabled = false;
        // Dynamically calculate gunCooldown here?
        // in seconds
        float gunCooldown = 0.33f;
        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time <= lastShot + gunCooldown) return;
            fireGun();
            lastShot = Time.time;
        }
    }
    
    private void fireGun()
    {
        gunFlashSpot.SetActive(true);
        Vector3 start = _camera.transform.position;
        Vector3 direction = _camera.transform.forward;
        Vector3 end = start + 100f * direction;
        
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        //lineRenderer.enabled = true;
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 100f, LayerMask.GetMask("Enemy"))) {
            // Hit something!
            Debug.Log("Hit: " + hit.transform.name);
            //Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); // Impact particle

            // Handle Damage (e.g., call a TakeDamage() method on hit.collider.gameObject)
            hit.collider.GetComponentInParent<Enemy>()?.takeDamage(50);

            // Bullet Trail to hit point
            //GameObject trail = Instantiate(bulletTrailPrefab, firePoint.position, Quaternion.identity);

            // Set trail to go from firePoint.position to hit.point (using its script/LineRenderer)
        } else {
            Debug.Log("Miss");
            // Hit nothing (skybox/empty space)
            // Bullet Trail to max range
            // GameObject trail = Instantiate(bulletTrailPrefab, firePoint.position, Quaternion.identity);
            // Set trail to go from firePoint.position to firePoint.position + firePoint.forward * range
        }
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
            GameManager.Get().NextWorld();
            worldLastSwitched = Time.time;
        }
    }
}
