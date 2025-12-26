using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManger : MonoBehaviour
{

    public List<GameObject> Enemies;
    public static EnemyManger INSTANCE;

    private Transform player;
    private Transform enemyParent;

    private Transform GetPlayer()
    {
        if (player == null)
        {
            player = GameManager.Get().getPlayer().transform;
        }
        return player;
    }
    
    private Transform GetEnemyParent()
    {
        if (enemyParent == null)
        {
            enemyParent = GameObject.Find("/Enemies").transform;
        }
        return enemyParent;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        INSTANCE = this;
    }

    public static EnemyManger Get()
    {
        return INSTANCE;
    }

    public void Kill(Enemy enemy)
    {
        Enemies.Remove(enemy.gameObject);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Enemies.Count == 0)
        {
            Vector3 start = GameManager.Get().GetCurrentWorldPosition();
            Vector2 offsetInUnitCircle = Random.insideUnitCircle * 10;
            Vector3 offset =  new Vector3(offsetInUnitCircle.x, 1f, offsetInUnitCircle.y);
            var newEnemy = Instantiate(GameManager.Get().GetCurrentEnemyPrefab(), start + offset, Quaternion.identity);
            newEnemy.GetComponent<Enemy>().lookAtPlayer = GetPlayer();
            newEnemy.transform.SetParent(GetEnemyParent(), true);
            Enemies.Add(newEnemy);
        }
    }
}
