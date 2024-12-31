using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy1;
    [SerializeField] private Transform enemy2;
    [SerializeField] private Transform enemy3;

    [SerializeField] private TMP_Text data;
    [SerializeField] private TMP_Text enemiesCount;

    private Vector3 initialEnemyPosition1, initialEnemyPosition2, initialEnemyPosition3;

    private int wave = 0;

    private int initHP = 3;
    private int initDamage = 2;
    private int initSpeed = 4;

    private int killCount = 0;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        initialEnemyPosition1 = enemy1.position;
        initialEnemyPosition2 = enemy2.position;
        initialEnemyPosition3 = enemy3.position;

        wave++; // 1st wave
        SpawnEnemy(enemy1, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);
        SpawnEnemy(enemy2, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);
        SpawnEnemy(enemy3, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);

        // Subscribe to enemy destruction
        EnemyAI.OnEnemyDestroyed += OnEnemyDestroyed;
        data.text = "Enemies : " + (wave * initHP) + " / " + (wave * initDamage);
        enemiesCount.text = "Enemies Killed : " + killCount;
        killCount = 0;
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        EnemyAI.OnEnemyDestroyed -= OnEnemyDestroyed;
    }

    void SpawnEnemy(Transform enemy, int HP, int speed, int damage)
    {
        GameObject e = Instantiate(enemy, enemy.position, enemy.rotation).gameObject;
        activeEnemies.Add(e);
        var enemyAI = e.GetComponent<EnemyAI>();
        enemyAI.setBulletSpeedAndDamage(speed, damage);
        enemyAI.setHP(HP);
        e.SetActive(true);
    }

    void Update()
    {
        // Check if all enemies are destroyed
        if (activeEnemies.Count == 0)
        {
            Debug.Log("Wave cleared!");
            StartNextWave();
        }
    }

    private void OnEnemyDestroyed(EnemyAI enemy)
    {
        // Remove the destroyed enemy from the list
        activeEnemies.Remove(enemy.gameObject);
        Debug.Log("Enemy destroyed! Remaining enemies: " + activeEnemies.Count);

        killCount++;
        enemiesCount.text = "Enemies Killed : " + killCount;
    }

    void StartNextWave()
    {
        data.text = "Enemies : " + (wave * initHP) + " / " + (wave * initDamage);
        wave++;
        SpawnEnemy(enemy1, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);
        SpawnEnemy(enemy2, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);
        SpawnEnemy(enemy3, wave * initHP, (wave - 1) + initSpeed, wave * initDamage);
        ///power up the player as well
        player.GetComponent<PlayerCaracter>().powerUp();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("clicked!!");
    }
}
