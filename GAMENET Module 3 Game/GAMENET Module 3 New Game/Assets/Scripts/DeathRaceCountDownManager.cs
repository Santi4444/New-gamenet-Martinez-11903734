using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DeathRaceCountDownManager : MonoBehaviourPunCallbacks
{
    public Text timerText;

    public float timeToStartRace = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        //gets it from the script racinggamemanager
        timerText = DeathRaceGameManager.instance.timeText;
    }

    // Update is called once per frame
    void Update()
    {
        //the master client is the one who starts the countdown
        if (PhotonNetwork.IsMasterClient)
        {
            //time goes down at the start
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                //AllBuffered sends it 
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRaces", RpcTarget.AllBuffered);
            }
        }
    }
    //set time to this amount
    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRaces()
    {
        //there a boolean that prevents the cars to move but this makes them moveable
        GetComponent<VehicleMovement>().isControlEnabled = true;
        //It will immedately turn itself off from the game
        this.enabled = false;



    }
}
