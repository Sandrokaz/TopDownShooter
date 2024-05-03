using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    private GameUI_Manager uiManager;
    void Start()
    {
        uiManager = GameUI_Manager.Instance;
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            uiManager.SetGameState(GameState.TeamSelection);
        }

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.CreateRoom("OfflineRoom");
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = "OfflineJeff";
        uiManager.SetGameState(GameState.TeamSelection);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            uiManager.StartButton.SetActive(true);

        PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SceneManager.LoadScene(0);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (uiManager.GetGameState() != GameState.TeamSelection)
            return;

        uiManager.GameManager.RemovePlayerThatLeft();
    }
}
