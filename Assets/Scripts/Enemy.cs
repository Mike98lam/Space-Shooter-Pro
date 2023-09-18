using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    private Player _player;

    [SerializeField]
    private GameObject _laserPrefab;

    private float _fireRate = 3f;
    private float _canFire = -1;

    private bool _stopShooting = false;

    Animator _animator;

    private AudioSource _audioSource;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
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
    }

    IEnumerator laserFire()
    {
        yield return new WaitForSeconds(Random.Range(0, 2));
        while (_stopShooting == false)
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
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            _stopShooting = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            _stopShooting = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.8f);
        }
    }
}
