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

    private NavMeshAgent agent;

    private bool isDead = false;
    Coroutine damageRoutine = null;
    
    private bool isGoalSet = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
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
        hitpoints -= damage;
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
