using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;
    public Text GameModeText;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        //loads the master client scene
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        //activate the panel that says creating game... 
        ActivatePanel(CreatingRoomInfoUIPanel.name);
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }

            RoomOptions roomOptions = new RoomOptions();
            string[] roomPropertiesInLobby = { "gm" };//gamemode

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            PhotonNetwork.CreateRoom(roomName, roomOptions);

        }

    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        GameMode = gameMode;

        //From the hastable
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

        //0 is means no max number of players
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        //Note if there no room available use Photon Callback OnJoinRandomFailed
    }

    public void OnBackButtonClicked()
    {
        //Back button for choosing vehicle
        ActivatePanel(GameOptionsUIPanel.name);

    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();

    }

    public void OnStartGameButtonClicked()
    {
        //when you start the game 
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))//racing
            {
                PhotonNetwork.LoadLevel("FallingScene");

            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))//deathrace
            {
                PhotonNetwork.LoadLevel("DeathRaceScene");
            }



        }

    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created!");
    }
    public override void OnJoinedRoom()
    {
        //When the player successfully joins a room
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined the " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        //activates the inside room panel
        ActivatePanel(InsideRoomUIPanel.name);

        object gameModeName;
        //needs an object type
        //out the objects name
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
        {

            Debug.Log(gameModeName.ToString());
            //Updates the room number
            RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
                + PhotonNetwork.CurrentRoom.MaxPlayers;
            //changes text to these names
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                GameModeText.text = "Falling Game Mode";
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                GameModeText.text = "Death Race Mode";
            }

        }

        if (playerListGameObjects == null)
        {

            playerListGameObjects = new Dictionary<int, GameObject>();

        }

        //for each player in the room 
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //spawn the playerlist item (the one with the check mark)
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            //position
            playerListItem.transform.SetParent(PlayerListParent.transform);
            //scaled
            playerListItem.transform.localScale = Vector3.one;


            //Gets the item player id and nickname
            playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(player.ActorNumber, player.NickName);

            //see if player is ready and the check mark
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool)isPlayerReady);

            }

            //adds to dictionary
            playerListGameObjects.Add(player.ActorNumber, playerListItem);
        }

        //makes the button for start disappear
        StartGameButton.SetActive(false);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //spawn the playerlist item (the one with the check mark)
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        //position
        playerListItem.transform.SetParent(PlayerListParent.transform);
        //scaled
        playerListItem.transform.localScale = Vector3.one;
        //Gets the item player id and nickname
        playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        //adds to dictionary
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        //the start room button will appear if everyone in that function it true
        StartGameButton.SetActive(CheckAllPlayersReady());

    }



    //Used to update the list for when other players  in the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //destroys objects
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        //remove from dictionary
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    public override void OnLeftRoom()
    {
        //opens this panel
        ActivatePanel(GameOptionsUIPanel.name);

        //Destroys
        foreach (GameObject playerlistGameObject in playerListGameObjects.Values)
        {
            Destroy(playerlistGameObject);
        }

        //Clears the dicitionary and makes it null
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //plays no match found
        Debug.Log(message);

        //Then creates a room depending on the choosen GameMode name
        //null guard
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }
            //Room options object
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            //the hashtable key
            string[] roomPropertiesInLobby = { "gm" }; //gm = gamemode

            //Game modes
            //gamemodes
            //rc = racing "öld"
            //dr = death race
            //This sets the gm key to the value of Gamemode 
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };



            //roomoptions
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;


            //Create a room in Photon
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool)isPlayerReady);
            }

        }
        //Checks if everyone is ready
        StartGameButton.SetActive(CheckAllPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayersReady());

        }
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }
    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods
    private bool CheckAllPlayersReady()
    {
        //Checks if you are the master if not then no start button
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        //Checks if everyone ready
        foreach (Player p in PhotonNetwork.PlayerList)
        {

            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;

                }

            }
            else
            {
                return false;
            }

        }

        return true;
    }


    #endregion
}
