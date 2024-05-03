using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable
{
	public List<EntityBase> activeEntities = new List<EntityBase>();
	private GameUI_Manager uiManager;
    private List<string> entityNames;
    private List<int> killCounts;
    private int localPlayerID = 5;

    void Start()
    {
        uiManager = GameUI_Manager.Instance;
        uiManager.GameManager = this;
        entityNames = new List<string>();
        killCounts = new List<int>();
        
        for(int i = 0; i < 4; i ++)
        {
            entityNames.Add("NPC");
            killCounts.Add(0);
        }
    }
    public void SetPlayer(string _name, int _id)
    {
        if (entityNames[_id] != "NPC")
            return;

        for(int i = 0; i < entityNames.Count; i++)
        {
            if(entityNames[i].Equals(_name))
            {
                photonView.RPC("RPC_SetPlayer", RpcTarget.All, "NPC", i);
            }
        }
        photonView.RPC("RPC_SetPlayer", RpcTarget.All, _name, _id);

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            uiManager.PlayerMessageText.text = "Waiting For Host to Start Game";
        }
    }
    [PunRPC]
    public void RPC_SetPlayer(string _name, int _id)
    {
        entityNames[_id] = _name;
        uiManager.UpdateTeamSelectUIs();
    }

    public void StartGame()
    {
        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_StartGame()
    {

        uiManager.SetGameState(GameState.Running);
        uiManager.MainCamera.SetActive(false);


        localPlayerID = GetPlayerID(PhotonNetwork.LocalPlayer.NickName);
        SpawnPlayer();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            for(int i = 0; i < 4; i++)
            {
                if(entityNames[i] == "NPC")
                {
                    SpawnNPC(i);
                }
            }
        }
    }
    private void SpawnPlayer()
    {
        GameObject _playerObj = PhotonNetwork.Instantiate("Player", CustomRoom.Instance.SpawnPoints[localPlayerID].position, Quaternion.identity);
        PlayerController _playerScript = _playerObj.GetComponent<PlayerController>();
        _playerScript.SetID(localPlayerID);
    }
    private void SpawnNPC(int _id)
    {
        GameObject _NPC_Obj = PhotonNetwork.Instantiate("NPC", CustomRoom.Instance.SpawnPoints[_id].position, Quaternion.identity);
        NPC _NPC_Script = _NPC_Obj.GetComponent<NPC>();
        _NPC_Script.SetID(_id);
        _NPC_Script.EntityName = "NPC";
    }
    public void RemovePlayerThatLeft()
    {
        for(int i = 0; i < 4; i++)
        {
            if(entityNames[i] != "NPC")
            {
                for(int j = 0; j < PhotonNetwork.CurrentRoom.Players.Count; j++)
                {
                    if(entityNames[i] == PhotonNetwork.CurrentRoom.Players[j].NickName)
                    {
                        continue;
                    }

                    SetPlayer("NPC", i);
                }
            }
        }
    }
    public int GetAssignedPlayerCount()
    {
        int _count = 0;
        for(int i = 0; i < 4; i++)
        {
            if (entityNames[i] != "NPC")
                _count++;
        }
        return _count;
    }
    public List<string> GetPlayers()
    {
        return entityNames;
    }
    public int GetPlayerID(string _name)
    {
        for(int i = 0; i < entityNames.Count; i++)
        {
            if (entityNames[i].Equals(_name))
                return i;
        }
        return 6;
    }
    public void EntityDead(int _id)
    {

        if(localPlayerID == _id)
        uiManager.SetGameState(GameState.Respawning);
        photonView.RPC(nameof(RPC_EntityDead), RpcTarget.All,_id);
    }

    [PunRPC]
    public void RPC_EntityDead(int _id)
    {
        StartCoroutine(YieldRespawn(5, _id));
    }
    private IEnumerator YieldRespawn(float _time, int _id)
    {
        yield return new WaitForSeconds(_time);

        if (uiManager.GetGameState() != GameState.GameOver)
        {
            
            if(PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if(entityNames[_id] == "NPC")
                {
                    SpawnNPC(_id);
                }
            }
            if(localPlayerID == _id)
            {
                SpawnPlayer();
            }
        }


        StopCoroutine(YieldRespawn(_time,_id));
    }
    public void SetKillCount(int _id)
    {
        photonView.RPC(nameof(RPC_SetKillCount), RpcTarget.All, _id);
    }
    [PunRPC]
    public void RPC_SetKillCount(int _id)
    {
        killCounts[_id]++;
        Debug.Log("Player: " + _id + "Kills: " + killCounts[_id]);
    }
    public void GameOver()
    {
        int teamAKills = killCounts[0] + killCounts[1];
        int teamBKills = killCounts[2] + killCounts[3];
        string winnerTeam;

        if(teamAKills > teamBKills)
        {
            winnerTeam = "Blue";
        }
        else if(teamAKills == teamBKills)
        {

            winnerTeam = "Tie";
        }
        else
        {
            winnerTeam = "Red";

        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            for(int i= 3; i >= 0; i--)
            {
                if(activeEntities[i] != null)
                PhotonNetwork.Destroy(activeEntities[i].gameObject);
            }
        }
        photonView.RPC(nameof(RPC_GameOver), RpcTarget.All,  winnerTeam);

    }
    [PunRPC]
    public void RPC_GameOver(string _winTeam)
    {
        StopAllCoroutines();
        uiManager.SetGameState(GameState.GameOver);
        uiManager.SetGameOverTexts(entityNames, killCounts);
        uiManager.WinnerText.text = "Winner" + _winTeam;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
