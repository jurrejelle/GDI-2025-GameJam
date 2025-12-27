using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager INSTANCE;
    private World currentWorld = World.Scifi;
    private int totalWorlds = 3;
    private int playerKills = 0;
    private GameObject gameStageNode;
    public List<World> Worlds;
    
    
    public float playerHealth = 0f;
    public float totalHealth = 100f;
    
    
    private Camera _camera;
    private TextMeshProUGUI topLeftText;
    public readonly Dictionary<World, string> WorldTags = new()
    {
        { World.Scifi, "World1" },
        { World.Western, "World2" },
        { World.Fantasy, "World3" }
    };    
    
    public readonly Dictionary<World, string> WorldNames = new()
    {
        { World.Scifi, "World1" },
        { World.Western, "World2" },
        { World.Fantasy, "World3" }
    };    
    
    public readonly Dictionary<World, Vector3> WorldOffsets = new()
    {
        { World.Scifi, new Vector3(0f,0f,0f) },
        { World.Western, new Vector3(500f,0f,0f) },
        { World.Fantasy, new Vector3(1000f,0f,0f) }
    };

    public Dictionary<World, GameObject> WorldEnemies;
    
    private void Start()
    {
        _camera = Camera.main;

        foreach( TextMeshProUGUI gui in _camera.GetComponentsInChildren<TextMeshProUGUI>() )
        {
            if (gui.name == "Text_TopLeft")
            {
                topLeftText = gui;
            }
        }
        WorldEnemies = new();
        // All same enemies for now
        WorldEnemies.Add(World.Scifi, Resources.Load("Prefabs/Enemy_1") as GameObject);
        WorldEnemies.Add(World.Western, Resources.Load("Prefabs/Enemy_1") as GameObject);
        WorldEnemies.Add(World.Fantasy, Resources.Load("Prefabs/Enemy_1") as GameObject);
        gameStageNode = GameObject.Find("GameStage");
        Worlds = Enum.GetValues(typeof(World))
            .Cast<World>()
            .ToList();
        playerHealth = totalHealth;
        playerKills = 0;
        INSTANCE = this;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        topLeftText.text = "<color=green>Health: " + playerHealth + "/" + totalHealth + "</color>\n" + 
                           "<color=red>Kills: " + playerKills + "</color>";;
    }

    public static GameManager Get()
    {
        return INSTANCE;
    }

    public void SetWorld(World world)
    {
        _SetWorld(currentWorld, world);
    }

    public World NextWorld()
    {
        World oldWorld = currentWorld;
        int currentIndex = Worlds.IndexOf(currentWorld);
        int newIndex = (currentIndex + 1) % totalWorlds;
        World newWorld = Worlds[newIndex];
        _SetWorld(oldWorld, newWorld);
        return newWorld;
    }

    private void _SetWorld(World oldWorld, World newWorld)
    {
        Debug.Log("Setting new world to " + WorldNames[newWorld]);
        var player = getPlayer();
        CharacterController cc = player.GetComponent<CharacterController>();
        
        
        var oldWorldPosition = WorldOffsets[oldWorld];
        var newWorldPosition = WorldOffsets[newWorld];
        var oldPlayerPosition = player.transform.position;
        var playerPositionRelativeToZero = oldPlayerPosition - oldWorldPosition;
        var newPlayerPosition = playerPositionRelativeToZero + newWorldPosition;
        

        // Hack to force set player position
        cc.enabled = false;
        player.transform.position = newPlayerPosition;
        cc.enabled = true;
        
        currentWorld = newWorld;
    }

    public World GetWorld()
    {
        return currentWorld;
    }

    public Vector3 GetCurrentWorldPosition()
    {
        return WorldOffsets[currentWorld];
    }

    public GameObject GetCurrentEnemyPrefab()
    {
        return WorldEnemies[currentWorld];
    }

    public GameObject getPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            throw new InvalidOperationException("Player not found");
        }

        return player;
    }

    public void DamagePlayer(int amount)
    {
        playerHealth -= amount;
        Debug.Log("Damage taken: " + amount);
        Debug.Log("Player health: " + playerHealth);
        if (playerHealth <= 0)
        {
            Debug.Log("You lose!");
        }
    }

    public void playerKilledEnemy()
    {
        playerKills++;
    }
    
    

}
