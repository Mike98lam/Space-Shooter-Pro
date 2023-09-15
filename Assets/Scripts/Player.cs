using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10f;
    private float _speedMultiplier = 2f;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    
    [SerializeField]
    private Vector3 _offset = new Vector3(0, 1, 0);

    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngineDmg;
    [SerializeField]
    private GameObject _leftEngineDmg;
    
    [SerializeField]
    private AudioClip _laserAudio;
    private AudioSource _audioSource;
    //variable to store audio clip

    [SerializeField]
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _speedUpActive = false;
    [SerializeField]
    private bool _shieldsActive = false;


    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    void Start()
    {

        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the player is NULL");
        }    
        else
        {
            _audioSource.clip = _laserAudio;
        }
    }


    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            LaserFire();
        }

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        
        transform.Translate(direction * _speed * Time.deltaTime);
       
        

        if (transform.position.x >= 11.26f)
        {
            transform.position = new Vector3(-11.26f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.26f)
        {
            transform.position = new Vector3(11.26f, transform.position.y, 0);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.7f, 4.7f), 0);
    }

    void LaserFire()
    {

        _canFire = Time.time + _fireRate;
   
       

        if (_tripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        { 
            Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
        }

        _audioSource.Play();//play the laser audio clip

    }

    public void Damage()
    {
        //if shields is active
        //do nothing
        //deactivate shields
        // return;
        if (_shieldsActive == true)
        {
            _shieldsActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;

        _uiManager.UpdateLives(_lives);

        if (_lives <= 2)
        {
            _rightEngineDmg.SetActive(true);

        }
        if (_lives <= 1)
        {
            _leftEngineDmg.SetActive(true);
        }


        if (_lives == 0)
        {
            _spawnManager.OnPlayerDeath();    
            Destroy(gameObject);
        }

    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());

    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotActive = false;
    }

    public void SpeedUpActive()
    {
        _speedUpActive = true;
        if (_speedUpActive == true)
        {
            _speed *= _speedMultiplier;
        }
        StartCoroutine(SpeedUpPowerDownRoutine());
    }
    
    IEnumerator SpeedUpPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speedUpActive = false;
        _speed /= _speedMultiplier;
    }
    
    public void ShieldsActive()
    {
        _shieldsActive = true;
        _shieldVisualizer.SetActive(true);

    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    //method to add 10 to score
    //communicate with UI manager to update score
}
