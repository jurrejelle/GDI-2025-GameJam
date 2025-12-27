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

    private Dictionary<World, Transform> _goalsPerWorld;
    public Dictionary<World, Transform> GoalsPerWorld => _goalsPerWorld ??= new()
    {
        { World.Scifi, GameObject.Find("/Goals/World_2").transform},
        { World.Western, GameObject.Find("/Goals/World_2").transform},
        { World.Fantasy, GameObject.Find("/Goals/World_2").transform},
    };

    private static EnemyManger INSTANCE;

    private Transform _enemyParent;
    private Transform EnemyParent => _enemyParent ??= GameObject.Find("/Enemies").transform;
    
    
    private Transform _player;
    private Transform Player => _player ??= GameManager.Get().getPlayer().transform;
    
    
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
                var newEnemyObject = Instantiate(GameManager.Get().GetCurrentEnemyPrefab(), start + offset,
                    Quaternion.identity);
                var newEnemy = newEnemyObject.GetComponent<Enemy>();
                newEnemy.lookAtPlayer = Player;
                newEnemy.goal = GoalsPerWorld[world];
                newEnemyObject.transform.SetParent(EnemyParent, true);
                Enemies.Add(newEnemyObject);
            }
        }
    }
}
