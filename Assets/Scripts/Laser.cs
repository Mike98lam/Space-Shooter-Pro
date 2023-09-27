using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Laser : MonoBehaviour
{
 
    [SerializeField]
    private float _laserSpeed = 8.0f;
    private bool _isEnemyLaser = false;
    private bool _homingActive = false;
    private GameObject[] _activeEnemies;
    private GameObject _closestEnemy = null;
    private float _closestDistance;
    private Vector3 _currentPosition;

    private float _laserBoundsX = 10.5f;
    private float _laserBoundsY = 7.7f;

    void Start()
    {
            _closestEnemy = ClosestEnemy();
    }
    void Update()
    {
        BoundsCheck();

        if (_isEnemyLaser == true)
        {
            MoveDown();
        }

        else
        {
            MoveUp();
            if (_homingActive == true)
            {
                Homing();
            }
        }

    }

    void MoveUp()
    {
       
            transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);
        
        
        if (transform.position.y >= 7.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }

        
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (transform.position.y <= -7.5f)
        {
            Destroy(transform.parent.gameObject);

            if (transform.parent == null)
            {
                Destroy(gameObject);
            }
            

        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void AssignHomingLaser()
    {
        _homingActive = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && this.tag == "Enemy Laser")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }
    }

    public GameObject ClosestEnemy()
    {
        _activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        _closestDistance = Mathf.Infinity;
        _currentPosition = transform.position;

        foreach (GameObject enemy in _activeEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, _currentPosition);
            if (distance < _closestDistance)
            {
                _closestEnemy = enemy;
                _closestDistance = distance;
            }
        }

        return _closestEnemy;
    }

    public void Homing()
    {
        if (_closestEnemy != null)
        {
            Vector3 direction = this.transform.position - _closestEnemy.transform.position;
            direction = direction.normalized;
            this.transform.position -= direction * Time.deltaTime * _laserSpeed;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var offset = 90f;

            transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
        }

        if (_closestEnemy == null)
        {
           _closestEnemy = ClosestEnemy();
        }
    }

    private void BoundsCheck()
    {
        if (transform.position.x < -_laserBoundsX)
        {
            Destroy(gameObject);
        }

        if (transform.position.x > _laserBoundsX)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -_laserBoundsY)
        {
            Destroy(gameObject);
        }

        if (transform.position.y > _laserBoundsY)
        {
            Destroy(gameObject);
        }
    }
}
