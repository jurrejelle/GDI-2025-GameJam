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
        _SetWorld(world);
    }

    public World nextWorld()
    {
        int currentIndex = Worlds.IndexOf(currentWorld);
        int newIndex = (currentIndex + 1) % totalWorlds;
        World newWorld = Worlds[newIndex];
        _SetWorld(newWorld);
        return newWorld;
    }

    private void _SetWorld(World newWorld)
    {
        Debug.Log("Setting new world to " + WorldNames[newWorld]);
        currentWorld = newWorld;
        // TODO: Disable all non-world instances, enable all world instances
        UpdateWorldObjects();
    }

    public World getWorld()
    {
        return currentWorld;
    }

    private void UpdateWorldObjects()
    {
        
        // Find all objects that are a child of the "GameStage" node
        foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>()
                     .Where(go => go.transform.IsChildOf(gameStageNode.transform)))
        {
            // Enable everything with our current world tag
            if (gameObject.CompareTag(WorldTags[currentWorld]))
            {
                gameObject.SetActive(true);
            }
            else
            {
                // Disable everything without our current world tag
                foreach (World world in Worlds)
                {
                    if (world == currentWorld) continue;
                    if (gameObject.CompareTag(WorldTags[world]))
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        };
    }

}
