using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public string id;
    public int shootDamage;
    public float fireRate;
    protected float fireTime;
    protected Transform target;
    //public Sprite icon;

    public void InitWeapon(Transform target)
    {
        this.target = target;
    }

    public virtual void UpdateWeapon()
    {

    }
}
