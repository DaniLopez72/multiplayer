using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
    public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
    {
        public int life;
        public int deadCount;

        public CharacterController control;
        public float speed, rotationSpeed, jumpForce, gravity;
        Vector3 moveDir;

        public GameObject otherCanvas;
        public TextMeshProUGUI otherUsername;

        public List<Weapon> allWeapons;
        int currentWeapon;

        Vector3 initPosition;
        Quaternion initRotation;

        Vector3 currentPosition;
        Quaternion currentRotation;

        public Transform character;
        public Animator anim;
        bool jump;

        bool setRespawn;

        bool jugar = true;

        public GameObject lose;
        public GameObject win;
        // Start is called before the first frame update
        void Start()
        {
            string getSkin = (string)photonView.Owner.CustomProperties["Skin"];

            GameObject getCharacter = Resources.Load<GameObject>(getSkin);

            if (getCharacter != null)
            {
                GameObject newCharacter=Instantiate(getCharacter,character);
                anim=newCharacter.GetComponent<Animator>();
            }
            if (photonView.IsMine)
            {

                otherCanvas.SetActive(false);
                Camera.main.GetComponent<CameraControl>().SetTarget(transform);
                initPosition = transform.position;
                initRotation = transform.rotation;


            }
            else
            {
                otherUsername.text = photonView.Owner.NickName;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {

                if (Input.GetKeyDown(KeyCode.Alpha1)&&allWeapons.Count>=1)
                {
                    currentWeapon = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && allWeapons.Count >= 2)
                {
                    currentWeapon = 1;
                }else if (Input.GetKeyDown(KeyCode.Alpha3) && allWeapons.Count >= 3)
                {
                    currentWeapon = 2;
                }else if (Input.GetKeyDown(KeyCode.Alpha4) && allWeapons.Count >= 4)
                {
                    currentWeapon = 3;
                }
                if (PhotonNetwork.CurrentRoom.PlayerCount >= 2&&jugar==true)
                {
                if (allWeapons.Count>currentWeapon) { 
                
                    allWeapons[currentWeapon].UpdateWeapon();
                }
                    transform.Rotate(Vector3.up * rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);
                    moveDir = new Vector3(Input.GetAxis("Horizontal") * speed, moveDir.y, Input.GetAxis("Vertical") * speed);

                    Vector3 finalSpeedAnimator = moveDir;
                    finalSpeedAnimator.y = 0;
                    float finalSpeed = (finalSpeedAnimator.magnitude > 0) ? 1 : 0;
                    
                        SetValueAnimator("speed", finalSpeed);
                        SetValueAnimator("isGrounded", control.isGrounded);
                }
                
                
                
                moveDir=transform.TransformDirection(moveDir);

                if(control.isGrounded)
                {
                    if (Input.GetButton("Jump"))
                    {
                        moveDir.y = jumpForce;
                        SetValueAnimator("jump");
                        jump = true;
                    }
                }
                else
                {
                    moveDir.y -=gravity*Time.deltaTime;
                }

                if (deadCount==3)
                {
                    jugar = false;
                    lose.SetActive(true);
                }
                
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position,currentPosition,4*Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentRotation, 4 * Time.deltaTime);

                if (setRespawn)
                {
                    transform.position = currentPosition; 
                    transform.rotation = currentRotation;
                    
                }
                if (deadCount == 3)
                {
                    jugar = false;
                    win.SetActive(true);

                }
            }
            
        }
        private void FixedUpdate()
        {
            if (photonView.IsMine) { control.Move(moveDir * Time.deltaTime);}
            
            else
            {

            }
        }

        void AddWeapon(Weapon weapon)
        {
            if (allWeapons.Count >= 4)
            {
                allWeapons.RemoveAt(0);
            }
            Weapon newWeapon=Instantiate(weapon);
            newWeapon.InitWeapon(transform);
            allWeapons.Add(newWeapon);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Weapon"))
            {
               
                
                    Weapon getWeapon = other.GetComponent<WeaponItem>().weapon;
                    if (allWeapons.Find(x => x.id == getWeapon.id) != null) return;

                    AddWeapon(getWeapon);
                
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(other.gameObject);
                }
            }
            if (other.tag.Equals("Trampa"))
            {
                GetDamage(100);
            }
        }



        public void SetDamage(GameObject objective,int damage)
        {
            objective.GetComponent<PhotonView>().RPC(nameof(RPC_GetDagame),
                        RpcTarget.Others, damage);
        }

        [PunRPC]
        public void RPC_GetDagame(int damage)
        {
            GetDamage(damage);
        }
        public void GetDamage(int damage)
        {
            life-=damage;
            if (life < 0)
            {
                life = 0;
                deadCount++;
                transform.position = initPosition;
                transform.rotation = initRotation;
                setRespawn=true;
                life = 50;
            }
        }
        public void SetAnimator(Animator anim)
        {
            this.anim = anim;
        }

        public void SetValueAnimator(string propertyName,float value)
        {
            if(anim!=null) anim.SetFloat(propertyName, value);
        }
        public void SetValueAnimator(string propertyName,bool value)
        {
            if (anim != null) anim.SetBool(propertyName, value);
        }
        public void SetValueAnimator(string propertyName)
        {
            if (anim != null) anim.SetTrigger(propertyName);
        }
        public void SetValueAnimator(string propertyName, int value)
        {
            if (anim != null) anim.SetInteger(propertyName, value);
        }
        public float GetFloatAnimator(string propertyName)
        {
            if (anim != null) return anim.GetFloat(propertyName);
            return 0;
        }
        public bool GetBoolAnimator(string propertyName)
        {
            if (anim != null) return anim.GetBool(propertyName);
            return false;
        }
        public int GetIntAnimator(string propertyName)
        {
            if (anim != null) return anim.GetInteger(propertyName);
            return 0;
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(life);
                stream.SendNext(setRespawn);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(deadCount);

                stream.SendNext(GetFloatAnimator("speed"));
                stream.SendNext(GetBoolAnimator("isGrounded"));
                stream.SendNext(jump);
                jump = false;
                
                setRespawn = false;
            }
            else
            {
                life=(int)stream.ReceiveNext();
                setRespawn=(bool)stream.ReceiveNext();
                currentPosition = (Vector3)stream.ReceiveNext();
                currentRotation = (Quaternion)(stream.ReceiveNext());
                deadCount= (int)stream.ReceiveNext();

                float speedAnim=(float)stream.ReceiveNext();
                SetValueAnimator("speed", speedAnim);
                bool isGroundedAnim = (bool)stream.ReceiveNext();
                SetValueAnimator("isGrounded", isGroundedAnim);
                bool isJumping= (bool)stream.ReceiveNext();
                if (isJumping)
                {
                    SetValueAnimator("jump");
                }
            }
        }
    }

}