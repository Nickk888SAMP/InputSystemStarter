using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{

    [SerializeField] private float shootRate = 0.1f;
    
    private AudioSource shootAudioSource;
    private bool isShooting = false;
    private float shootTimer = 0f;

    private void Awake() 
    {
        shootAudioSource = GetComponent<AudioSource>();    
    }

    private void Update() 
    {
        if(isShooting)
        {
            shootTimer += Time.deltaTime;
            if(shootTimer > shootRate)
            {
                shootTimer = 0;
                shootAudioSource.Play();
            }
        }
        else
        {
            shootTimer = 0;
        }
    }

    public void SetShooting(bool set)
    {
        if(isShooting != set)
        {
            shootTimer = shootRate;
        }
        isShooting = set;
        
    }
}
