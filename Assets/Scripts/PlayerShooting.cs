using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerShooting : MonoBehaviour
{
    public GameObject shootingPoint;
    public AudioSource shootSound;
    private Animator _animator;
    public int bulletsCount;
    public ParticleSystem fireEffect;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        shootSound = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.timeScale > 0)
        {
            if (bulletsCount > 0)
            {
                Fire();
            }
        }
        else
        {
            _animator.SetBool("Shot Bullet Bool", false);
        }
    }

    void Fire()
    {
        _animator.SetBool("Shot Bullet Bool",true);
        GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject();
        bullet.layer = LayerMask.NameToLayer("Player Bullet");
        bullet.transform.position = shootingPoint.transform.position;
        bullet.transform.rotation = shootingPoint.transform.rotation;
        bullet.SetActive(true);
        fireEffect.Play();
        shootSound.Play();
        bulletsCount--;
        if (bulletsCount < 0)
        {
            bulletsCount = 0;
        }
    }
}
