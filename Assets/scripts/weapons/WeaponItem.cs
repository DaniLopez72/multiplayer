using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviourPunCallbacks, IPunObservable
{
    public Weapon weapon;
    public string weaponName;

    private void Start()
    {
        if (weapon == null && weaponName!="")
        {
            weapon = Resources.Load<Weapon>("Scriptables/" + weaponName);
        }
    }
    public void setWeapon(string weaponName)
    {
        this.weaponName=weaponName;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weaponName);
        }
        else
        {
            weaponName=(string)stream.ReceiveNext();
            if (weapon == null && weaponName != "")
            {
                weapon = Resources.Load<Weapon>("Scriptables/" + weaponName);
            }
        }
    }
}
