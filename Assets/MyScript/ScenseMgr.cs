using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScenseMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("$$$$$$$$$$$$$$$");
        PhotonNetwork.Instantiate("zPlayer", new Vector3(-809.4f, 423f, -771f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
