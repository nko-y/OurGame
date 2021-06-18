using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class timeBullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    // Bullet Towards Time
    public int towards;

    // Players
    GameObject theRival;

    void Update()
    {
        chechAttack();
    }

    void chechAttack()
    {

    }


    void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }

        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
