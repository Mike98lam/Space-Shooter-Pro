using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField] //ID for Powerups  0 = tripleshot  1 = speed  2 = shields 3 = ammo 4 = healthpack
    private int _powerUpID;
    [SerializeField]
    private AudioClip _powerUpAudio;
    [SerializeField]
    private AudioClip _explosion;
    [SerializeField]
    private GameObject _explosionPrefab;
    private Player _player;
    private float _collectSpeed = 6f;
   
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }
    void Update()
    {
        
        transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        
        if (transform.position.y <= -7f)
        {
            Destroy(gameObject);
        }

        if (Input.GetKey("c"))
        {
            if (_player != null)
            {
                Vector3 direction = this.transform.position - _player.transform.position;
                direction = direction.normalized;
                this.transform.position -= direction * Time.deltaTime * _collectSpeed;
            }
            
        }
      
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_powerUpAudio, transform.position);

            if (player != null)
            {
                switch(_powerUpID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedUpActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoCrate();
                        break;
                    case 4:
                        player.Heal();
                        break;
                    case 5:
                        player.MachineFire();
                        break;
                    case 6:
                        player.Skull();
                        break;
                    case 7:
                        player.HomingLaser();
                        break;
                }
            }
            Destroy(gameObject);
        }

        if (other.tag == "Enemy Laser" && this.tag == "PowerUp")
        {
           
            AudioSource.PlayClipAtPoint(_explosion, transform.position);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject);
        }
    }
}
