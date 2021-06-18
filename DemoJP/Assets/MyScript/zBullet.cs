using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class zBullet : MonoBehaviourPun
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!isMelee && other.gameObject.tag == "Player")
        {
            PhotonView p = PhotonView.Get(other.gameObject);
            p.RPC("OnHealtDecRPC", RpcTarget.Others, 20);

            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}