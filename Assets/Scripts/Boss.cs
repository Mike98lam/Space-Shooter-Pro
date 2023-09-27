using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 2f;
    [SerializeField]
    private GameObject _rightEngineDmg;
    [SerializeField]
    private GameObject _leftEngineDmg;
    [SerializeField]
    private GameObject _backRightEngineDmg;
    [SerializeField]
    private GameObject _backLeftEngineDmg;
    [SerializeField]
    private GameObject _laserPrefab_1;
    [SerializeField]
    private GameObject _laserPrefab_2;
    [SerializeField]
    private int _lives = 5;
    [SerializeField]
    private bool _shieldActive;
    [SerializeField]
    private int _shieldStrength = 3;
    [SerializeField]
    private float _canFire = -1;
    [SerializeField]
    private float _fireRate = 3f;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private Player _player;
    private AudioSource _audioSource;
    private float _enemyXMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _shieldActive = true;
        _shieldVisualizer.SetActive(true);
        _audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            _enemyXMovement = _player.transform.position.x;
        }
        
        CalculateMovement();
        StartCoroutine(LaserFire());
    }

    void CalculateMovement()
    {
       
        if (transform.position.y >= 3.5f)
        {
            transform.Translate(new Vector3(0, -1, 0) * _enemySpeed * Time.deltaTime, Space.World);
           
        }
        if (transform.position.x <= _enemyXMovement)
        {
            transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime, Space.World);
        }

        if (transform.position.x >= _enemyXMovement)
        {
            transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime, Space.World);
        }


    }

    public void Damage()
    {
        _lives--;
       
        if (_lives <= 4)
        {
            _leftEngineDmg.SetActive(true);

        }
        if (_lives <= 3)
        {
            _rightEngineDmg.SetActive(true);
        }
        if (_lives <= 2)
        {
            _backLeftEngineDmg.SetActive(true);
        }
        if (_lives <= 1)
        {
            _backRightEngineDmg.SetActive(true);
        }
        if (_lives == 0)
        {
            _spawnManager.OnBossDeath();
            _player.AddScore(500);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyReduce();
            _uiManager.GameCompleteSequence();
            Destroy(gameObject, .25f);
        }
    }

    IEnumerator LaserFire()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(1f, 4f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab_1, transform.position, Quaternion.identity);
            GameObject enemyLaser2 = Instantiate(_laserPrefab_2, transform.position, Quaternion.identity);

            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            Laser[] lasers2 = enemyLaser2.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

            for (int x = 0; x < lasers2.Length; x++)
            {
                lasers2[x].AssignEnemyLaser();
            }
        }
        yield return new WaitForSeconds(0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Damage();
            }

            if (_shieldActive == true)
            {
                _shieldStrength--;

                switch (_shieldStrength)
                {
                    case 0:
                        _shieldActive = false;
                        _shieldVisualizer.SetActive(false);
                        break;

                    case 1:
                        _shieldVisualizer.transform.GetComponent<SpriteRenderer>().color = Color.red;
                        break;

                    case 2:
                        _shieldVisualizer.transform.GetComponent<SpriteRenderer>().color = Color.white;
                        break;
                }
                return;
            }
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null && _shieldActive == false)
            {
                Damage();
                if (_lives > 0)
                {
                    _audioSource.Play();
                }
            }

            if (_shieldActive == true)
            {
                _shieldStrength--;

                switch (_shieldStrength)
                {
                    case 0:
                        _shieldActive = false;
                        _shieldVisualizer.SetActive(false);
                        break;

                    case 1:
                        _shieldVisualizer.transform.GetComponent<SpriteRenderer>().color = Color.red;
                        break;

                    case 2:
                        _shieldVisualizer.transform.GetComponent<SpriteRenderer>().color = Color.white;
                        break;

                }
                return;

            }
        }

    }
}



