using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;


    public int playerPoints = 0;

    //hit effect 
    public GameObject hitEffectPrefab;


    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    //name
    public Text nameBox;
    private string name = "";

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();

        photonView.RPC("GetName", RpcTarget.AllBuffered);
        //name = PhotonNetwork.LocalPlayer.NickName;
        //nameBox.text = name;
        
        

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Fire()
    {
        //the raycast
        RaycastHit hit;

        //the ray is the camera view point
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        //if the ray is within the ray start and the h
        if (Physics.Raycast(ray, out hit, 200))
        {

            Debug.Log(hit.collider.gameObject.name);

            //RPC (rpc function name, target, if there are paramters in the chosen rpc function );
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            //starts the effect
            GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
            //destroys it after 0.2f
            Destroy(hitEffectGameObject, 0.2f);

            //If you hit an player and it isn't yourself
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                //gets from takedamage, spreads the info to everyone in and coming in to the server, and it is 25 damage
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);

                if (hit.collider.gameObject.GetComponent<Shooting>().health <= 0)
                {
                    
                    playerPoints++;
                    if(playerPoints >= 10 )
                    {
                        //GameObject victoryText = GameObject.Find("Victory Text");
                        //victoryText.GetComponent<Text>().text = " Win ";
                        photonView.RPC("VictoryScreen", RpcTarget.All);
                    }
                }
            }


        }

    }
    //RPC
    //photonmessage info gives info via a message by default
    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            //photonView.RPC("KillFeedTracker", RpcTarget.All);
            Die();

            //Killfeed
            StartCoroutine(KillFeedCountdown());
            GameObject killFeedText = GameObject.Find("Kill Feed");
            killFeedText.GetComponent<Text>().text = info.Sender.NickName + " killed " + info.photonView.Owner.NickName;

            //says that the you send a message that you were killed this opponent
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

            
        
        }
    
    }
    //RPC
    //photonmessage info gives info via a message by default
    [PunRPC]
    public void VictoryScreen( PhotonMessageInfo info)
    {
        GameObject victoryText = GameObject.Find("Victory Text");
        victoryText.GetComponent<Text>().text = info.Sender.NickName + " has Won! ";
    }
    //RPC
    //photonmessage info gives info via a message by default
    [PunRPC]
    public void GetName(PhotonMessageInfo info)
    {

        //GameObject nameplate = GameObject.Find("NamePlate");
        //nameplate.GetComponent<Text>().text = info.Sender.NickName;
        nameBox.text = info.photonView.Owner.NickName;


    }
    [PunRPC]
    public void KillFeedTracker(PhotonMessageInfo info)
    {
        GameObject killFeedText = GameObject.Find("Kill Feed");
        killFeedText.GetComponent<Text>().text = "";



    }




    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        //starts the effect
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        //destroys it after 0.2f
        Destroy(hitEffectGameObject, 0.2f);

    }

    public void Die()
    {
        //if it your character then
        if (photonView.IsMine)
        {

            animator.SetBool("isDead", true);
            
            StartCoroutine(RespawnCountdown());

            
        }
    
    }
    IEnumerator KillFeedCountdown()
    {

        float killFeedTime = 5.0f;

        //while respawntime is graeter than 0
        while (killFeedTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            killFeedTime--;

        }
        photonView.RPC("KillFeedTracker", RpcTarget.All);
       

    }
        IEnumerator RespawnCountdown()
    {
        //RespawnCountdown text and timer
        GameObject respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;

        //while respawntime is graeter than 0
        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            //disable movement when you are dead
            transform.GetComponent<PlayerMovementController>().enabled = false;

            //makes the text say
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
            
        }


        //make the boolean isDead false to respawn
        animator.SetBool("isDead", false);
        //makes text blank to make player see no respawn text while alive
        respawnText.GetComponent<Text>().text = "";

        //resets the killfeed
        GameObject killFeedText = GameObject.Find("Kill Feed");
        killFeedText.GetComponent<Text>().text = "";

        int randomPointX = Random.Range(-20, 20);
        int randomPointZ = Random.Range(-20, 20);

        //Chooses among the list
        Vector3 spawnLocation = SpawnPoints.instance.spawnPoints[Random.Range(0, SpawnPoints.instance.spawnPoints.Count)].transform.position;


        //random respawn location
        //this.transform.position = new Vector3(randomPointX, 0, randomPointZ);

        //spawn point spawner
        this.transform.position = spawnLocation;

        //renable movement controller
        transform.GetComponent<PlayerMovementController>().enabled = true;

        //resets health function
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);


    }



    [PunRPC]
    public void RegainHealth()
    {
        //reset health and health bar
        health = 100;
        healthBar.fillAmount = health / startHealth;

    }
}
