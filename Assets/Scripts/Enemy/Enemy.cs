using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float hitpoints = 100f;

    public Transform lookAtPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = lookAtPlayer.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }

    public void takeDamage(float damage)
    {
        Debug.Log("hit!");
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            Debug.Log("dying!");
            Destroy(gameObject);
            EnemyManger.Get().Kill(this);
        }
    }
}
