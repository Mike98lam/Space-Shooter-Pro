using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 7f;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private float _machineFireRate = 0.1f;

    [SerializeField]
    private int _lives = 3;
    [SerializeField] // 3 is green shield, 2 is blue shield, 1 is red shield
    private int _shieldStrength = 3;
    
    [SerializeField]
    private long _ammoCount = 15;

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

    [SerializeField]
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _speedUpActive = false;
    [SerializeField]
    private bool _shieldsActive = false;
    [SerializeField]
    private bool _machineFireActive = false;

    [SerializeField]
    private Slider _thrusterSlider;
    [SerializeField]
    private float _maxGauge = 100;
    [SerializeField]
    private bool _thrustersActive = false;
    [SerializeField]
    private GameObject _thrusters;

    [SerializeField]
    private bool _skullActive = false;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    private CameraShake _mainCamera;
    void Start()
    {

        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thrusters.transform.localPosition = new Vector3(-0.017f, -2.5f, 0);
        _thrusters.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        _thrusterSlider = GameObject.Find("Canvas").GetComponentInChildren<Slider>();
        _mainCamera = GameObject.Find("Main Camera").GetComponent<CameraShake>();

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

        if (_thrusterSlider == null)
        {
            Debug.LogError("The Slider is NULL");
        }

        if (_mainCamera == null)
        {
            Debug.LogError("The Main Cmera is NULL");
        }

        else
        {
            _audioSource.clip = _laserAudio;
        }
    }


    void Update()
    {
        CalculateMovement();
        
        if (_machineFireActive == true)
        {
            if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
            {
                _canFire = Time.time + _machineFireRate;
                LaserFire();
            } 
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            {
                _canFire = Time.time + _fireRate;
                LaserFire();
            }
        }
        
    }

    void CalculateMovement()
    {
        if (_skullActive == false)
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

            if (Input.GetKeyDown(KeyCode.LeftShift) && _maxGauge >= 0)
            {
                ThrusterOn();
                _thrusters.transform.localPosition = new Vector3(-0.017f, -3.5f, 0);
                _thrusters.transform.localScale = new Vector3(1, 1, 0);
                StartCoroutine(ThrusterRoutine());

                if (_maxGauge <= 0)
                {
                    ThrusterOff();
                    _thrusters.transform.localPosition = new Vector3(-0.017f, -2.5f, 0);
                    _thrusters.transform.localScale = new Vector3(0.5f, 0.5f, 0);
                }

            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ThrusterOff();
                _thrusters.transform.localPosition = new Vector3(-0.017f, -2.5f, 0);
                _thrusters.transform.localScale = new Vector3(0.5f, 0.5f, 0);
                StartCoroutine(ThrusterRegenerationRoutine());
            }
            _thrusterSlider.value = _maxGauge;

        }
    }

       

    void LaserFire()
    {
        
        if (_ammoCount != 0)
        {

            if (_tripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
            }
            _audioSource.Play();
            if (_machineFireActive == false)
            {
                AmmoLoss();
            }
        }

        else if (_ammoCount == 0)
        {
            AmmoZero();
        }
    }

    public void Damage()
    {
        if (_shieldsActive == true)
        {
            _shieldStrength--;

            switch (_shieldStrength)
            {
                case 0:
                    _shieldsActive = false;
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

        _lives--;
        _mainCamera.CameraMovement();
        _uiManager.UpdateLives(_lives);

        if (_lives <= 2)
        {
            _leftEngineDmg.SetActive(true);

        }
        if (_lives <= 1)
        {
            _rightEngineDmg.SetActive(true);
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
            _speed = 14;
        }
        StartCoroutine(SpeedUpPowerDownRoutine());
    }

    IEnumerator SpeedUpPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speedUpActive = false;
        _speed = 7f;
    }

    public void ShieldsActive()
    {
        _shieldStrength = 3;
        _shieldVisualizer.transform.GetComponent<SpriteRenderer>().color = Color.green;
        _shieldsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AmmoLoss()
    {
        _ammoCount--;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void AmmoZero()
    {
        _uiManager.AmmoEmpty();
    }

    public void AmmoCrate()
    {
        if (_machineFireActive == false)
        {
            _ammoCount = 15;
            _uiManager.UpdateAmmo(_ammoCount);
        }
    }

    public void Heal()
    {
        if (_lives <3)
        {
            _lives++;
            if (_lives == 3)
            {
                _leftEngineDmg.SetActive(false);
            }
            if (_lives >= 2)
            {
                _rightEngineDmg.SetActive(false);
            }
        }
        
        _uiManager.UpdateLives(_lives);
    }

    public void MachineFire()
    {
        _machineFireActive = true;
        _ammoCount = 999999999999999;
        _uiManager.AmmoInfinite();
        StartCoroutine(MachineFirePowerDownRoutine());

    }

    IEnumerator MachineFirePowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _machineFireActive = false;
        _ammoCount = 15;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    private void ThrusterOn()
    {
        _thrustersActive = true;
        if (_speedUpActive == false)
        {
            _speed = 11;
        }
        else if ( _speedUpActive == true)
        {
            _speed = 18;
        }
        
    }

    private void ThrusterOff()
    {
        _thrustersActive = false;
        if (_speedUpActive == false)
        {
            _speed = 7;
        }
        else if (_speedUpActive == true)
        {
            _speed = 14;
        }
        
    }

    IEnumerator ThrusterRoutine()
    {
        while (_thrustersActive == true && _maxGauge >= 0)
        {
            _maxGauge -= 0.5f;
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator ThrusterRegenerationRoutine()
    {
        yield return new WaitForSeconds(1);
        while (_thrustersActive == false && _maxGauge <= 100)
        {
            _maxGauge += 2;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void Skull()
    {
        _skullActive = true;
        StartCoroutine(SkullPowerDown());
    }

    IEnumerator SkullPowerDown()
    {
        yield return new WaitForSeconds(3);
        _skullActive = false;
    }
}