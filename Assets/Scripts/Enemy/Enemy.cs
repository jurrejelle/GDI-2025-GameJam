using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class Enemy : MonoBehaviour
{

    public float hitpoints = 100f;

    public Transform lookAtPlayer;
    public Transform goal;
    public World world = World.Western;
    
    public bool isPhantom = false;
    
    private NavMeshAgent agent;

    private bool isDead = false;
    Coroutine damageRoutine = null;
    private MeshRenderer spriteRenderer;
    private Material frame1;
    private Material frame2;
    
    private float lastSwap = -100f;
    private float swapDelay = 0.5f;


    
    private bool isGoalSet = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frame1 = Resources.Load<Material>(EnemyManger.spritesPerWorld[world]["Walking1"]);
        frame2 = Resources.Load<Material>(EnemyManger.spritesPerWorld[world]["Walking2"]);
        
        spriteRenderer = GetComponentInChildren<MeshRenderer>();
        spriteRenderer.material = frame1;
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.baseOffset = 0.5f;
        agent.speed = 1f;
        agent.acceleration = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWalking();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (Time.time <= lastSwap + swapDelay) return;
        if (spriteRenderer.material.mainTexture == frame1.mainTexture)
        {
            spriteRenderer.material = frame2;
        }
        else
        {
            spriteRenderer.material = frame1;
        }
        lastSwap = Time.time;
    }

    private void UpdateWalking()
    {
        if (!isGoalSet)
        {
            agent.SetDestination(goal.position);
            isGoalSet = true;
        }

        Vector3 dir = lookAtPlayer.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }
    public void takeDamage(float damage)
    {
        if (isPhantom)
        {
            PoofParticles.SpawnWhite(transform.position);
            // TODO: Make visually phantom when shot
            return;
        }
        hitpoints -= damage;
        PoofParticles.SpawnRed(transform.position);
        if (hitpoints <= 0)
        {
            Destroy(gameObject);
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine);
                damageRoutine = null;
            }
            EnemyManger.Get().Kill(this);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HouseTrigger"))
        {
            if (isPhantom) return;
            damageRoutine ??= StartCoroutine(DamageOverTime());
        }
    }
    
    
    IEnumerator DamageOverTime()
    {
        while (!isDead)
        {
            GameManager.Get().DamagePlayer(10);
            yield return new WaitForSeconds(1);
        }

        damageRoutine = null;
    }
    
}
