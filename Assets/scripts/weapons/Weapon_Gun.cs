using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Gun", menuName ="Weapon/Gun")]
public class Weapon_Gun : Weapon
{
    RaycastHit hitShoot;

    public override void UpdateWeapon()
    {

        fireTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0)&&fireTime>=fireRate)
        {
            if (Physics.Raycast(target.position, target.forward, out hitShoot, Mathf.Infinity))
            {
                if (hitShoot.collider.tag.Equals("Player")&&hitShoot.collider.transform !=target)
                {
                    target.GetComponent<PlayerControl>().SetDamage(hitShoot.collider.gameObject, shootDamage);
                }
            }
        }
    }

}
