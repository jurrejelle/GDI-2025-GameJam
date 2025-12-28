using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager INSTANCE;
    private World currentWorld = World.Scifi;
    private int totalWorlds = 3;
    private Camera _camera;
    
    // Cached UI elements
    private TextMeshProUGUI topLeftText;
    private GameObject gameOverCanvas;
    private TextMeshProUGUI gameOverText;

    public static readonly List<World> Worlds = new()
    {
        World.Scifi,
        World.Western,
        World.Fantasy
    };

    public static readonly Dictionary<World, string> WorldTags = new()
    {
        { World.Scifi, "World1" },
        { World.Western, "World2" },
        { World.Fantasy, "World3" }
    };    
    
    public static readonly Dictionary<World, string> WorldNames = new()
    {
        { World.Scifi, "World1" },
        { World.Western, "World2" },
        { World.Fantasy, "World3" }
    };    
    
    public static readonly Dictionary<World, Vector3> WorldOffsets = new()
    {
        { World.Scifi, new Vector3(0f,0f,0f) },
        { World.Western, new Vector3(500f,0f,0f) },
        { World.Fantasy, new Vector3(1000f,0f,0f) }
    };
    
    
    // Global game values
    public float playerHealth = 0f;
    public float totalHealth = 100f;
    private int playerKills = 0;
    public int currentWave = 1;
    
    // Per-wave game values
    public int enemiesLeftToSpawnThisWave = 0;
    public float spawnDelay = 3;
    private int timeUntilNextWave = -1;
    private Coroutine nextWaveRoutine;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _camera = Camera.main;

        foreach( TextMeshProUGUI gui in _camera.GetComponentsInChildren<TextMeshProUGUI>(true) )
        {
            if (gui.name == "Text_TopLeft")
            {
                topLeftText = gui;
            }

            if (gui.name == "Score")
            {
                gameOverText = gui;
            }
        }

        foreach (Canvas gui in _camera.GetComponentsInChildren<Canvas>(true))
        {
            if (gui.gameObject.name == "GameOver")
            {
                gameOverCanvas = gui.gameObject;
            }
        }
        playerHealth = totalHealth;
        playerKills = 0;
        INSTANCE = this;
        currentWave = 1;
        StartWave();
    }

    private void StartWave()
    {
        enemiesLeftToSpawnThisWave = 5 + 3 * currentWave;
        spawnDelay = 4f - currentWave * 0.2f;
        // Hacky fix to deal with EnemyManager not being present on wave 1 spawn
        if(currentWave > 1) EnemyManger.Get().NextWave();
    }

    public bool  ShouldSpawnPhantom()
    {
        // TODO: change to 3+
        return currentWave >= 3 && (new Random()).NextDouble() >= 0.25;
    }
    public bool ShouldStillSpawnEnemy()
    {
        return enemiesLeftToSpawnThisWave > 0;
    }

    public void EnemySpawned()
    {
        enemiesLeftToSpawnThisWave--;
    }

    public void CheckIfNextWaveShouldStart()
    {
        if (GetTotalEnemiesLeft() <= 0 && nextWaveRoutine == null)
        {
            nextWaveRoutine = StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    public int GetTotalEnemiesLeft()
    {
        return EnemyManger.Get().TotalEnemiesAlive() + enemiesLeftToSpawnThisWave;
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        for (int i = 5; i > 0; i--)
        {
            timeUntilNextWave = i;
            yield return new WaitForSeconds(1f);
        }
        timeUntilNextWave = -1;
        currentWave++;
        StartWave();
        nextWaveRoutine = null;
    }

    private void Update()
    {
        UpdateUI();
        CheckRestart();
    }

    private void CheckRestart()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame )
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void UpdateUI()
    {
        topLeftText.text = "<color=red>Health: " + playerHealth + "/" + totalHealth + "</color>\n" + 
                           "<color=green>Kills: " + playerKills + "</color>\n" +
                           "<color=yellow>Wave: " + currentWave +  "</color>\n" +
                           "<color=white>Enemies left: " + (enemiesLeftToSpawnThisWave + EnemyManger.Get().TotalEnemiesAlive())  +  "</color>\n";
        if (timeUntilNextWave > 0)
        {
            topLeftText.text += "<color=green>Time until next wave: " + timeUntilNextWave + "s</color>\n";
        }
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
    
    public World PreviousWorld()
    {
        World oldWorld = currentWorld;
        int currentIndex = Worlds.IndexOf(currentWorld);
        int newIndex = (currentIndex + -1 + totalWorlds) % totalWorlds;
        World newWorld = Worlds[newIndex];
        _SetWorld(oldWorld, newWorld);
        return newWorld;
    }

    private void _SetWorld(World oldWorld, World newWorld)
    {
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

    public World GetRandomWorld()
    {
        return Worlds[(int) ((new Random()).NextDouble()*100 % totalWorlds)];
        
    }

    public Vector3 GetCurrentWorldPosition()
    {
        return WorldOffsets[currentWorld];
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
        if (playerHealth <= 0)
        {
            PlayerLoses();
        }
    }


    private void PlayerLoses()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverCanvas.SetActive(true);
        gameOverText.text = "<color=red>You Lose!</color>\n" +
                            "<color=yellow>You got to wave " + currentWave + ".</color>\n" +
                            "<color=yellow>Your score was " + playerKills + ".</color>\n";
    }
    public void playerKilledEnemy()
    {
        playerKills++;
        CheckIfNextWaveShouldStart();
    }
    
    

}
