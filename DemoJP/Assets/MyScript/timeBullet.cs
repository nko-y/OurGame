using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class timeBullet : MonoBehaviourPun
{
    public int damage;

    // Bullet Towards Time
    public int towards = 2;

    // Players
    public static GameObject [] playerlist;

    void Start()
    {
        playerlist = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        checkLife();
        chechAttack();
    }

    void chechAttack()
    {
        for(int i=0;i<playerlist.Length;i++){
            timePlayer tp =  playerlist[i].GetComponent<timePlayer>();
            if(tp.photonView.IsMine) continue;
            Vector3 [] history = tp.historyPos.ToArray();
            Vector3 pos = history[6];
            float dist = Vector3.Distance(transform.position, pos);
            Debug.Log(dist);
            if(dist < 10 ){
                Debug.Log("@@@@@ HIT @@@@@");
                PhotonView p = PhotonView.Get(playerlist[i]);
                p.RPC("OnHealtDecRPC", RpcTarget.Others, 20);

                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }


    
    public int life = 600;
    void checkLife()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if(--life == 0){
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
