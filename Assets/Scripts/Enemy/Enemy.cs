using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class Enemy : MonoBehaviour
{

    public float hitpoints = 100f;

    public Transform lookAtPlayer;
    public Transform goal;

    private NavMeshAgent agent;

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
            EnemyManger.Get().Kill(this);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision entered");
        if (other.gameObject.CompareTag("HouseTrigger"))
        {
            Debug.Log("YOU DIE!!!!!");
        }
    }
}
