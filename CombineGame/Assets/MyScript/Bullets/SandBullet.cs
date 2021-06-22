using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SandBullet : MonoBehaviourPun
{
    public int damage = 10;
    public bool isMelee;
    public bool isRock;


    void OnTriggerStay(Collider other)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!isMelee && other.gameObject.tag == "Player")
        {
            /* Calculate Damage */
            zPlayer zp = other.gameObject.GetComponent<zPlayer>();

            damage *= 3;

            PhotonView p = PhotonView.Get(other.gameObject);
            p.RPC("OnHealtDecRPC", RpcTarget.Others, damage, zp.name);
            p.RPC("OnSandSlowRPC", RpcTarget.Others, 5 * 60, zp.name);
            p.RPC("OnSetBrownRPC", RpcTarget.Others, 60, zp.name);

            PhotonNetwork.Destroy(this.gameObject);
        }
        else if (!isMelee && other.gameObject.tag == "Wall")  /* Destroy when collided with wall */
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


    /* A bullet can exist for at most 10 sec, after which it will be collected */
    public int life = 600;
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (--life == 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


}