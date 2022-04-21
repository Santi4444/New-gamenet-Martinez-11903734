using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiringProjectileCode : MonoBehaviour
{
    public GameObject Bullet;
    public int speed;
    public Transform Barrel;
    public int DestroyBullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Bullet == null)
        {
            Debug.Log("No ammo");
        }
        else
        { 
            if(Input.GetButtonDown("Fire1"))
            {
                GameObject Projectile = Instantiate(Bullet, Barrel.transform.position, Barrel.rotation);
                Projectile.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                Destroy(Projectile, DestroyBullet);
            }
        }


    }
}
