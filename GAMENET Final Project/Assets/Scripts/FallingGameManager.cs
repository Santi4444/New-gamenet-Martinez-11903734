using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FallingGameManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;

    public Transform[] startingPositions;


    public GameObject winnerUIText;

    ////singleton
    public static FallingGameManager instance = null;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);
                ////Position of car
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;

                //instantiate the car: type, position, rotation 
                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }

        }

    }



    // Update is called once per frame
    void Update()
    {

    }
}
