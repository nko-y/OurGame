using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class SceneMgr : MonoBehaviourPunCallbacks
{
    //������
    public GameObject[] weaponSlots = new GameObject[3];
    public Sprite[] weaponPics = new Sprite[4];
    public GameObject[] selectWeapon = new GameObject[3];

    public static int[] nowWeapons = new int[3];

    //���Բ�
    public GameObject attrSlot;
    public Sprite[] attrSprite = new Sprite[4];

    //Ѫ����
    public GameObject healthText;

    //�����
    public GameObject aliveText;

    //��������
    public GameObject diePanel;

    //��ʤ����
    public GameObject winPanel;

    //��������
    public static GameObject LocalPlayerInstance;

    //�Ƿ�����ص���ʼ����
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
        //��ó�ʼ����
        for (int i = 0; i < 3; i++)
        {
            nowWeapons[i] = GlobalVariable.UserWeapon[i];
        }
        //���ó�ʼ����
        for (int i  =0; i < 3; i++)
        {
            weaponSlots[i].GetComponent<Image>().sprite = weaponPics[nowWeapons[i]];
            weaponSlots[i].SetActive(true);
        }
        //���ó�ʼ����
        diePanel.SetActive(false);
        winPanel.SetActive(false);
        //���ó�ʼ״̬
        isLeaving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeaving) return;
        int playerCurrentWeapon = LocalPlayerInstance.GetComponent<zPlayer>().curWeapon-1;
        //����ѡ������
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

        //�趨��ǰ������
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

        //���õ�ǰѪ��
        int tempMaxHealth = LocalPlayerInstance.GetComponent<zPlayer>().MaxHealth;
        int tempNowHealth = LocalPlayerInstance.GetComponent<zPlayer>().Health;
        string tempHealthText = tempNowHealth + " / " + tempMaxHealth;
        healthText.GetComponent<Text>().text = tempHealthText;

        //�趨��ǰ���
        int tempMaxPerson = LocalPlayerInstance.GetComponent<zPlayer>().MaxPerson;
        int tempNowPerson = LocalPlayerInstance.GetComponent<zPlayer>().myalivePerson;
        string tempPersonText = tempNowPerson + " / " + tempMaxPerson;
        aliveText.GetComponent<Text>().text = tempPersonText;

        //���õ�ǰ����
        bool tempIsDead = LocalPlayerInstance.GetComponent<zPlayer>().isDead;
        if (tempIsDead)
        {
            diePanel.SetActive(true);
        }
        if(!tempIsDead && tempNowPerson==1)
        {
            winPanel.SetActive(true);
        }

        //�жϵ��ESC�Ƿ�ֱ���˳�
        if (LocalPlayerInstance.GetComponent<zPlayer>().sureToLeave)
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
            //�������С�Player����ǩ�Ķ���
            for (int i = 0; i < list.Length; i++)
            {
                PhotonView p = PhotonView.Get(list[i]);
                p.RPC("OnPlayerDecRPC", RpcTarget.All);
            }
            this.returnToStart();
        }
    }

    
    //�˳���ǰ�����ص���ʼ����
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
