using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public GameObject prefabWeapon;
    GameObject currentInstance;
    public List<Weapon> weapons;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating(nameof(CreateWeapon), 2, 4);
        }
    }

    void CreateWeapon()
    {
        if (currentInstance!= null) return;

        int randomWeapon = Random.Range(0, weapons.Count);

        

        float randomX = Random.Range(-transform.localScale.x/2, transform.localScale.x/2);
        float randomZ = Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);
        Vector3 finalPosition = transform.position;
        finalPosition.x = randomX;
        finalPosition.z = randomZ;
        currentInstance = PhotonNetwork.Instantiate(prefabWeapon.name,finalPosition,Quaternion.Euler(0,0,0));
        currentInstance.GetComponent<WeaponItem>().setWeapon(weapons[randomWeapon].id);
    }
}
