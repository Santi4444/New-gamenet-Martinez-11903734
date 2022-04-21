using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class DeathMatchController : MonoBehaviourPunCallbacks
{
    private const byte PLAYER_COUNT = 0;
    void Start()
    {
        ////for each gameobject in the RacingGM lapTriggers list
        ////Add it to LapController lapTriggers list
        //foreach (GameObject go in RacingGameManager.instance.lapTriggers)
        //{
        //    lapTriggers.Add(go);
        //}
    }
    // Update is called once per frame
    void Update()
    {

    }
    //public List<GameObject> lapTriggers = new List<GameObject>();

    public void UpdateWinnerText()
    {

        string nickName = photonView.Owner.NickName;
        int playerNumber = photonView.Owner.ActorNumber;
        int viewID = photonView.ViewID;

        object[] data = new object[] { nickName, playerNumber, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        //sender
        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        //name of event, data sent, who receives it, how to send it?
        //PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);

        PhotonNetwork.RaiseEvent(PLAYER_COUNT, data, raiseEventOptions, sendOptions);
    }

    private void OnEnable() 
    {
        
       PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;

    }



    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    //onevent
    //oneventreceived

    void OnEventReceived(EventData photonEvent)
    {
        if(photonEvent.Code == PLAYER_COUNT)
        {
            //customdata = the updatewinnertext dat
            object[] data = (object[])photonEvent.CustomData;
            string playerNickName = (string)data[0];
            int playerNumber = (int)data[1];

            int viewId = (int)data[2];

            //write down win code
            GameObject winnerText = DeathRaceGameManager.instance.winnerTextUi;

            if(viewId == photonView.ViewID)
            {
                //playerNickName +
                winnerText.GetComponent<Text>().text = playerNickName + " has WON!!!";
            }

        }

    }








    //public enum RaiseEventsCode
    //{
    //    WhoFinishedEventCode = 0
    //}

    //private int finishOrder = 0;

    //// enable, disable and on event allow the of adding listners to all events


    //void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
    //    {
    //        //array of data on the different events?
    //        object[] data = (object[])photonEvent.CustomData;
    //        //the data of the object data
    //        //name
    //        string nickNameOfFinishedPlayer = (string)data[0];
    //        //placement
    //        finishOrder = (int)data[1];
    //        //viewId
    //        int viewId = (int)data[2];

    //        //placement when you finish the lap
    //        Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

    //        //placement when you finish the lap now on canvas
    //        GameObject orderUiText = RacingGameManager.instance.finisherTextUi[finishOrder - 1];
    //        //Renables the disabled ui finsher text
    //        orderUiText.SetActive(true);

    //        //mean this is you 
    //        if (viewId == photonView.ViewID)
    //        {
    //            //Changes text and color to say the player name and placement
    //            orderUiText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer + "(YOU)";
    //            orderUiText.GetComponent<Text>().color = Color.red;
    //        }
    //        else //this means you aren't the person
    //        {
    //            //Changes text to say the player name and placement
    //            orderUiText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
    //        }



    //    }

    //}
    //// Start is called before the first frame update


    //private void OnTriggerEnter(Collider col)
    //{
    //    //When this object hits an istrigger 
    //    //and that object is found in the  LapController lapTriggers list then...
    //    if (lapTriggers.Contains(col.gameObject))
    //    {
    //        //gets the index in the LapController lapTriggers list 
    //        int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);
    //        //removes the object from the  LapController lapTriggers list 
    //        lapTriggers[indexOfTrigger].SetActive(false);
    //    }

    //    if (col.gameObject.tag == "FinishTrigger")
    //    {
    //        GameFinish();

    //    }
    //}

    //public void GameFinish()
    //{
    //    //removes camera
    //    GetComponent<PlayerSetup>().camera.transform.parent = null;
    //    //removes the vehiclemovement
    //    GetComponent<VehicleMovement>().enabled = false;

    //    //placement of person
    //    finishOrder++;
    //    //gets nickname of the owner
    //    string nickName = photonView.Owner.NickName;
    //    //see the ranking in our canvas
    //    int viewId = photonView.ViewID;

    //    //event data (in this case it tells your name , placement and viewId when you finish)
    //    object[] data = new object[] { nickName, finishOrder, viewId };

    //    //The raise event variables
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
    //    {
    //        Receivers = ReceiverGroup.All,
    //        CachingOption = EventCaching.AddToRoomCache
    //    };

    //    //sender
    //    SendOptions sendOptions = new SendOptions
    //    {
    //        Reliability = false
    //    };

    //    //name of event, data sent, who receives it, how to send it?
    //    PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);

    //}



}
