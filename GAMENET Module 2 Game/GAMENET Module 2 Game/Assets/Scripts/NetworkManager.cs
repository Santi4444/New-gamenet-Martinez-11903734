using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//for photon
using Photon.Pun;
//for photon room optiona
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //seen in netowrk manager script in unity above playername input
    [Header("Connection Status Panel")]
    public Text connectionStatusText;


    //seen in netowrk manager script in unity above playername input
    [Header("Login UI Panel")]
    public InputField playerNameInput;

    //gameobjects for panels
    public GameObject loginUiPanel;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header(" Create Room Panel")]
    public GameObject createRoomPanel;
    //input for room name
    public InputField roomNameInputField;
    //input field for count input field
    public InputField playerCountInputField;

    [Header(" Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header(" Show Room List Panel")]
    public GameObject showRoomListPanel;

    [Header(" Inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    //the parent is so that the list item prefab can be a child of the parent
    public GameObject playerListViewParent;
    public GameObject startGameButton;

    [Header(" Room List Panel")]
    public GameObject roomListPanel;
    public GameObject roomItemPrefab;
    //the parent is so that the item prefab can be a child of the parent
    public GameObject roomListParent;



    //Dictionary accepts 2 objectss a key and a value
    //so here the key is the string and the value is the Room info
    //used to keep a list of room names 
    private Dictionary<string, RoomInfo> cacheRoomList;
    //we also need to cache the gameObjects
    private Dictionary<string, GameObject> roomListGameObjects;
    //cache for a onplayerjoined entering a room update
    private Dictionary<int, GameObject> playerListGameObjects;


    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        //adds 1 for the dicitionary
        cacheRoomList = new Dictionary<string, RoomInfo>();
        //inatilize  makes this into a dictionary
        roomListGameObjects = new Dictionary<string, GameObject>();

        //activate login panel
        ActivatePanel(loginUiPanel);

        //when a player joins a room will load the same level 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion


    #region UI Callbacks

    //when you click the login button 
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {

            Debug.Log("Player name is invalid!!!");

        }
        else
        {

            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();

        }

    }

    //when you click the create room button 
    public void OnCreateRoomButtonClicked()
    {

        string roomName = roomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            //when you create a room and it has no name
            roomName = "Room " + Random.Range(1000, 10000);

        }
        // create the room options like max players
        RoomOptions roomOptions = new RoomOptions();
        // can only be done in int so turn to byte for room options
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);

        //cretes the room
        PhotonNetwork.CreateRoom(roomName, roomOptions);

    }

    //when you click the cancel  button 
    public void onCancelButtonClicked()
    {
        //returns you to game options panel
        ActivatePanel(gameOptionsPanel);

    }

    //when you click the show room list button 
    public void onShowRoomListButtonClicked()
    {
        //if we are not in a lobby
        if (!PhotonNetwork.InLobby)
        {
            //join a lobby
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(showRoomListPanel);
    }

    //when the back button is clicked
    public void OnBackButtonClicked()
    {
        //leave lobby and activate game options panel
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(gameOptionsPanel);
    }

    //when the on leave game button is clicked
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        //will also call the onleftroom() automatically
    }

    //when you click the on join random room button
    public void OnJoinRandomRoomClicked()
    {
        //activate the random room panel and joins a room if there is one
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();

    }

    //when you press the start game button
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion


    #region PUN Callbacks

    public override void OnConnected()
    {
        //conneceted to internet
        Debug.Log("Connected to the internet");
    }

    public override void OnConnectedToMaster()
    {
        //double check connected to photon servers
        
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to Photon Servers");
        //Activate game options panel
        ActivatePanel(gameOptionsPanel);
    }
    public override void OnCreatedRoom()
    {
        //when the room is created
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created");
    }
    
    public override void OnJoinedRoom()
    {
        //when the one joins a room
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name);
        //activate the inside room panel
        ActivatePanel(insideRoomPanel);

        //updates the roominfotext in the inside room panel
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        //to prevent a null reference exeption 
        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        //Display the player list
        //for each player in the player list instantiate this prefab
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab);
            //sets the instatiated object to the parent
            playerItem.transform.SetParent(playerListViewParent.transform);
            //(1,1,1)
            playerItem.transform.localScale = Vector3.one;
            //makes the prefab have the player name
            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            //helps indicate if the prefab is yours or a another player
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);


            playerListGameObjects.Add(player.ActorNumber, playerItem);
            
        }
        

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Removes all the prefab gameObjects so you can update them
        ClearRoomListGameObjects();
        Debug.Log("OnRoomListUpdate is called");

        //the start game button will work if you are the host only
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);



        //when a room is updated in a lobby this function will be called and will return a list of all room info in the lobby
        //OnRoomListUpdate() : called when any update the room-listing while in a lobby

        //for each room info in the room list it will... 
        foreach (RoomInfo info in roomList)
        {
            //shows the room name
            Debug.Log(info.Name);

            //to prevent a duplicated room to the dictionary
            // if the room is not open, visble or is removed from the list
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                //if in the dictionary there a key with the same key/name it removes it
                if (cacheRoomList.ContainsKey(info.Name))
                {
                    cacheRoomList.Remove(info.Name);
                }
            }
            else
            {
                //update existing room info
                if (cacheRoomList.ContainsKey(info.Name))
                {
                    cacheRoomList[info.Name] = info;
                }
                //adds to the cachedRoomList dictionary the room name and its options
                cacheRoomList.Add(info.Name, info);
            }


        
        }

        //will have the updated room info list
        foreach (RoomInfo info in cacheRoomList.Values)
        {
            //use the prefab of roomItemPrefab and makes it a child of roomListParent.transform
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            //makes the prefab appear at 1,1,1
            listItem.transform.localScale = Vector3.one;


            //finds the name of this child of the prefab and updates the room name
            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            //finds the name of this child of the prefab and updates the room player count
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player Count: "+ info.PlayerCount
                + "/" + info.MaxPlayers;
            //adds a listener for the button and joins the chosen room
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomClicked(info.Name));

            //adds to gameObject dictionary the prefab of the show list
            roomListGameObjects.Add(info.Name, listItem);
        }
    }


    //when you leave the lobby
    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cacheRoomList.Clear();
    }
    //used in the inside room panel
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Used whenever a player enters a room

        //updates the roominfotext prefab
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;


        GameObject playerItem = Instantiate(playerListItemPrefab);
        //sets the instatiated object to the parent
        playerItem.transform.SetParent(playerListViewParent.transform);
        //(1,1,1)
        playerItem.transform.localScale = Vector3.one;
        //makes the prefab have the player name
        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        //helps indicate if the prefab is yours or a another player
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);


        playerListGameObjects.Add(newPlayer.ActorNumber, playerItem);

    }

    //used in the inside room panel
    //when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //if the host leaves it will make the next player the host
        //the start game button will work if you are the host only
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        //updates the roominfotext prefab
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        //when a player leaves the room the room will destory the prefab and remove the gameobject from the dictionary

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        //when you leave the room you destroy all the gameobjects in the dictionary, 
        //clear the list, makes it null, and activates the gameoptions panel
        foreach (var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        ActivatePanel(gameOptionsPanel);

    }

    //when there are no random rooms to join it will do this
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //gives a warning
        Debug.LogWarning(message);

        //creates room with preset options
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region Private Methods

    //when you click a join button in the show room list panel item prefab
    //used in the showlist item prefab to join a room with the same room Name
    private void OnJoinRoomClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            //leaves the lobby when you join a room
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    
    }

    //when chosen it will destroy all item prefabs in the dictionary for the show list
    private void ClearRoomListGameObjects()
    {
        foreach (var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }

    #endregion



    #region Public Methods

    public void ActivatePanel(GameObject panelToBeActivated)
    {
        //activate panels you want when chosen/clicked
        loginUiPanel.SetActive(panelToBeActivated.Equals(loginUiPanel));
        gameOptionsPanel.SetActive(panelToBeActivated.Equals(gameOptionsPanel));
        createRoomPanel.SetActive(panelToBeActivated.Equals(createRoomPanel));
        joinRandomRoomPanel.SetActive(panelToBeActivated.Equals(joinRandomRoomPanel));
        showRoomListPanel.SetActive(panelToBeActivated.Equals(showRoomListPanel));
        insideRoomPanel.SetActive(panelToBeActivated.Equals(insideRoomPanel));
        roomListPanel.SetActive(panelToBeActivated.Equals(roomListPanel));



    }


    #endregion
}
