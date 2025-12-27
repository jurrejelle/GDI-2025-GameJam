using System.Collections.Generic;
using UnityEngine;

public class EnemyManger : MonoBehaviour
{

    public Dictionary<World, List<GameObject>> EnemiesPerWorld = new()
    {
        { World.Scifi, new List<GameObject>() },
        { World.Western, new List<GameObject>() },
        { World.Fantasy, new List<GameObject>() }
    };

    private Dictionary<World, Transform> _goalsPerWorld;
    public Dictionary<World, Transform> GoalsPerWorld => _goalsPerWorld ??= new()
    {
        { World.Scifi, GameObject.Find("/Goals/World_1").transform},
        { World.Western, GameObject.Find("/Goals/World_2").transform},
        { World.Fantasy, GameObject.Find("/Goals/World_3").transform},
    };

    private static EnemyManger INSTANCE;

    private Transform _enemyParent;
    private Transform EnemyParent => _enemyParent ??= GameObject.Find("/Enemies").transform;
    
    
    private Transform _player;
    private Transform Player => _player ??= GameManager.Get().getPlayer().transform;
    
    private GameManager _gameManager;
    private GameManager GameManagerCache => _gameManager ??= GameManager.Get();

    private float lastEnemySpawn = -100f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        INSTANCE = this;
    }

    public static EnemyManger Get()
    {
        return INSTANCE;
    }

    public void NextWave()
    {
        lastEnemySpawn = -100f;
    }

    public void Kill(Enemy enemy)
    {
        foreach (World world in GameManager.Worlds)
        {
            var Enemies = EnemiesPerWorld[world];
            Enemies.Remove(enemy.gameObject);
        }
        GameManagerCache.playerKilledEnemy();

    }
    
    // Update is called once per frame
    void Update()
    {
        
        if (Time.time <= lastEnemySpawn + GameManagerCache.spawnDelay) return;
        lastEnemySpawn = Time.time;
        foreach (World world in GameManager.Worlds)
        {
            if (GameManagerCache.ShouldStillSpawnEnemy())
            {
                SpawnEnemy(world);
                GameManagerCache.EnemySpawned();
            }
        }

    }

    private void SpawnEnemy(World world)
    {
        var Enemies = EnemiesPerWorld[world];
        Vector3 start = GameManager.WorldOffsets[world];
        Vector2 offsetInUnitCircle = Random.insideUnitCircle * 10;
        Vector3 offset = new Vector3(offsetInUnitCircle.x, 1f, offsetInUnitCircle.y);
        var newEnemyObject = Instantiate(GameManagerCache.GetCurrentEnemyPrefab(), start + offset,
            Quaternion.identity);
        var newEnemy = newEnemyObject.GetComponent<Enemy>();
        newEnemy.lookAtPlayer = Player;
        newEnemy.goal = GoalsPerWorld[world];
        newEnemyObject.transform.SetParent(EnemyParent, true);
        Enemies.Add(newEnemyObject);
    }

    public int TotalEnemiesAlive()
    {
        int total = 0;
        foreach (World world in GameManager.Worlds)
        {
            total += EnemiesPerWorld[world].Count;
        }

        return total;
    }
}
