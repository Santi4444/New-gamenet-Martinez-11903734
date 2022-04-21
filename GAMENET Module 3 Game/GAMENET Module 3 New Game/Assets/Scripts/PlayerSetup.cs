using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI playerNameText;


    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        //gets the camera from the prefab
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        //makes sure your camera your own
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            //controller movement is yours
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            //makes sure  the lapcontroller is yours 
            GetComponent<LapController>().enabled = photonView.IsMine;
            //camera enabled is yours
            camera.enabled = photonView.IsMine;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            //controller movement is yours
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            //makes sure  the lapcontroller is yours 
            //GetComponent<LapController>().enabled = photonView.IsMine;
            //camera enabled is yours
            camera.enabled = photonView.IsMine;

            GetComponent<FiringProjectileCode>().enabled = photonView.IsMine;
            GetComponent<DeathMatchController>().enabled = photonView.IsMine;

        }
        playerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
