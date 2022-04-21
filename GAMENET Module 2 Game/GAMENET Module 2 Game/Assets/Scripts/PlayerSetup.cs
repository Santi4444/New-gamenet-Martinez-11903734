using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonfpsModel;

    public GameObject playerUIPrefab;

    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;


    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;
   

    private Shooting shooting;

    // Start is called before the first frame update
    void Start()
    {
        //set the movemnet controller to this
        playerMovementController = this.GetComponent<PlayerMovementController>();
        //gets the animator
        animator = this.GetComponent<Animator>();

        //sets the fps model as active when it is yours 
        fpsModel.SetActive(photonView.IsMine);
        //sets the nonfps model as active when it is not yours 
        nonfpsModel.SetActive(!photonView.IsMine);
        //makes the boolean isLocalPlayer in the animator 
        animator.SetBool("isLocalPlayer", photonView.IsMine);

        //get the Shooting script for shooting
        shooting = this.GetComponent<Shooting>();



        //used to decide which animator state to use depending if you control yours or it another opponent
        //basically this but in one line code
        /*
        if (photonView.IsMine)
        {

            this.animator.avatar = fpsAvatar;
        }
        else
        {
            this.animator.avatar = nonFpsAvatar;
        }
        */
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;




        //If it is your player camera
        if (photonView.IsMine)
        {
            //instantiate the player ui prefab if its yours
            GameObject playerUI = Instantiate(playerUIPrefab);

            //set the touch field as the player named ui touchfield
            playerMovementController.fixedTouchField = playerUI.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            //sets camerae;
            fpsCamera.enabled = true;

            //find the fireButton button and adds a listner for the fire function
            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            //if it isn't yours then it disable to prevent players controlling other player controls
            playerMovementController.enabled = false;

            //disable the rigidbody controller
            GetComponent<RigidbodyFirstPersonController>().enabled = false;

            //set to false the player camera
            fpsCamera.enabled = false;

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
