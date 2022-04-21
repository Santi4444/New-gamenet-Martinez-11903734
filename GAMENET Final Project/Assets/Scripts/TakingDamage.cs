using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    public GameObject Respawnpoint;

    [SerializeField]
    Image healthbar;

    private float startHealth = 100;

    public float health;


    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthbar.fillAmount = health / startHealth;
        
    }

    //To do it in an online setting you need to broadcast to everyone so you need a RPC (Remote Procedure Calls)
    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);

        //healthbar fill bar
        healthbar.fillAmount = health / startHealth;

        //Destroy(col.gameObject);

        if (health <= 0)
        {
            //this.gameObject.GetComponent<KilledEvent>().UpdateKilledText();
            this.gameObject.GetComponent<PhotonView>().RPC("Die", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void TakeHealth(int medkit)
    {
        health += medkit;
        
        if (health >= 100)
        {
            health = 100;
        }
        Debug.Log(health);
        //healthbar fill bar
        healthbar.fillAmount = health / startHealth;

    }
    [PunRPC]
    public void InstantDeath()
    {
        health *= 0;
        Debug.Log(health);

        //healthbar fill bar
        healthbar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            //this.gameObject.GetComponent<KilledEvent>().UpdateKilledText();
            this.gameObject.GetComponent<PhotonView>().RPC("Die", RpcTarget.AllBuffered);
        }

    }
    [PunRPC]
    public void TakeSpeed()
    {

        this.gameObject.GetComponent<Rigidbody>().drag = 1;

        Debug.Log(this.gameObject.GetComponent<Rigidbody>().drag);


    }
    [PunRPC]
    public void Die(PhotonMessageInfo info)
    {
        //When you "die" than
        if (photonView.IsMine)
        {
            this.transform.position = new Vector3(Respawnpoint.gameObject.transform.position.x, Respawnpoint.gameObject.transform.position.y, Respawnpoint.gameObject.transform.position.z);
            Debug.Log("No more health");

            health = startHealth;
            Debug.Log("Restored health");
            healthbar.fillAmount = health / startHealth;
            //GameObject killFeedText = GameObject.Find("Kill Feed");
            //killFeedText.GetComponent<Text>().text = info.photonView.Owner.NickName + " has been killed";
            //This is a singleton call instance found in the gamemanger file so when you die you leave the room
            //DeathRaceGameManager.instance.LeaveRoom();
        }
        health = startHealth;
        Debug.Log("Restored health");
        healthbar.fillAmount = health / startHealth;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Projectile")
        {

            this.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 5);
            Destroy(col.gameObject);
            //photonView
        }
        if (col.gameObject.tag == "Medkit")
        {

            this.gameObject.GetComponent<PhotonView>().RPC("TakeHealth", RpcTarget.AllBuffered, 20);
            //Destroy(col.gameObject);
            //photonView
        }
        if (col.gameObject.tag == "InstantDeath")
        {

            this.gameObject.GetComponent<PhotonView>().RPC("InstantDeath", RpcTarget.AllBuffered);

            //photonView
        }
        if (col.gameObject.tag == "Speed")
        {

            this.gameObject.GetComponent<PhotonView>().RPC("TakeSpeed", RpcTarget.AllBuffered);

            //photonView
        }
        if (col.gameObject.tag == "FinishLine")
        {

            this.gameObject.GetComponent<EndLapEvent>().UpdateFirstText();

            //photonView
        }
    }

    void Update()
    {

    }
}
