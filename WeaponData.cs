using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class WeaponData
{
    public static List<WeaponData> allWeaponData;

    public int wepID;
    public string wepName;
    public float wepDamage;
    public float wepSpeed;
    public float wepForce;
    public float wepRange;
    public Sprite wepSkin;

    public WeaponData(int wepID, string wepName, float wepDamage, float wepSpeed, float wepForce, float wepRange, Sprite wepSkin)
    {
        this.wepID = wepID;
        this.wepName = wepName;
        this.wepDamage = wepDamage;
        this.wepSpeed = wepSpeed;
        this.wepForce = wepForce;
        this.wepRange = wepRange;
        this.wepSkin = wepSkin;
    }

    public static void InitializeWeaponData()
    {
        Sprite shortsword_Wood_spr = Resources.Load<Sprite>("Assets/Artwork/Assets/0_Assets/Atlas/at_dungeon_01/obj_wep_m_ss_01");
        WeaponData shortSwordData = new WeaponData(1, "Bronze shortsword", Randomise(1,2), Randomise(1,1.3), Randomise(1,1.3), Randomise(1,1.3), shortsword_Wood_spr);
        allWeaponData.Add(shortSwordData);
    }

    public override string ToString()
    {
        return "Weapon ID: " + wepID + ", Name: " + wepName + ", Damage: " + wepDamage + ", Speed: " + wepSpeed + ", Force: " + wepForce + ", Range: " + wepRange + ", Sprite" + wepSkin;
    }


    public static float Randomise(double minValue, double maxValue)
    {
        System.Random random = new System.Random();
        float randomFloat = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        return (float)Math.Round(randomFloat, 2);
    }
}


