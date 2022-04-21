using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks
{
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

        if (health <= 0)
        {
            this.gameObject.GetComponent<KilledEvent>().UpdateKilledText();
            this.gameObject.GetComponent<PhotonView>().RPC("Die", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Die(PhotonMessageInfo info)
    {
        //When you "die" than
        if (photonView.IsMine)
        {
            
            //GameObject killFeedText = GameObject.Find("Kill Feed");
            //killFeedText.GetComponent<Text>().text = info.photonView.Owner.NickName + " has been killed";
            //This is a singleton call instance found in the gamemanger file so when you die you leave the room
            DeathRaceGameManager.instance.LeaveRoom();
        }

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Projectile")
        {
           
            this.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10);
            Destroy(col.gameObject);
            //photonView
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("ONLY ONE");
            this.gameObject.GetComponent<DeathMatchController>().UpdateWinnerText();
        }
    }
}
