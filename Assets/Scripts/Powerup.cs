using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField] //ID for Powerups  0 = tripleshot  1 = speed  2 = shields 3 = ammo 4 = healthpack
    private int _powerUpID;
    [SerializeField]
    private AudioClip _powerUpAudio;

    void Update()
    {
        
        transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        
        if (transform.position.y <= -7f)
        {
            Destroy(gameObject);
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
                }
            }
            Destroy(gameObject);
        }
    }
}
