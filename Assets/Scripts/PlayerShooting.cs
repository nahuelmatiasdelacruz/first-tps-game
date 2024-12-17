using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerShooting : MonoBehaviour
{
    public GameObject shootingPoint;
    public AudioSource shootSound;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        shootSound = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animator.SetTrigger("ShotBullet");
            Fire();
        }
    }

    void Fire()
    {
        _animator.Play("Shoot_SingleShot_AR",0,0);
        GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject();
        bullet.layer = LayerMask.NameToLayer("Player Bullet");
        bullet.transform.position = shootingPoint.transform.position;
        bullet.transform.rotation = shootingPoint.transform.rotation;
        bullet.SetActive(true);
        shootSound.Play();
    }
}
