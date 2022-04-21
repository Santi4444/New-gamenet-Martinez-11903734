using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiringCode : MonoBehaviour
{
    //mono
    public GameObject Bullet;
    public int speed;
    public Transform Barrel;
    public int DestroyBullet;


    public float fireRate = 1f;
    private float nextFire = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject Projectile = Instantiate(Bullet, Barrel.transform.position, Barrel.rotation);
            Projectile.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            Destroy(Projectile, DestroyBullet);

        }


        


    }
}
