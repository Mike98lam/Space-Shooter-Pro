using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    private Player _player;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    private bool _shieldActive = false;
    private int _shieldRandomizer;

    private float _fireRate = 3f;
    private float _canFire = -1;

    private float _enemyXMovement;
    private float _randomEnemyMovement;

    
    private bool _stopShooting = false;

    Animator _animator;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    private AudioSource _audioSource;

    [SerializeField]
    private int _enemyID;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _enemyXMovement = Random.Range(-9.5f, 9.6f);
        _randomEnemyMovement = Random.Range(1, 3);
        _shieldRandomizer = Random.Range(0, 2);
        if (_shieldRandomizer == 1)
        {
            _shieldActive = true;
            _shieldVisualizer.SetActive(true);
        }
        
        if (_player == null)
        {
            Debug.LogError("Couldn't find the Player");
        }
        
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Couldn't find the Animator");
        }

    }

    void Update()
    {
        CalculateMovement();

        StartCoroutine(laserFire());
    }

    void CalculateMovement()
    {
        
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        
        if (transform.position.y <= -5.5f)
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }

        if (_enemyID == 1)
        {
            if (transform.position.y <= 4f && transform.position.x <= _enemyXMovement)
            {
                transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime);
            }

            if (transform.position.y <= 4f && transform.position.x >= _enemyXMovement)
            {
                transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime);
            }
            
        }
        
    }

    IEnumerator laserFire()
    {
        yield return new WaitForSeconds(Random.Range(0, 2));
        while (_stopShooting == false)
        {
            if (_enemyID == 0)
            {
                if (Time.time > _canFire)
                {
                    _fireRate = Random.Range(3f, 7f);
                    _canFire = Time.time + _fireRate;
                    GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                    Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                    for (int i = 0; i < lasers.Length; i++)
                    {
                        lasers[i].AssignEnemyLaser();
                    }
                }
                yield return new WaitForSeconds(0f);
            }

            if (_enemyID == 1)
            {
                if (Time.time > _canFire)
                {
                    _fireRate = Random.Range(5f, 10f);
                    _canFire = Time.time + _fireRate;
                    GameObject enemyBeam = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                    Laser[] lasers = enemyBeam.GetComponentsInChildren<Laser>();

                    for (int i = 0; i < lasers.Length; i++)
                    {
                        lasers[i].AssignEnemyLaser();
                    }

                }
            }
            yield return new WaitForSeconds(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            
            Player player = other.transform.GetComponent<Player>();
           if (player != null)
            {
                player.Damage();
            }

           if (_shieldActive == false)
            {
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0;
                _audioSource.Play();
                _stopShooting = true;
                Destroy(GetComponent<Collider2D>());
                _spawnManager.EnemyReduce();
                Destroy(gameObject, 2.8f);
            }

           if (_shieldActive == true)
            {
                _shieldActive = false;
                _shieldVisualizer.SetActive(false);
            }
            
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null && _shieldActive == false)
            {
                _player.AddScore(10);
            }

            if (_shieldActive == false)
            {
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0;
                _audioSource.Play();
                _stopShooting = true;
                Destroy(GetComponent<Collider2D>());
                _spawnManager.EnemyReduce();
                Destroy(gameObject, 2.8f);
            }
            
            if (_shieldActive == true)
            {
                _shieldActive = false;
                _shieldVisualizer.SetActive(false);
            }
        }
       
    }
}
