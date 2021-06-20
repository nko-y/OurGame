using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWeapon : MonoBehaviour
{
    public GameObject firePanel;
    public GameObject waterPanel;
    public GameObject soilPanel;
    public GameObject windPanel;

    //0-火，1-水，2-土，3-风
    public Sprite[] AllSprite = new Sprite[4];
    public GameObject[] ImagePanel = new GameObject[3];
    public int[] weaponNum = new int[3] { -1, -1, -1 };


    //设置武器图片
    void setWeaponImg(int id)
    {
        int whichOne = 0;
        if (weaponNum[0] == -1) { whichOne = 0; }
        else if (weaponNum[1] == -1) { whichOne = 1; }
        else if (weaponNum[2] == -1) { whichOne = 2; }
        else return;
        weaponNum[whichOne] = id;
        ImagePanel[whichOne].GetComponent<Image>().sprite = AllSprite[id];
        ImagePanel[whichOne].SetActive(true);
    }

    public void setDelete(int whichOne)
    {
        if (weaponNum[whichOne] == -1) return;
        int afterNum = 0;
        for(int i=whichOne+1; i<3; i++)
        {
            if (weaponNum[i] == -1) break;
            else afterNum += 1;
        }
        for(int i=whichOne; i<2; i++)
        {
            weaponNum[i] = weaponNum[i + 1];
            ImagePanel[i].GetComponent<Image>().sprite = ImagePanel[i + 1].GetComponent<Image>().sprite;
        }
        weaponNum[whichOne + afterNum] = -1;
        ImagePanel[whichOne + afterNum].GetComponent<Image>().sprite = null;
        ImagePanel[whichOne + afterNum].SetActive(false);
    }

    public void OnClickFire()
    {
        setWeaponImg(0);
    }
    public void OnEnterFire()
    {
        firePanel.SetActive(true);
    }
    public void OnExitFire()
    {
        firePanel.SetActive(false);
    }

    public void OnClickSoil()
    {
        setWeaponImg(2);
    }
    public void OnEnterSoil()
    {
        soilPanel.SetActive(true);
    }
    public void OnExitSoil()
    {
        soilPanel.SetActive(false);
    }

    public void OnClickWater()
    {
        setWeaponImg(1);
    }
    public void OnEnterWater()
    {
        waterPanel.SetActive(true);
    }
    public void OnExitWater()
    {
        waterPanel.SetActive(false);
    }

    public void OnClickWind()
    {
        setWeaponImg(3);
    }
    public void OnEnterWind()
    {
        windPanel.SetActive(true);
    }
    public void OnExitWind()
    {
        windPanel.SetActive(false);
    }
}
