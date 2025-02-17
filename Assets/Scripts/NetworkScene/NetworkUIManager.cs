using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkUIManager : MonoBehaviour
{
    public static NetworkUIManager Instance;

    private NetworkSceneManager punCallbacks;

    private ConnectionStatus connectionStatus;

    private List<RoomInfo> roomInfos;

    private List<GameObject> panels;


    [Header("Connection Failed UIs")]
    [SerializeField] private GameObject connectFailedPanel;

    [Header("JoinLobbyUIs")]
    [SerializeField] private GameObject joinLobbyUIs;
    [SerializeField] private TMP_Text playerMessageText;
    [SerializeField] private TMP_InputField playerNameField;

    [Header("Host/Join Room")]
    [SerializeField] private GameObject hostOrJoinRoomUIs;
    [SerializeField] private TMP_InputField roomNameField;

    [Header("RoomSelection")]
    [SerializeField] private GameObject roomSelectionUIs;
    [SerializeField] private List<TMP_Text> roomUI_Texts;

    [Header("InRoomUIs")]
    [SerializeField] private GameObject inRoomUIs;
    [SerializeField] private List<TMP_Text> playerDescriptionTexts;
    [SerializeField] private GameObject startGameButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void Start()
    {

        punCallbacks = NetworkSceneManager.Instance;
        roomInfos = new List<RoomInfo>();
        panels = new List<GameObject>();
        panels.Add(hostOrJoinRoomUIs);
        panels.Add(joinLobbyUIs);
        panels.Add(inRoomUIs);
        panels.Add(roomSelectionUIs);
        panels.Add(connectFailedPanel);
        SetConnectionStatus(ConnectionStatus.Connecting);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //audioManager.PlayClickSound();
        }
    }
    public void SetPlayerMessageText(string value)
    {
        playerMessageText.text = value;
    }
    public void SetConnectionStatus(ConnectionStatus status)
    {
        connectionStatus = status;
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(false);
        }

        SetPlayerMessageText("");

        switch (connectionStatus)
        {
            case ConnectionStatus.Connecting:
                playerMessageText.text = "Connecting";
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.AutomaticallySyncScene = true;
                break;
            case ConnectionStatus.ConnectionFailed:
                connectFailedPanel.SetActive(true);
                playerMessageText.text = "Connection Failed. Play Offline?";
                break;
            case ConnectionStatus.Connected:
                playerMessageText.text = "Please Enter a Name";
                joinLobbyUIs.SetActive(true);
                break;
            case ConnectionStatus.HostingOrJoiningRoom:
                playerMessageText.text = "Enter Room Name in Order to Host";
                hostOrJoinRoomUIs.SetActive(true);
                break;
            case ConnectionStatus.InRoomSelection:
                playerMessageText.text = "Select Room To Join";
                roomSelectionUIs.SetActive(true);
                break;
            case ConnectionStatus.InRoom:
                if(!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    playerMessageText.text = "Waiting for Host to Start Game";
                }
                inRoomUIs.SetActive(true);
                break;

        }
    }
    public void JoinLobby()
    {
        if (string.IsNullOrWhiteSpace(playerNameField.text))
        {
            SetPlayerMessageText("Missing: Name!");
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = playerNameField.text;
        PhotonNetwork.JoinLobby();
    }

    public void HostRoom()
    {
        if (string.IsNullOrWhiteSpace(roomNameField.text))
        {
            playerMessageText.text = "Missing: Room Name";
            return;
        }

        if (punCallbacks.RoomInfos.Count > 5)
        {
            playerMessageText.text = "No More Rooms Available";
            return;
        }
        if(PhotonNetwork.CountOfRooms < 6)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsOpen = true;
            PhotonNetwork.CreateRoom(roomNameField.text, roomOptions);
        }
        else
        {
            playerMessageText.text = "No More Rooms Available for Hosting";
        }
    }
    public void JoinRoom()
    {
        if (punCallbacks.RoomInfos.Count == 0)
        {
            playerMessageText.text = "No Rooms Availabe";
            return;
        }
        

        SetConnectionStatus(ConnectionStatus.InRoomSelection);
    }
    public void UpdateRoomUI_Texts()
    {
        roomInfos = punCallbacks.RoomInfos;
        for (int i = 0; i < roomUI_Texts.Count; i++)
        {
            if( i < roomInfos.Count)
            {
                if(roomInfos[i].IsOpen)
                {
                    roomUI_Texts[i].text = "RoomName: " + roomInfos[i].Name + "   PlayerCount: " + roomInfos[i].PlayerCount + " / 4";
                }
                else
                {
                    roomUI_Texts[i].text = "Running Game";
                }
            }
            else
            {
                roomUI_Texts[i].text = "Empty";
            }
        }
    }
    public void JoinRoomSelected(int id)
    {
        if (id >= roomInfos.Count)
        {
            playerMessageText.text = "Room not Active";
            return;
        }
        if(!roomInfos[id].IsOpen)
        {
            playerMessageText.text = "Game Already in Progress";
            return;
        }
        PhotonNetwork.JoinRoom(roomInfos[id].Name);
    }
    public void UpdatePlayerDescriptionTexts()
    {
        for (int i = 0; i < playerDescriptionTexts.Count; i++)
        {
            if (i < PhotonNetwork.CurrentRoom.Players.Count)
            {
                playerDescriptionTexts[i].text = "Player: " + PhotonNetwork.CurrentRoom.Players[i +1].NickName;
            }
            else
            {
                playerDescriptionTexts[i].text = "Player: NPC";
            }
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }
    public void GoBack()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.Disconnect();

        SetConnectionStatus(ConnectionStatus.Connecting);
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        SceneManager.LoadScene(1);
    }
    public void PlayOffline()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
