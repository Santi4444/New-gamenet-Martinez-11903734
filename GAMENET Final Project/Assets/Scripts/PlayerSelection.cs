using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{

    public GameObject[] SelectablePlayers;

    public int playerSelectionNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerSelectionNumber = 0;

        ActivatePlayer(playerSelectionNumber);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void ActivatePlayer(int x)
    {
        //for each object in selectable players
        foreach (GameObject go in SelectablePlayers)
        {
            go.SetActive(false);

        }
        //activate the chosem player
        SelectablePlayers[x].SetActive(true);

        ////setting the player selevtion for the vehicle
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);

    }

    public void goToNextPlayer()
    {
        playerSelectionNumber++;
        if (playerSelectionNumber >= SelectablePlayers.Length)
        {
            playerSelectionNumber = 0;

        }
        ActivatePlayer(playerSelectionNumber);

    }

    public void goToPrevPlayeras()
    {
        playerSelectionNumber--;
        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = SelectablePlayers.Length - 1;

        }
        ActivatePlayer(playerSelectionNumber);

    }
}
