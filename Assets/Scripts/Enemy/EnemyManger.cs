using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManger : MonoBehaviour
{

    public Dictionary<World, List<GameObject>> EnemiesPerWorld = new()
    {
        { World.Scifi, new List<GameObject>() },
        { World.Western, new List<GameObject>() },
        { World.Fantasy, new List<GameObject>() }
    };

    private static EnemyManger INSTANCE;

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
        foreach (World world in GameManager.Get().Worlds)
        {
            var Enemies = EnemiesPerWorld[world];
            Enemies.Remove(enemy.gameObject);
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        foreach (World world in GameManager.Get().Worlds)
        {
            var Enemies = EnemiesPerWorld[world];
            // At least 1 enemy in every world
            if (Enemies.Count == 0)
            {
                Vector3 start = GameManager.Get().WorldOffsets[world];
                Vector2 offsetInUnitCircle = Random.insideUnitCircle * 10;
                Vector3 offset = new Vector3(offsetInUnitCircle.x, 1f, offsetInUnitCircle.y);
                var newEnemy = Instantiate(GameManager.Get().GetCurrentEnemyPrefab(), start + offset,
                    Quaternion.identity);
                newEnemy.GetComponent<Enemy>().lookAtPlayer = GetPlayer();
                newEnemy.transform.SetParent(GetEnemyParent(), true);
                Enemies.Add(newEnemy);
            }
        }
    }
}
