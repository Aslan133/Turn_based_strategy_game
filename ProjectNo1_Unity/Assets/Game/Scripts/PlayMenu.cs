using System.Collections.Generic;
using System.Data.SQLite;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayMenu : MonoBehaviour
{
    #region Variables
    private Dropdown _player1SelectionDrd;
    private Dropdown _player2SelectionDrd;
    private Dropdown _playerChoiceInEditDrd;
    private Dropdown _playerChoiceInStatsDrd;
    private GameObject _playerEditObj;
    private GameObject _playersScoreboardObj;
    private GameObject _playersStatsObj;
    private Button _mainMenuPlayBtn;
    private Button _editPlayerBtn;
    private Button _addPlayerBtn;
    private Button _deletePlayerBtn;
    private Button _playMenuPlayBtn;
    private Button _scoreboardBtn;
    private Button _playersStatsBtn;
    private Button _exitScoreboardBtn;
    private Button _exitStatsBtn;
    private InputField _newPlayerNameIF;
    private InputField _moneyInitIF;
    private InputField _troopPriceInitIF;
    

    //info for gameplay init
    public static List<PlayerScoreboardInfo> PlayersSelected;
    public static int Money;
    public static int TroopPrice;

    //additional variables
    private List<PlayerScoreboardInfo> _availablePlayers;
    private List<PlayerPlayerStatsInfo> _chosedPlayerStats;

    // stats
    private Transform _scoreboardContainerTr;
    private Transform _scoreboardTemplateTr;
    private GameObject _scoreaboardScrollviewContentObj;

    private Transform _statsContainerTr;
    private Transform _statsTemplateTr;
    private GameObject _statsScrollviewContentObj;
    #endregion

    private void Start()
    {
        //set references
        PlayMenuInit();

        //initial visibility
        _scoreboardTemplateTr.gameObject.SetActive(false);
        _statsTemplateTr.gameObject.SetActive(false);
        _playersScoreboardObj.SetActive(false);
        _playersStatsObj.SetActive(false);
        _playerEditObj.SetActive(false);

        //add listner to GameObjects
        _mainMenuPlayBtn.onClick.AddListener(ReadPlayersDataAndFillDropdowns);
        _editPlayerBtn.onClick.AddListener(OpenPlayerEditWindow);
        _deletePlayerBtn.onClick.AddListener(DeletePlayer);
        _addPlayerBtn.onClick.AddListener(AddPlayer);
        _playMenuPlayBtn.onClick.AddListener(GoToGamePlayScene);
        _scoreboardBtn.onClick.AddListener(FillPlayersScoreboardTable);
        _playersStatsBtn.onClick.AddListener(OpenIndividualStatsTable);
        _exitScoreboardBtn.onClick.AddListener(() => ExitTable(_playersScoreboardObj));
        _exitStatsBtn.onClick.AddListener(() => ExitTable(_playersStatsObj));
        _playerChoiceInStatsDrd.onValueChanged.AddListener(delegate { FillPlayersIndividualStatsTable();});

        //default initial money/troop price
        _moneyInitIF.text = "2000";
        _troopPriceInitIF.text = "50";

        //other
        PlayersSelected = new List<PlayerScoreboardInfo>();
        _availablePlayers = new List<PlayerScoreboardInfo>();
        _chosedPlayerStats = new List<PlayerPlayerStatsInfo>();

        ReadPlayersDataAndFillDropdowns();
    }
    private void ExitTable( GameObject table)
    {
        table.SetActive(false);
    }
    private void FillPlayersScoreboardTable()
    {
        //order players by descending total result
        List<PlayerScoreboardInfo> orderedPlayers = new List<PlayerScoreboardInfo>();
        orderedPlayers.AddRange(_availablePlayers.OrderByDescending(x => x.TotalResult));

        float templateHeight = 40f;
        float totalContentSize = 0;

        for (int i = 0; i < _availablePlayers.Count; i++)
        {
            //creating copy of template and add as new table row

            Transform entryTransform = Instantiate(_scoreboardTemplateTr, _scoreboardContainerTr);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            
            //players ranking

            int rank = i + 1;
            string ranking;
            switch (rank)
            {
                case 1:
                    ranking = "1ST";
                    break;
                case 2:
                    ranking = "2ND";
                    break;
                case 3:
                    ranking = "3RD";
                    break;
                default:
                    ranking = rank + "TH";
                    break;
            }

            //filling row info

            entryTransform.Find("PlayerPosTxt").GetComponent<Text>().text = ranking;
            entryTransform.Find("PlayerNameTxt").GetComponent<Text>().text = orderedPlayers[i].Name;
            entryTransform.Find("PlayerVictoriesTxt").GetComponent<Text>().text = orderedPlayers[i].Victories.ToString();
            entryTransform.Find("PlayerDefeatsTxt").GetComponent<Text>().text = orderedPlayers[i].Defeats.ToString();
            
            //set current row visible
            entryTransform.gameObject.SetActive(true);

            //scrollview content rect height
            totalContentSize += templateHeight;
        }
        //change scrollview content rect height
        RectTransform dd = _scoreaboardScrollviewContentObj.GetComponent<RectTransform>();
        dd.sizeDelta = new Vector2(0, totalContentSize);
        
        //set filled table visible
        _playersScoreboardObj.SetActive(true);
    }
    private void OpenIndividualStatsTable()
    {
        //read data
        ReadPlayersDataAndFillDropdowns();
        //set visible
        _playersStatsObj.SetActive(true);
        //fill
        FillPlayersIndividualStatsTable();
    }
    private void FillPlayersIndividualStatsTable()
    {
        ReadChosedPlayerStats(_availablePlayers[_playerChoiceInStatsDrd.value].ID);

        float templateHeight = 40f;
        float totalContentSize = 0;

        //clear scrollview content container - destroy all childs except first - it is template
        int j = 0;
        foreach (Transform child in _statsContainerTr)
        {
            if (j != 0)
            {
                GameObject.Destroy(child.gameObject);
            }
            j++;
        }

        for (int i = 0; i < _chosedPlayerStats.Count; i++)
        {
            //creating copy of template and add as new table row
            Transform entryTransform = Instantiate(_statsTemplateTr, _statsContainerTr);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);

            //filling row info
            entryTransform.Find("GameNoTxt").GetComponent<Text>().text = (i+1).ToString();
            entryTransform.Find("StateTxt").GetComponent<Text>().text = _chosedPlayerStats[i].State;
            entryTransform.Find("TotalMoneyTxt").GetComponent<Text>().text = _chosedPlayerStats[i].Total_money.ToString();
            entryTransform.Find("MoneyAtTheEndTxt").GetComponent<Text>().text = _chosedPlayerStats[i].Money_at_the_end.ToString();
            entryTransform.Find("TotalTroopsTxt").GetComponent<Text>().text = _chosedPlayerStats[i].Total_troops.ToString();
            entryTransform.Find("TroopsAtTheEndTxt").GetComponent<Text>().text = _chosedPlayerStats[i].Troops_at_the_end.ToString();
            entryTransform.Find("TroopsLostTxt").GetComponent<Text>().text = _chosedPlayerStats[i].Troops_lost.ToString();

            //set visible
            entryTransform.gameObject.SetActive(true);

            //scrollview content rect height
            totalContentSize += templateHeight;
        }
        //change scrollview content rect height
        RectTransform dd = _statsScrollviewContentObj.GetComponent<RectTransform>();
        dd.sizeDelta = new Vector2(0, totalContentSize);
    }
    private void PlayMenuInit()
    {
        //statistics objects init
        //scoreboard
        _scoreaboardScrollviewContentObj = GameObject.Find("Canvas/Menu/PlayMenu/PlayersScoreboard/ScoreboardTable/Viewport/Content");
        _scoreboardContainerTr = GameObject.Find("Canvas/Menu/PlayMenu/PlayersScoreboard/ScoreboardTable/Viewport/Content/ScoreboardContainer").transform;
        _scoreboardTemplateTr = GameObject.Find("Canvas/Menu/PlayMenu/PlayersScoreboard/ScoreboardTable/Viewport/Content/ScoreboardContainer/ScoreboardTemplate").transform;

        //statistics
        _statsScrollviewContentObj = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats/StatsTable/Viewport/Content");
        _statsContainerTr = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats/StatsTable/Viewport/Content/StatsContainer").transform;
        _statsTemplateTr = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats/StatsTable/Viewport/Content/StatsContainer/PlayerStatsTemplate").transform;

        //GameObjects init
        _player1SelectionDrd = GameObject.Find("Canvas/Menu/PlayMenu/PlayersNo1Drd").GetComponent<Dropdown>();
        _player2SelectionDrd = GameObject.Find("Canvas/Menu/PlayMenu/PlayersNo2Drd").GetComponent<Dropdown>();
        _playerChoiceInEditDrd = GameObject.Find("Canvas/Menu/PlayMenu/PlayerEdit/PlayerChoiceInEditDrd").GetComponent<Dropdown>();
        _playerChoiceInStatsDrd = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats/PlayersStatsDrd").GetComponent<Dropdown>();
        _playerEditObj = GameObject.Find("Canvas/Menu/PlayMenu/PlayerEdit");
        _playersScoreboardObj = GameObject.Find("Canvas/Menu/PlayMenu/PlayersScoreboard");
        _playersStatsObj = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats");
        _mainMenuPlayBtn = GameObject.Find("Canvas/Menu/MainMenu/PlayBtn").GetComponent<Button>();
        _editPlayerBtn = GameObject.Find("Canvas/Menu/PlayMenu/EditPlayerBtn").GetComponent<Button>();
        _addPlayerBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayerEdit/AddPlayerBtn").GetComponent<Button>();
        _deletePlayerBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayerEdit/DeleteUserBtn").GetComponent<Button>();
        _playMenuPlayBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayBtn").GetComponent<Button>();
        _scoreboardBtn = GameObject.Find("Canvas/Menu/PlayMenu/ScoreboardBtn").GetComponent<Button>();
        _playersStatsBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStatsBtn").GetComponent<Button>();
        _exitScoreboardBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayersScoreboard/ExitScoreboardBtn").GetComponent<Button>();
        _exitStatsBtn = GameObject.Find("Canvas/Menu/PlayMenu/PlayersStats/ExitStatsBtn").GetComponent<Button>();
        _newPlayerNameIF = GameObject.Find("Canvas/Menu/PlayMenu/PlayerEdit/NewPlayerNameIF").GetComponent<InputField>();
        _moneyInitIF = GameObject.Find("Canvas/Menu/PlayMenu/MoneyInitIF").GetComponent<InputField>();
        _troopPriceInitIF = GameObject.Find("Canvas/Menu/PlayMenu/TroopPriceInitIF").GetComponent<InputField>();
    }
    public void OpenPlayerEditWindow()
    {
        _playerEditObj.SetActive(true);
        _newPlayerNameIF.text = "";
        ReadPlayersDataAndFillDropdowns();
    }
    public void ClickedEmptyBtn()
    {
        //clicked outside window - close it
        _playerEditObj.SetActive(false);
    }
    static SQLiteConnection CreateConnection()
    {
        //create path to database and connection string
        string relativePath = @"Assets\Plugins\mydb.db";
        string currentPath;
        string absolutePath;
        currentPath = Path.GetDirectoryName(Application.dataPath);
        absolutePath = currentPath + @"\" + relativePath;
        string ConnectionSt = "Data Source="+ absolutePath;
        SQLiteConnection sqlite_conn;

        // Create a new database connection:
        sqlite_conn = new SQLiteConnection(ConnectionSt);

        // Open the connection:
        try
        {
            sqlite_conn.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        return sqlite_conn;
    }
    public void AddPlayer()
    {
        if (!string.IsNullOrEmpty(_newPlayerNameIF.text))
        {
            InsertNewPlayerToDatabase(CreateConnection(), _newPlayerNameIF.text);
            _playerEditObj.SetActive(false);
            ReadPlayersDataAndFillDropdowns();
        }
    }
    private void DeletePlayer()
    {
        DeleteSelectedPlayer(CreateConnection(), _availablePlayers[_playerChoiceInEditDrd.value].ID, _availablePlayers[_playerChoiceInEditDrd.value].Name);
        _playerEditObj.SetActive(false);
        ReadPlayersDataAndFillDropdowns();
    }
    public void ReadPlayersDataAndFillDropdowns()
    {
        //read and fill list of available players
        _availablePlayers.Clear();
        _availablePlayers.AddRange(ReadAndReturnPlayersList(CreateConnection()));

        //clear dropdowns for new info
        _player1SelectionDrd.ClearOptions();
        _player2SelectionDrd.ClearOptions();
        _playerChoiceInEditDrd.ClearOptions();
        _playerChoiceInStatsDrd.ClearOptions();

        //temp list of names
        List<string> temp_list = new List<string>();
        foreach (var player in _availablePlayers)
        {
            temp_list.Add(player.Name);
        }

        //fill dropdowns
        _player1SelectionDrd.AddOptions(temp_list);
        _player2SelectionDrd.AddOptions(temp_list);
        _playerChoiceInEditDrd.AddOptions(temp_list);
        _playerChoiceInStatsDrd.AddOptions(temp_list);
    }
    public void ReadChosedPlayerStats(int ID)
    {
        //read and fill chosed player stats list
        _chosedPlayerStats.Clear();
        var chosedPlayer = _availablePlayers.Where(x => x.ID == ID).First();
        _chosedPlayerStats.AddRange(ReadPlayerStatsAndReturnIt(CreateConnection(), chosedPlayer.Name + "_" + chosedPlayer.ID));
    }
    public void GoToGamePlayScene()
    {
        //add chosed player 1 and player 2 to list - it goes to "gameplay" init
        PlayersSelected.Clear();
        PlayersSelected.Add(_availablePlayers[_player1SelectionDrd.value]);
        PlayersSelected.Add(_availablePlayers[_player2SelectionDrd.value]);

        //check if info correct to start game
        if (PlayersSelected.Count > 0
            && PlayersSelected[0].ID != PlayersSelected[1].ID
            && int.TryParse(_moneyInitIF.text, out Money)
            && int.TryParse(_troopPriceInitIF.text, out TroopPrice))
        {
            SceneManager.LoadScene("GamePlay");
        }
    }
    public void InsertNewPlayerToDatabase(SQLiteConnection conn, string name)
    {
        //first - check if it already exist, second - add as new

        string name_temp = "";
        List<string> namesList = new List<string>();

        //generate list of names
        foreach (var player in _availablePlayers)
        {
            namesList.Add(player.Name);
        }

        //if name already exist - add index to name
        if (namesList.Contains(name))
        {
            int i = 1;
            foreach (var item in namesList)
            {
                if (item == name)
                {
                    i++;
                }
            }
            name_temp = name + i;
        }
        else
        {
            name_temp = name;
        }

        //insert new player
        SQLiteCommand sqlite_cmd = conn.CreateCommand();
        sqlite_cmd.CommandText = "INSERT INTO Scoreboard (Name, Victories, Defeats) VALUES('" + name_temp + "', 0, 0); ";
        sqlite_cmd.ExecuteNonQuery();

        conn.Close();
    }
    private List<PlayerScoreboardInfo> ReadAndReturnPlayersList(SQLiteConnection conn)
    {

        List<PlayerScoreboardInfo> list_temp = new List<PlayerScoreboardInfo>();

        SQLiteDataReader sqlite_datareader;
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = conn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM Scoreboard";
        sqlite_datareader = sqlite_cmd.ExecuteReader();

        while (sqlite_datareader.Read())
        {
            list_temp.Add(new PlayerScoreboardInfo() { 
                ID = Convert.ToInt32(sqlite_datareader["ID"]), 
                Name = sqlite_datareader["Name"].ToString(), 
                Victories = Convert.ToInt32(sqlite_datareader["Victories"]), 
                Defeats = Convert.ToInt32(sqlite_datareader["Defeats"]), 
                TotalResult = Convert.ToInt32(sqlite_datareader["Victories"]) - Convert.ToInt32(sqlite_datareader["Defeats"]) });
        }
        conn.Close();

        return list_temp;
    }
    private List<PlayerPlayerStatsInfo> ReadPlayerStatsAndReturnIt(SQLiteConnection conn, string tableName)
    {

        List<PlayerPlayerStatsInfo> list_temp = new List<PlayerPlayerStatsInfo>();

        SQLiteDataReader sqlite_datareader;
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = conn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM " + tableName;
        sqlite_datareader = sqlite_cmd.ExecuteReader();

        while (sqlite_datareader.Read())
        {
            list_temp.Add(new PlayerPlayerStatsInfo() { 
                ID = Convert.ToInt32(sqlite_datareader["ID"]), 
                State = sqlite_datareader["State"].ToString(), 
                Total_money = Convert.ToInt32(sqlite_datareader["Total_money"]), 
                Money_at_the_end = Convert.ToInt32(sqlite_datareader["Money_at_the_end"]), 
                Total_troops = Convert.ToInt32(sqlite_datareader["Total_troops"]), 
                Troops_at_the_end = Convert.ToInt32(sqlite_datareader["Troops_at_the_end"]), 
                Troops_lost = Convert.ToInt32(sqlite_datareader["Troops_lost"]) });
        }
        conn.Close();

        return list_temp;
    }
    private void DeleteSelectedPlayer(SQLiteConnection conn, int id, string tableName)
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = conn.CreateCommand();

        //delete from scoreboard
        sqlite_cmd.CommandText = "DELETE FROM Scoreboard WHERE ID = " + id.ToString();
        sqlite_cmd.ExecuteNonQuery();

        //delete player stats table
        sqlite_cmd.CommandText = "DROP TABLE IF EXISTS " + tableName + "_" + id + ";";
        sqlite_cmd.ExecuteNonQuery();

        conn.Close();
    }
}
public struct PlayerScoreboardInfo
{
    public int ID;
    public string Name;
    public int Victories;
    public int Defeats;
    public int TotalResult;
}
public struct PlayerPlayerStatsInfo
{
    public int ID;
    public string State;
    public int Total_money;
    public int Money_at_the_end;
    public int Total_troops;
    public int Troops_at_the_end;
    public int Troops_lost;
}