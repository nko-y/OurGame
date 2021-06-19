using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject StartPanel;
    public GameObject SelectPanel;
    public GameObject confirmPanel;
    public GameObject waitingPanel;
    public GameObject Player;
    public GameObject InputText;
    public GameObject NeedName;
    public GameObject NeedWeapon;
    public GameObject[] NonePerson = new GameObject[4];
    public GameObject[] InPerson = new GameObject[4];
    public int NeedPerson = 2;

    //连接参数
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    string gameVersion = "1";
    bool isConnecting;
    bool hasJoinRoom = false;
    bool hasEnterRoom = false;
    bool canClick = true;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        StartPanel.SetActive(true);
        SelectPanel.SetActive(false);
        confirmPanel.SetActive(false);
        waitingPanel.SetActive(false);
        Player.SetActive(false);
        for(int i=0; i<4; i++)
        {
            NonePerson[i].SetActive(true);
            InPerson[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (hasJoinRoom && !hasEnterRoom)
        {
            waitingPanel.SetActive(true);
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                NonePerson[i].SetActive(false);
                InPerson[i].SetActive(true);
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == NeedPerson)
            {
                GlobalVariable.UserName = InputText.GetComponent<InputField>().text;
                for(int i=0; i<3; i++)
                {
                    GlobalVariable.UserWeapon[i] = this.gameObject.GetComponent<AddWeapon>().weaponNum[i];
                }
                PhotonNetwork.LoadLevel("Room Test");
                hasEnterRoom = true;
                canClick = true;
            }
        }
    }

    public void OnclickExit()
    {
        Start();
    }

    public void OnClickStartGame()
    {
        StartPanel.SetActive(false);
        SelectPanel.SetActive(true);
        Player.SetActive(true);
    }

    public void OnClickConfirm()
    {
        NeedName.SetActive(false);
        NeedWeapon.SetActive(false);
        confirmPanel.SetActive(false);
        canClick = true;
    }

    public void Connect()
    {
        if (canClick == false) return;
        canClick = false;
        //获取名称
        InputField NameInputField = InputText.GetComponent<InputField>();
        string TempUserName = NameInputField.text;
        if (TempUserName == "")
        {
            confirmPanel.SetActive(true);
            NeedName.SetActive(true);
            return;
        }
        if (this.gameObject.GetComponent<AddWeapon>().weaponNum[2] == -1)
        {
            confirmPanel.SetActive(true);
            NeedWeapon.SetActive(true);
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }


    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        hasJoinRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
}
