using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    private Camera _camera;
    
    // Gun stuff
    private int currentGun = 0;
    private int totalGuns = 3;
    private float[] flashDuration = { 0.1f, 0.1f, 0.2f };
    private float[] gunCooldowns = { 0.20f, 0.33f, 1.0f }; //bopper, gun, wand 
    private float[] gunDamages = { 20f, 50f, 100f };
    private int imageState = 0;
    private List<Material> gunMaterialsRest = new();
    private List<Material> gunMaterialsShooting = new();
    private List<Material> gunMaterialsShooting2 = new();
    private MeshRenderer _gunRenderer;
    private MeshRenderer GunRenderer => _gunRenderer ??= GameObject.Find("Gun").GetComponent<MeshRenderer>();
    
    // Crosshair stuff
    private Sprite[] crosshairs;
    private Image _crosshairImage;
    private Image CrosshairImage => _crosshairImage ??= GameObject.Find("Crosshair").GetComponent<Image>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _camera = Camera.main;
        cc = GetComponent<CharacterController>();
        foreach (String name in new List<String>{"Bopper", "Gun", "Wand"})
        {
            var texture = Resources.Load<Texture2D>("Images/Weapons/" + name + "_Rest");
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = texture;
            gunMaterialsRest.Add(mat);
            
            var texture1 = Resources.Load<Texture2D>("Images/Weapons/" + name + "_Shoot_1");
            Material mat1 = new Material(Shader.Find("Sprites/Default"));
            mat1.mainTexture = texture1;
            gunMaterialsShooting.Add(mat1);
            
            var texture2 = Resources.Load<Texture2D>("Images/Weapons/" + name + "_Shoot_2");
            Material mat2 = new Material(Shader.Find("Sprites/Default"));
            mat2.mainTexture = texture2;
            gunMaterialsShooting2.Add(mat2);
        }

        GunRenderer.sharedMaterial = gunMaterialsRest[currentGun];

        crosshairs = new[]
        {
            Resources.Load<Sprite>("Images/Crosshairs/Scifi"),
            Resources.Load<Sprite>("Images/Crosshairs/Western"),
            Resources.Load<Sprite>("Images/Crosshairs/Fantasy"),
        };
        CrosshairImage.sprite = crosshairs[0];
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGunSwitch();
        HandleGun();
        HandleMouseLook();
        HandleMovement();
        HandleWorldSwitching();
    }
    
    private void HandleGunSwitch()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0) // Scroll up
        {
            currentGun = (currentGun + 1) % totalGuns;
            GunRenderer.sharedMaterial = gunMaterialsRest[currentGun];
            CrosshairImage.sprite = crosshairs[currentGun];
            imageState = 0;
        }
        else if (scroll < 0) // Scroll down
        {
            currentGun--;
            if (currentGun < 0) currentGun = totalGuns - 1;
            GunRenderer.sharedMaterial = gunMaterialsRest[currentGun];
            CrosshairImage.sprite = crosshairs[currentGun];
            imageState = 0;
        }

    }
    private void HandleGun()
    {
        if (Time.time > lastShot + flashDuration[currentGun] / 2 && imageState == 1)
        {
            // Flash effects
            GunRenderer.sharedMaterial = gunMaterialsShooting2[currentGun];
            imageState = 2;
        }        
        if (Time.time > lastShot + flashDuration[currentGun] && imageState == 2)
        {
            // Flash effects
            GunRenderer.sharedMaterial = gunMaterialsRest[currentGun];
            imageState = 0;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time <= lastShot + gunCooldowns[currentGun]) return;
            fireGun();
            lastShot = Time.time;
        }
    }
    
    private void fireGun()
    {
        AudioSource.PlayClipAtPoint(MusicManager.Get().weapon_shoot, _camera.transform.position);
        if (GunRenderer.material.name != gunMaterialsShooting[currentGun].name)
        {
            GunRenderer.sharedMaterial = gunMaterialsShooting[currentGun];
            imageState = 1;
        }

        // Enable flash effects
        Vector3 start = _camera.transform.position;
        Vector3 direction = _camera.transform.forward;
        Vector3 end = start + 100f * direction;
        //lineRenderer.enabled = true;
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 100f, LayerMask.GetMask("Enemy"))) {
            // Hit something!
            //Debug.Log("Hit: " + hit.transform.name);
            //Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); // Impact particle

            // Handle Damage (e.g., call a TakeDamage() method on hit.collider.gameObject)
            hit.collider.GetComponentInParent<Enemy>()?.takeDamage(gunDamages[currentGun]);

            // Bullet Trail to hit point
            //GameObject trail = Instantiate(bulletTrailPrefab, firePoint.position, Quaternion.identity);

            // Set trail to go from firePoint.position to hit.point (using its script/LineRenderer)
        } else {
            //Debug.Log("Miss");
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
            var currentWorld = GameManager.Get().PreviousWorld();
            worldLastSwitched = Time.time;
        }
        if (Keyboard.current.gKey.isPressed)
        {
            if (Time.time <= worldLastSwitched + worldSwitchCooldown) return;
            var currentWorld = GameManager.Get().NextWorld();
            worldLastSwitched = Time.time;
        }
    }
}
