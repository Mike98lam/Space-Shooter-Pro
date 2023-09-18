using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionAudio;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source of Explosion is NULL");
        }
        else
        {
            _audioSource.clip = _explosionAudio;
        }

        _audioSource.Play();
        Destroy(gameObject, 3f);
    }
}
