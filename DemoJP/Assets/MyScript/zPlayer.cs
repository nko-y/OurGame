using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class zPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;

    public int Health = 100;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();


        zCameraWork _cameraWork = this.gameObject.GetComponent<zCameraWork>();


        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                 _cameraWork.OnStartFollowing();
             }
         }
        else
         {
             Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
         }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Interation();
        Die();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        //gDown = Input.GetButtonDown("Fire2");
        //rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        //sDown1 = Input.GetButtonDown("Swap1");
        //sDown2 = Input.GetButtonDown("Swap2");
        //sDown3 = Input.GetButtonDown("Swap3");
    }


    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady || isDead)
            moveVec = Vector3.zero;

        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            //jumpSound.Play();
        }
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.tag == "Weapon" && nearObject.activeSelf)
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                equipWeaponIndex = weaponIndex;
                if (equipWeapon != null)
                    equipWeapon.gameObject.SetActive(false);
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                //Destroy(nearObject);

                nearObject.SetActive(false);
            
                PhotonView p = PhotonView.Get(nearObject);
                p.RPC("OnDetroyWeaponRPC", RpcTarget.All);
                
            }
        }
    }

    void Die()
    {
        if (Health <= 0 && !isDead)
        {
            isDead = true;
            anim.SetTrigger("doDie");
            //PhotonView.Destroy(this.gameObject);
        }
    }



    float fireDelay;
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        GameObject weapon1 = weapons[0];//.GetComponent<Weapon>().gameObject;
        GameObject weapon2 = weapons[1];//.GetComponent<Weapon>().gameObject;
        GameObject weapon3 = weapons[2];//.GetComponent<Weapon>().gameObject;
        if (stream.IsWriting)
        {
            stream.SendNext(weapon1.activeSelf);
            stream.SendNext(weapon2.activeSelf);
            stream.SendNext(weapon3.activeSelf);
            stream.SendNext(this.Health);
        }
        else
        {
            weapon1.SetActive((bool)stream.ReceiveNext());
            weapon2.SetActive((bool)stream.ReceiveNext());
            weapon3.SetActive((bool)stream.ReceiveNext());
            this.Health = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void OnHealtDecRPC(int dec)
    {
        this.Health -= dec;
    }
}
