using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager INSTANCE;
    private World currentWorld = 0;
    private int totalWorlds = 3;
    private GameObject gameStageNode;
    public List<World> Worlds;
    public Dictionary<World, string> WorldTags = new()
    {
        { World.World1, "World1" },
        { World.World2, "World2" },
        { World.World3, "World3" }
    };    
    
    public Dictionary<World, string> WorldNames = new()
    {
        { World.World1, "World1" },
        { World.World2, "World2" },
        { World.World3, "World3" }
    };    
    
    public Dictionary<World, Vector3> WorldOffsets = new()
    {
        { World.World1, new Vector3(0f,0f,0f) },
        { World.World2, new Vector3(500f,0f,0f) },
        { World.World3, new Vector3(1000f,0f,0f) }
    };
    
    
    private void Start()
    {
        gameStageNode = GameObject.Find("GameStage");
        Worlds = Enum.GetValues(typeof(World))
            .Cast<World>()
            .ToList();
        INSTANCE = this;
    }

    public static GameManager Get()
    {
        return INSTANCE;
    }

    public void SetWorld(World world)
    {
        _SetWorld(currentWorld, world);
    }

    public World nextWorld()
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
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            throw new InvalidOperationException("Player not found");
        }
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

    public World getWorld()
    {
        return currentWorld;
    }

}
