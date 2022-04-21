using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class EndLapEvent : MonoBehaviourPunCallbacks
{
    private const byte PLAYER_COUNT = 0;
    private int num;
    void Start()
    {
        ////for each gameobject in the RacingGM lapTriggers list
        ////Add it to LapController lapTriggers list
        //foreach (GameObject go in RacingGameManager.instance.lapTriggers)
        //{
        //    lapTriggers.Add(go);
        //}
        num = 1;
    }
    // Update is called once per frame
    void Update()
    {

    }
    //public List<GameObject> lapTriggers = new List<GameObject>();

    public void UpdateFirstText()
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

        PhotonNetwork.NetworkingClient.EventReceived += OnEvents;

    }



    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvents;
    }

    //onevent
    //oneventreceived

    void OnEvents(EventData photonEvent)
    {
        if (photonEvent.Code == PLAYER_COUNT)
        {
            //customdata = the updatekilledtext dat
            object[] data = (object[])photonEvent.CustomData;
            string playerNickName = (string)data[0];
            int playerNumber = (int)data[1];

            int viewId = (int)data[2];

            //write down win code
            GameObject winnerText = FallingGameManager.instance.winnerUIText;

            


                if (viewId == photonView.ViewID)
                {
                //playerNickName +

                    winnerText.GetComponent<Text>().text = playerNickName + " has won ";
                    num++;
                }
            
        }

    }
}
