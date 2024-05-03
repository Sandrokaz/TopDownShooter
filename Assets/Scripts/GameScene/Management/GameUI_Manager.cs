using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameUI_Manager : MonoBehaviour
{
    public static GameUI_Manager Instance;

    private GameState gameState;

    private GameManager gameManager;
    public GameManager GameManager { get => gameManager; set => gameManager = value; }

    private List<GameObject> panels;

    [SerializeField] private GameObject mainCamera;
    public GameObject MainCamera { get => mainCamera; set => mainCamera = value; }

    private float gameTime = 120;
    private float gameTimePassed;

    [Header("Team Selection UIs")]
    [SerializeField] private GameObject teamSelectionPanel;
    [SerializeField] private List<TMP_Text> teamSelectButtonTexts;
    [SerializeField] private TMP_Text playerMessageText;
    [SerializeField] private GameObject startButton;
    public TMP_Text PlayerMessageText { get => playerMessageText; set => playerMessageText = value; }
    public GameObject StartButton { get => startButton; set => startButton = value; }


    [Header("Room UIs")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject gameUIs;
    [SerializeField] private TMP_Text gameTimeTxt;

    [Header("Pause Menu UIs")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Game Over UIs")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private List<TMP_Text> entityKillCountTexts;
    [SerializeField] private TMP_Text winnerText;
    public TMP_Text WinnerText { get => winnerText; set => winnerText = value; }

    [SerializeField] private GameObject respawnPanel;


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
    private void Start()
    {
        gameTimePassed = gameTime;
        panels = new List<GameObject>();
        panels.Add(teamSelectionPanel);
        panels.Add(roomPanel);
        panels.Add(pauseMenuPanel);
        panels.Add(respawnPanel);
        panels.Add(gameOverPanel);
        panels.Add(gameUIs);
        DeactivatePanels();
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            SetGameState(GameState.Paused);
        }

        if(GetGameState() == GameState.Running || GetGameState() == GameState.Paused || GetGameState() == GameState.Respawning)
        {
            gameTimePassed -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(gameTimePassed / 60);
            int seconds = Mathf.FloorToInt(gameTimePassed % 60);

            if (minutes < 0)
                return;

            gameTimeTxt.text = minutes + " : " + seconds;
        }
    }
    public void SetGameState(GameState _gameState)
    {
        DeactivatePanels();
        gameState = _gameState;
        UpdateUI_Panels();
    }
    private void UpdateUI_Panels()
    {
        switch(gameState)
        {
            case GameState.TeamSelection:
                playerMessageText.text = "Choose A Team";
                teamSelectionPanel.SetActive(true);
                break;
            case GameState.Running:
                MainCamera.SetActive(false);
                gameUIs.SetActive(true);
                roomPanel.SetActive(true);
                break;
            case GameState.Paused:
                pauseMenuPanel.SetActive(true);
                break;
            case GameState.GameOver:
                MainCamera.SetActive(true);
                gameOverPanel.SetActive(true);
                break;
            case GameState.Respawning:
                respawnPanel.SetActive(true);
                mainCamera.SetActive(true);
                break;
        }
    }
    private void DeactivatePanels()
    {
        playerMessageText.text = "";
        for(int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(false);
        }
    }
    public void UpdateTeamSelectUIs()
    {
        for(int i = 0; i < 4; i++)
        {
            teamSelectButtonTexts[i].text = gameManager.GetPlayers()[i];
        }
    }
    public void SetGameOverTexts(List<string> playerNames, List<int> _entityKillCounts)
    {
        for(int i=0; i<entityKillCountTexts.Count; i++)
        {
            entityKillCountTexts[i].text = playerNames[i] + " Killcount: " + _entityKillCounts[i];

        }
    }
    public void ConfigPlayer(int number)
    {
        string localName = PhotonNetwork.LocalPlayer.NickName;
        gameManager.SetPlayer(localName, number);
    }
    public void Resume()
    {
        SetGameState(GameState.Running);
    }
    public void Quit()
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
    public void StartGame()
    {
        if(gameManager.GetAssignedPlayerCount() != PhotonNetwork.CurrentRoom.Players.Count)
        {
            playerMessageText.text = "Waiting on Other Players to join Team";
            return;
        }

        gameManager.StartGame();

        StartCoroutine(GameTimesUp(gameTime));
    }
    public GameState GetGameState()
    {
        return gameState;
    }
    public List<TMP_Text> GetTeamSelectionButtonTexts()
    {
        return teamSelectButtonTexts;
    }
    private IEnumerator GameTimesUp( float _gameTime)
    {
        yield return new WaitForSeconds(_gameTime);
        gameManager.GameOver();
        mainCamera.SetActive(true);
        StopAllCoroutines();
    }

}
