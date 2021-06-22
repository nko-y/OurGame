using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class SceneMgr : MonoBehaviourPunCallbacks
{
    //武器槽
    public GameObject[] weaponSlots = new GameObject[3];
    public Sprite[] weaponPics = new Sprite[4];
    public GameObject[] selectWeapon = new GameObject[3];

    public static int[] nowWeapons = new int[3];

    //属性槽
    public GameObject attrSlot;
    public Sprite[] attrSprite = new Sprite[4];

    //血量槽
    public GameObject healthText;

    //存货槽
    public GameObject aliveText;

    //死亡界面
    public GameObject diePanel;

    //获胜界面
    public GameObject winPanel;

    //物体索引
    public static GameObject LocalPlayerInstance;

    //是否结束回到初始界面
    private bool isLeaving;

    // Start is called before the first frame update
    void Start()
    {
        if (LocalPlayerInstance == null)
            LocalPlayerInstance = PhotonNetwork.Instantiate("zPlayer", GlobalVariable.StartPos[GlobalVariable.myOrder], Quaternion.identity, 0);
        else
            LocalPlayerInstance = GlobalVariable.LocalPlayerInstance;
        // Weapons
        //PhotonNetwork.Instantiate("Weapons", new Vector3(-809.465f, 445.753f, -703.306f), Quaternion.identity, 0);
        //获得初始武器
        for (int i = 0; i < 3; i++)
        {
            nowWeapons[i] = GlobalVariable.UserWeapon[i];
        }
        //设置初始武器
        for (int i  =0; i < 3; i++)
        {
            weaponSlots[i].GetComponent<Image>().sprite = weaponPics[nowWeapons[i]];
            weaponSlots[i].SetActive(true);
        }
        //设置初始界面
        diePanel.SetActive(false);
        winPanel.SetActive(false);
        //设置初始状态
        isLeaving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeaving) return;
        int playerCurrentWeapon = LocalPlayerInstance.GetComponent<zPlayer>().curWeapon-1;
        //设置选定武器
        for (int i = 0; i < 3; i++) {
            if (i == playerCurrentWeapon)
            {
                selectWeapon[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                selectWeapon[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
        GameObject[] thisPlayer = GameObject.FindGameObjectsWithTag("Player");

        //设定当前的属性
        string playerCurrentAttr = LocalPlayerInstance.GetComponent<zPlayer>().Attribute;
        if (playerCurrentAttr == "Fire")
        {
            attrSlot.GetComponent<Image>().sprite = attrSprite[0];
            attrSlot.SetActive(true);
        }
        else if (playerCurrentAttr == "Water")
        {
            attrSlot.GetComponent<Image>().sprite = attrSprite[1];
            attrSlot.SetActive(true);
        }
        else if (playerCurrentAttr == "Earth")
        {
            attrSlot.GetComponent<Image>().sprite = attrSprite[2];
            attrSlot.SetActive(true);
        }
        else if (playerCurrentAttr == "Wind")
        {
            attrSlot.GetComponent<Image>().sprite = attrSprite[3];
            attrSlot.SetActive(true);
        }
        else
        {
            attrSlot.SetActive(false);
        }

        //设置当前血量
        int tempMaxHealth = LocalPlayerInstance.GetComponent<zPlayer>().MaxHealth;
        int tempNowHealth = LocalPlayerInstance.GetComponent<zPlayer>().Health;
        string tempHealthText = tempNowHealth + " / " + tempMaxHealth;
        healthText.GetComponent<Text>().text = tempHealthText;

        //设定当前存活
        int tempMaxPerson = LocalPlayerInstance.GetComponent<zPlayer>().MaxPerson;
        int tempNowPerson = LocalPlayerInstance.GetComponent<zPlayer>().myalivePerson;
        string tempPersonText = tempNowPerson + " / " + tempMaxPerson;
        aliveText.GetComponent<Text>().text = tempPersonText;

        //设置当前界面
        bool tempIsDead = LocalPlayerInstance.GetComponent<zPlayer>().isDead;
        if (tempIsDead)
        {
            diePanel.SetActive(true);
        }
        if(!tempIsDead && tempNowPerson==1)
        {
            winPanel.SetActive(true);
        }

        //判断点击ESC是否直接退出
        if (LocalPlayerInstance.GetComponent<zPlayer>().sureToLeave)
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
            //遍历所有“Player”标签的对象
            for (int i = 0; i < list.Length; i++)
            {
                PhotonView p = PhotonView.Get(list[i]);
                p.RPC("OnPlayerDecRPC", RpcTarget.All);
            }
            this.returnToStart();
        }
    }

    
    //退出当前场景回到开始界面
    public void returnToStart()
    {
        isLeaving = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}
