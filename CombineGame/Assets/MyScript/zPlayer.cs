using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class zPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public int[] weaponMap;   //  from index to weaponIdx
    public int curWeapon=0;     //  current index (not weaponIdx)
    public zWeapon equipWeapon;
    int equipWeaponIndex = -1;

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
    bool aDown;
    bool mDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    public bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;

    public int MaxHealth = 100;
    public int Health = 100;
    public int MaxPerson=4;
    public int myalivePerson=4;
    public string Attribute;
    public string playerName = "Default";

    /* Status Effect Flags */
    public int isFreezed = 0;   /* Ice  */
    public int isSlowed = 0;    /* Sand */
    public int isFog = 0;       /* Steam */
    public int isHeal = 0;      /* Heal */


    /* Effects */
    public GameObject HealParticle;
    public int playBlue = 0;
    public GameObject BlueHitParticle;
    public int playRed  = 0;
    public GameObject RedHitParticle;
    public int playBrown = 0;
    public GameObject BrownHitParticle;
    public int playWhite = 0;
    public GameObject WhiteHitParticle;

    /* Sound */
    public AudioSource shootSound;
    public AudioSource runSound;
    public AudioSource getAttributeSound;
    public AudioSource hitSound;

    /* UI */
    public GameObject myHealthUI;
    public GameObject myNameUI;
    public bool sureToLeave;

    // Awake
    void Awake()
    {
        Application.targetFrameRate = 60;
        RenderSettings.fog = false;
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

        myNameUI.GetComponent<Text>().text = GlobalVariable.UserName;

        sureToLeave = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        GetInput();
        CheckStatus();
        if (!isDead)
        {
            Move();
            Turn();
            Jump();
            Swap();
            Attack();
            MixAttack();
            Interation();
            GetAttribute();
            Die();
        }

        //更新血量UI
        updateUI();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sureToLeave = true;
        }
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
        aDown = Input.GetButtonDown("Attribute");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        mDown = Input.GetButtonDown("Mix");
    }

    void CheckStatus()
    {
        // Slow
        if (isSlowed > 0)
        {
            isSlowed--;
        }

        // Freeze
        if (isFreezed > 0)
        {
            isFreezed--;
        }

        // Fog
        if (isFog > 0)
        {
            isFog--;
            if (!RenderSettings.fog) RenderSettings.fog = true;
        }
        else
        {
            if (RenderSettings.fog) RenderSettings.fog = false;
        }

        // Heal
        if(isHeal > 0)
        {
            isHeal--;
            if (!HealParticle.activeSelf) HealParticle.SetActive(true);
        }
        else
        {
            if (HealParticle.activeSelf) HealParticle.SetActive(false);
        }

        // Hit Bule
        if (playBlue > 0)
        {
            playBlue--;
            if (!BlueHitParticle.activeSelf) BlueHitParticle.SetActive(true);
        }
        else
        {
            if (BlueHitParticle.activeSelf) BlueHitParticle.SetActive(false);
        }

        // Hit Red
        if (playRed > 0)
        {
            playRed--;
            if (!RedHitParticle.activeSelf) RedHitParticle.SetActive(true);
        }
        else
        {
            if (RedHitParticle.activeSelf) RedHitParticle.SetActive(false);
        }

        // Hit Brown
        if (playBrown > 0)
        {
            playBrown--;
            if (!BrownHitParticle.activeSelf) BrownHitParticle.SetActive(true);
        }
        else
        {
            if (BrownHitParticle.activeSelf) BrownHitParticle.SetActive(false);
        }

        // Hit White
        if (playWhite > 0)
        {
            playWhite--;
            if (!WhiteHitParticle.activeSelf) WhiteHitParticle.SetActive(true);
        }
        else
        {
            if (WhiteHitParticle.activeSelf) WhiteHitParticle.SetActive(false);
        }

    }

    void Move()
    {
        if (isFreezed > 0) return;          /* Freeez the character */

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady || isDead)
            moveVec = Vector3.zero;

        float s = isSlowed > 0 ? (speed / 2) : speed;  /* Half   the speed     */

        if (!isBorder)
            transform.position += moveVec * s * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        if (moveVec != Vector3.zero)
        {
            if(!runSound.isPlaying && !isJump)  runSound.Play();
        }
        else
        {
            if (runSound.isPlaying) runSound.Stop();
        }
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            nextVec.y = 0;
            transform.LookAt(transform.position + nextVec);
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            RenderSettings.fog = !RenderSettings.fog;

            //jumpSound.Play();
        }
        else if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void Swap()
    {
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isDead)
        {
            if (sDown1)
            {
                curWeapon = 1;
            }
            else if (sDown2)
            {
                curWeapon = 2;
            }
            else if (sDown3)
            {
                curWeapon = 3;
            }
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeapon = weapons[weaponMap[SceneMgr.nowWeapons[curWeapon - 1]]].GetComponent<zWeapon>();
            equipWeapon.gameObject.SetActive(true);
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
                equipWeapon = weapons[weaponIndex].GetComponent<zWeapon>();
                equipWeapon.gameObject.SetActive(true);

                //Destroy(nearObject);

                nearObject.SetActive(false);

                PhotonView p = PhotonView.Get(nearObject);
                p.RPC("OnDetroyWeaponRPC", RpcTarget.All);

            }
        }
    }

    void GetAttribute()
    {
        if (aDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.activeSelf && (nearObject.tag == "Water" || nearObject.tag == "Fire" || nearObject.tag == "Wind" || nearObject.tag == "Earth"))
            {
                Attribute = nearObject.tag;
                getAttributeSound.Play();
            }
        }
    }

    void Die()
    {
        if (Health <= 0 && !isDead)
        {
            isDead = true;
            anim.SetTrigger("doDie");
            //PhotonView.Destroy(this.gameObject)
            //PhotonView p = PhotonView.Get(this);
            //p.RPC("OnPlayerDecRPC", RpcTarget.Others);

            GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
            //遍历所有“Player”标签的对象
            for (int i = 0; i < list.Length; i++)
            {
                PhotonView p = PhotonView.Get(list[i]);
                p.RPC("OnPlayerDecRPC", RpcTarget.All);
            }
        }
    }
    
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }



    float fireDelay;
    void Attack()
    {
        if (equipWeapon == null)
            return;

        if (isHeal > 0)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == zWeapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;

            shootSound.Play();
            PhotonView p = PhotonView.Get(this);
            p.RPC("OnPlayShootRPC", RpcTarget.Others);
        }
        
    }

    float mixDelay;
    bool isMixReady;
    float mixRate = 3;
    void MixAttack()
    {
        if (equipWeapon == null)
            return;

        mixDelay += Time.deltaTime;
        isMixReady = mixRate < mixDelay;

        if (mDown && isFireReady && isMixReady && !isDodge && !isSwap && !isDead)
        {
            equipWeapon.Use(true);
            anim.SetTrigger(equipWeapon.type == zWeapon.Type.Melee ? "doSwing" : "doShot");
            mixDelay = 0;

            shootSound.Play();
            PhotonView p = PhotonView.Get(this);
            p.RPC("OnPlayShootRPC", RpcTarget.Others);
        }

    }

    // ========================= Callbacks ==========================

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor" || collision.gameObject.tag == "wall")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Water" || other.tag == "Fire" || other.tag == "Wind" || other.tag == "Earth")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Water" || other.tag == "Fire" || other.tag == "Wind" || other.tag == "Earth")
            nearObject = null;
    }

    // ========================= Serialize Sync ==========================
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        GameObject weapon1 = weapons[0];//.GetComponent<Weapon>().gameObject;
        GameObject weapon2 = weapons[1];//.GetComponent<Weapon>().gameObject; // Wind
        GameObject weapon3 = weapons[2];//.GetComponent<Weapon>().gameObject; // Water
        GameObject weapon4 = weapons[3];//.GetComponent<Weapon>().gameObject; // Fire
        GameObject weapon5 = weapons[4];//.GetComponent<Weapon>().gameObject; // Earth
        if (stream.IsWriting)
        {
            /* Weapon Sync*/
            stream.SendNext(weapon1.activeSelf);
            stream.SendNext(weapon2.activeSelf);
            stream.SendNext(weapon3.activeSelf);
            stream.SendNext(weapon4.activeSelf);
            stream.SendNext(weapon5.activeSelf);
            /* Effect Sync*/
            stream.SendNext(HealParticle.activeSelf);
            stream.SendNext(BlueHitParticle.activeSelf);
            stream.SendNext(RedHitParticle.activeSelf);
            stream.SendNext(BrownHitParticle.activeSelf);
            stream.SendNext(WhiteHitParticle.activeSelf);
            /* Property Sync*/
            stream.SendNext(this.Health);
            stream.SendNext(this.Attribute);
            stream.SendNext(this.playerName);
            /* UI Sync*/
            stream.SendNext(this.Health * 1.0f / this.MaxHealth);
            stream.SendNext(GlobalVariable.UserName);
        }
        else
        {
            /* Weapon Sync*/
            weapon1.SetActive((bool)stream.ReceiveNext());
            weapon2.SetActive((bool)stream.ReceiveNext());
            weapon3.SetActive((bool)stream.ReceiveNext());
            weapon4.SetActive((bool)stream.ReceiveNext());
            weapon5.SetActive((bool)stream.ReceiveNext());
            /* Effect Sync*/
            HealParticle.SetActive((bool)stream.ReceiveNext());
            BlueHitParticle.SetActive((bool)stream.ReceiveNext());
            RedHitParticle.SetActive((bool)stream.ReceiveNext());
            BrownHitParticle.SetActive((bool)stream.ReceiveNext());
            WhiteHitParticle.SetActive((bool)stream.ReceiveNext());
            /* Property Sync*/
            this.Health = (int)stream.ReceiveNext();
            this.Attribute = (string)stream.ReceiveNext();
            this.playerName = (string)stream.ReceiveNext();
            /* UI Sync*/
            this.myHealthUI.GetComponent<Slider>().value = (float)stream.ReceiveNext();
            this.myNameUI.GetComponent<Text>().text = (string)stream.ReceiveNext();
        }
    }

    void updateUI()
    {
        myHealthUI.GetComponent<Slider>().value = Health * 1.0f / MaxHealth;
    }

    // ========================= RPC ==========================

    [PunRPC]
    public void OnHealtDecRPC(int dec, string name)
    {
        //if (this.playerName != name) return;
        this.Health -= dec;
    }

    [PunRPC]
    public void OnIceFreezeRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.isFreezed += time;
    }

    [PunRPC]
    public void OnSteamFogRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.isFog += time;
    }


    [PunRPC]
    public void OnSandSlowRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.isSlowed += time;
    }

    [PunRPC]
    public void OnMagmaClearRPC(string name)
    {
        //if (this.playerName != name) return;
        this.Attribute = "";
    }

    [PunRPC]
    public void OnSetBlueRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.playBlue += time;
    }

    [PunRPC]
    public void OnSetRedRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.playRed += time;
    }

    [PunRPC]
    public void OnSetBrownRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.playBrown += time;
    }

    [PunRPC]
    public void OnSetWhiteRPC(int time, string name)
    {
        //if (this.playerName != name) return;
        this.playWhite += time;
    }

    [PunRPC]
    public void OnPlayShootRPC()
    {
        //if (this.playerName != name) return;
        this.shootSound.Play();
    }

    [PunRPC]
    public void OnPlayHitRPC()
    {
        //if (this.playerName != name) return;
        this.hitSound.Play();
    }

    [PunRPC]
    public void OnPlayerDecRPC()
    {
        //if (this.playerName != name) return;
        this.myalivePerson -= 1;
    }

}
