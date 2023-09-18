using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private GameObject _enemyContainer;
   
    private bool _stopSpawning = false;
    
    public void StartSpawning()
    {
       
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }
   
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
            
        }
    }
    
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawnPowerup = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
            int randomPowerUp = Random.Range(0, 6);
            int _chanceToSpawn = Random.Range(0, 10);
            if (randomPowerUp == 5 && _chanceToSpawn >= 5)
            {
                Instantiate(_powerUps[randomPowerUp], posToSpawnPowerup, Quaternion.Euler(0, 90, 0));
            }
            if (randomPowerUp == 3)
            {
                Instantiate(_powerUps[randomPowerUp], posToSpawnPowerup, Quaternion.Euler(0, 90, 0));
            }

            if (randomPowerUp == 4)
            {
                Instantiate(_powerUps[randomPowerUp], posToSpawnPowerup, Quaternion.Euler(-90, 0, 0));
            }

            else if (randomPowerUp == 0 || randomPowerUp == 1 || randomPowerUp == 2)
            {
                Instantiate(_powerUps[randomPowerUp], posToSpawnPowerup, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}