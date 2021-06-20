using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SceneMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("zPlayer", new Vector3(-809.4f, 423f, -771f), Quaternion.identity, 0);

        // Weapons
        //PhotonNetwork.Instantiate("Weapons", new Vector3(-809.465f, 445.753f, -703.306f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
