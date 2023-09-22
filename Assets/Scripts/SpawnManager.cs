using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _enemySpawnRate = 4f;
    [SerializeField]
    private int _waveNum = 1;
    [SerializeField]
    private int _enemyNum;
    [SerializeField]
    private int _enemyRemaining;
 

    private UIManager _uiManager;

    private bool _playerAlive = true;
    private bool _stopSpawning = false;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("Couldn't find UI Manager");
        }
    }
    public void StartSpawning()
    {
       
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void EnemyReduce()
    {
        _enemyRemaining--;
        _uiManager.EnemyRemaining(_enemyRemaining);
    }

    public void NewWave()
    {
        _waveNum++;
        _uiManager.UpdateWave(_waveNum);
        _stopSpawning = false;
        StartCoroutine(SpawnEnemyRoutine());
    }
    IEnumerator SpawnEnemyRoutine()
    {
       
        _enemyNum = 5 * _waveNum;
        _enemyRemaining = _enemyNum;
        _uiManager.EnemyRemaining(_enemyRemaining);
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            if (_enemyRemaining > 0)
            {
                int randomEnemy = Random.Range(0, 2);
                Vector3 posToSpawn = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
                GameObject newEnemy = Instantiate(_enemyPrefab[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnRate);

            }
            else
            {
                _stopSpawning = true;
            }
        }
        if (_stopSpawning == true && _playerAlive == true)
        {
            Destroy(GameObject.FindWithTag("Enemy"));
            NewWave();
        }
    }
    
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawnPowerup = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
            int chanceToSpawn = Random.Range(0, 100);

            if (chanceToSpawn >= 0 && chanceToSpawn < 15) 
            {
                Instantiate(_powerUps[0], posToSpawnPowerup, Quaternion.identity); // tripleshot
            }
            if (chanceToSpawn >= 15 && chanceToSpawn < 30) 
            {
                Instantiate(_powerUps[1], posToSpawnPowerup, Quaternion.identity); // speedup
            }
            if (chanceToSpawn >= 30 && chanceToSpawn < 45) 
            {
                Instantiate(_powerUps[2], posToSpawnPowerup, Quaternion.identity); // shield
            }
            if (chanceToSpawn >= 45 & chanceToSpawn < 50) 
            {
                Instantiate(_powerUps[4], posToSpawnPowerup, Quaternion.Euler(-90, 0, 0)); // heal
            }
            if (chanceToSpawn >= 50 && chanceToSpawn < 55) 
            {
                Instantiate(_powerUps[5], posToSpawnPowerup, Quaternion.Euler(0, 90, 0)); // machinefire
            }
            if (chanceToSpawn >= 55 && chanceToSpawn <70) 
            {
                Instantiate(_powerUps[6], posToSpawnPowerup, Quaternion.Euler(0, 180, 0)); // skull debuff
            }
            if (chanceToSpawn >= 70)
            {
                Instantiate(_powerUps[3], posToSpawnPowerup, Quaternion.Euler(0, 90, 0)); // ammo
            }
            yield return new WaitForSeconds(Random.Range(3, 8));
        // Ammo 30% spawn, tripleshot 15% spawn, speedup 15% spawn, shield 15% spawn, heal 5% spawn, skull 15% spawn, machinefire 5% spawn
        }
    }

    
    public void OnPlayerDeath()
    {
        _playerAlive = false;
        _stopSpawning = true;
    }
}