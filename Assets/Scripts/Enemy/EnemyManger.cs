using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManger : MonoBehaviour
{

    public Dictionary<World, List<Enemy>> EnemiesPerWorld = new()
    {
        { World.Scifi, new List<Enemy>() },
        { World.Western, new List<Enemy>() },
        { World.Fantasy, new List<Enemy>() }
    };

    private Dictionary<World, Transform> _goalsPerWorld;
    public Dictionary<World, Transform> GoalsPerWorld => _goalsPerWorld ??= new()
    {
        { World.Scifi, GameObject.Find("/Goals/World_1").transform},
        { World.Western, GameObject.Find("/Goals/World_2").transform},
        { World.Fantasy, GameObject.Find("/Goals/World_3").transform},
    };

    public static readonly Dictionary<World, Dictionary<String, String>> spritesPerWorld = new()
    {
        {
            World.Scifi, new()
            {
                { "Walking1", "Materials/Robot_Walking_1" },
                { "Walking2", "Materials/Robot_Walking_2" },
            }
        },
        {
            World.Western, new()
            {
                { "Walking1", "Materials/Cowboy_Walking_1" },
                { "Walking2", "Materials/Cowboy_Walking_2" },
            }
        },
        {
            World.Fantasy, new()
            {
                { "Walking1", "Materials/Ogre_Walking_1" },
                { "Walking2", "Materials/Ogre_Walking_2" },
            }
        }
    };

    private static EnemyManger INSTANCE;

    private Transform _enemyParent;
    private Transform EnemyParent => _enemyParent ??= GameObject.Find("/Enemies").transform;
    
    
    private Transform _player;
    private Transform Player => _player ??= GameManager.Get().getPlayer().transform;
    
    private GameManager _gameManager;
    private GameManager GameManagerCache => _gameManager ??= GameManager.Get();

    private float lastEnemySpawn = -100f;
    private GameObject EnemyPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyPrefab = Resources.Load("Prefabs/Enemy_1") as GameObject;
        INSTANCE = this;
    }

    public static EnemyManger Get()
    {
        return INSTANCE;
    }

    public void NextWave()
    {
        lastEnemySpawn = -100f;
        
        foreach (World world in GameManager.Worlds)
        {
            var Enemies = EnemiesPerWorld[world];
            foreach (Enemy enemy in Enemies)
            {
                Destroy(enemy.gameObject);
            }
            Enemies.Clear();
        }
    }

    public void Kill(Enemy enemy)
    {
        foreach (World world in GameManager.Worlds)
        {
            var Enemies = EnemiesPerWorld[world];
            Enemies.Remove(enemy);
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
                if (GameManagerCache.ShouldSpawnPhantom())
                {
                    SpawnEnemy(world, true);
                } 
                SpawnEnemy(world, false);
                GameManagerCache.EnemySpawned();
            }
        }

    }

    private void SpawnEnemy(World world, bool isPhantom)
    {
        var Enemies = EnemiesPerWorld[world];
        World skinWorld = world;
        World worldToSpawnIn = world;
        if (isPhantom)
        {
            worldToSpawnIn = GameManagerCache.GetRandomWorld();
        }
        
        
        Vector3 start = GameManager.WorldOffsets[worldToSpawnIn];
        Vector3 offset = new Vector3(Random.Range(-20, 20), 1f, 30);
        
        var newEnemyObject = Instantiate(EnemyPrefab, start + offset,
            Quaternion.identity);
        var newEnemy = newEnemyObject.GetComponent<Enemy>();
        newEnemy.lookAtPlayer = Player;
        newEnemy.world = skinWorld;
        newEnemy.goal = GoalsPerWorld[worldToSpawnIn];
        newEnemy.isPhantom = isPhantom;
        newEnemyObject.transform.SetParent(EnemyParent, true);
        Enemies.Add(newEnemy);
    }

    public int TotalEnemiesAlive()
    {
        int total = 0;
        foreach (World world in GameManager.Worlds)
        {
            foreach(Enemy enemy in EnemiesPerWorld[world])
            {
                if (!enemy.isPhantom)
                {
                    total += 1;
                }
            }
        }

        return total;
    }
}
