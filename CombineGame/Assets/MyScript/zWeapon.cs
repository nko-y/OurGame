using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class zWeapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;

    public GameObject owner;
    public string Attribute;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        zPlayer zp = owner.GetComponent<zPlayer>();

        if (zp.Attribute == "Fire" && this.Attribute == "Fire" || zp.Attribute == "" && this.Attribute == "Fire" || zp.Attribute == "Fire" && this.Attribute == "")
        {
            GameObject intantBullet = PhotonNetwork.Instantiate("FireBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Water" && this.Attribute == "Water" || zp.Attribute == "" && this.Attribute == "Water" || zp.Attribute == "Water" && this.Attribute == "")
        {
            GameObject intantBullet = PhotonNetwork.Instantiate("WaterBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Wind" && this.Attribute == "Wind" || zp.Attribute == "" && this.Attribute == "Wind" || zp.Attribute == "Wind" && this.Attribute == "")
        {
            GameObject intantBullet = PhotonNetwork.Instantiate("WindBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Earth" && this.Attribute == "Earth" || zp.Attribute == "" && this.Attribute == "Earth" || zp.Attribute == "Earth" && this.Attribute == "")
        {
            GameObject intantBullet = PhotonNetwork.Instantiate("EarthBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Earth" && this.Attribute == "Water" || zp.Attribute == "Water" && this.Attribute == "Earth")
        {
            // Heal
            zp.Health += 5;
            zp.isHeal = 5 * 60;
        }
        else if (zp.Attribute == "Wind" && this.Attribute == "Water" || zp.Attribute == "Water" && this.Attribute == "Wind")
        {
            // Ice
            GameObject intantBullet = PhotonNetwork.Instantiate("IceBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Wind" && this.Attribute == "Earth" || zp.Attribute == "Earth" && this.Attribute == "Wind")
        {
            // Sand
            GameObject intantBullet = PhotonNetwork.Instantiate("SandBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Wind" && this.Attribute == "Fire" || zp.Attribute == "Fire" && this.Attribute == "Wind")
        {
            // Explode
            GameObject intantBullet = PhotonNetwork.Instantiate("ExplodeBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Earth" && this.Attribute == "Fire" || zp.Attribute == "Fire" && this.Attribute == "Earth")
        {
            // Magma
            GameObject intantBullet = PhotonNetwork.Instantiate("MagmaBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else if (zp.Attribute == "Water" && this.Attribute == "Fire" || zp.Attribute == "Fire" && this.Attribute == "Water")
        {
            // Steam
            GameObject intantBullet = PhotonNetwork.Instantiate("SteamBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }
        else
        {
            GameObject intantBullet = PhotonNetwork.Instantiate("greenBullet", bulletPos.position, bulletPos.rotation, 0);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 50;
        }


        yield return null;
    }
}
