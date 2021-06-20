using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;


namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        //True, when the user is firing
        bool IsFiring;
        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {

        }

        void Start()
        {
            //CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();


            //if (_cameraWork != null)
            //{
            //    if (photonView.IsMine)
            //    {
           //         _cameraWork.OnStartFollowing();
           //     }
           // }
            //else
           // {
           //     Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
           // }
        }
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {

        }

        #endregion

        #region Custom



        #endregion
    }

}