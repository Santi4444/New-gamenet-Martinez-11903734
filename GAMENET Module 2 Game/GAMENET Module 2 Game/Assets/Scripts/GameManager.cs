using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            //Instantiate the prefab position with random range, and roation with quaternion

            int randomPointX = Random.Range(-10, 10);
            int randomPointZ = Random.Range(-10, 10);

            
            //Chooses among the list
            Vector3 spawnLocation = SpawnPoints.instance.spawnPoints[Random.Range(0, SpawnPoints.instance.spawnPoints.Count)].transform.position;

            //PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX, 0, randomPointZ), Quaternion.identity);

            PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation, Quaternion.identity);


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
