using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerListItemInitializer : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private bool isPlayerReady = false;

    public void Initialize(int playerId, string playerName)
    {
        PlayerNameText.text = playerName;

        if (PhotonNetwork.LocalPlayer.ActorNumber != playerId)
        {
            //If it not your gameobject no check mark
            PlayerReadyButton.gameObject.SetActive(false);

        }
        else
        {
            //sets custom property for each player "isPlayerReady"
            //Constants has data accessible everywhere in the project
            ExitGames.Client.Photon.Hashtable initializeProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } };

            PhotonNetwork.LocalPlayer.SetCustomProperties(initializeProperties);

            //this will be a custom function so when you click the playerready button then
            PlayerReadyButton.onClick.AddListener(() =>
            {
                //makes true since started false
                isPlayerReady = !isPlayerReady;
                //plays the function
                SetPlayerReady(isPlayerReady);

                //Hashtable key becomes isPlayerReady
                ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } };

                PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
            });
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        //enables check
        PlayerReadyImage.enabled = playerReady;
        //makes text eihter say ready or ?
        if (playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready";
        }
        else
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready?";

        }

    }
}
