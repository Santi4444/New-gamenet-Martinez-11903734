using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{

    public static SpawnPoints instance;

    public List<Transform> spawnPoints = new List<Transform>();

   


    private void Awake()
    {
        
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public int getSpawnPointsAmount()
    {
        return spawnPoints.Count;
    
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
