using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static string UserName;
    public static int[] UserWeapon = new int[3];
    public static GameObject LocalPlayerInstance=null;
    public static Vector3[] StartPos = new Vector3[4] { 
        new Vector3(-809.4f, 423f, -771f), 
        new Vector3(-767.1986f, 423f, -751.3f), 
        new Vector3(-796.62f, 423f, -751.3f), 
        new Vector3(-796.62f, 423f, -793.6f) 
    };
    public static int myOrder;
}
