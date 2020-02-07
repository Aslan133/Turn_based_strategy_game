using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Data;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    private Game _game;
    //public Text resol;
    void Start()
    {
        _game = new Game();
    }
    
    void Update()
    {
        _game.Moving();
        _game.Buying();
        _game.FindCurrentArea();
        _game.HideDifferentAreaTypeElements();
        _game.AreasColor();
        _game.AreasTroopsAndMoneyInfoUpdate();
        _game.AlwaysOnTopObject();
        _game.UpdateAreasVisibleInfo();
        _game.UpdatePlayersTotalTroops();
        _game.GameStateCheck();
        //resol.text = PlayerPrefs.GetInt("resolutionIndex").ToString();
    }
    public void AreaTroopNumVisibileTrue(int i)
    {
        _game.Areas[i].NumOfTotalTroopsInAreaTxt.gameObject.SetActive(true);
    }
    public void AreaTroopNumVisibileFalse(int i)
    {
        _game.Areas[i].NumOfTotalTroopsInAreaTxt.gameObject.SetActive(false);
    }
}

[Serializable]
public class Troop
{
    public int NumberOfTroops;
    public bool MovedThisTurn;
    public Troop(int num, bool moved)
    {
        NumberOfTroops = num;
        MovedThisTurn = moved;
    }
}
public class PlayerIdentificationInfo
{
    public string Name { get; }
    public int ID { get; }
    public PlayerIdentificationInfo(string Name, int ID)
    {
        this.Name = Name;
        this.ID = ID;
    }
}
public class Area
{
    #region AreaObj
    public Button AreaBtn;
    public Button MoveBtn;
    public Button TroopBtn;
    public Button UpgradeBtn;
    public Button AbandonBtn;
    public Button TroopsBuyBtn;
    public Button TroopsDivideBtn;
    public Button TroopsUpgradeBtn;
    public Button NewGroupTrueBtn;
    public Button NewGroupFalseBtn;
    public Button SoldiersToBuyUpBtn;
    public Button SoldiersToBuyDownBtn;
    public Button BuySoldiersBtn;
    public Button AbandonTrueBtn;
    public Button AbandonFalseBtn;
    public Button MoveDivideBtn;
    public Button MoveMoveBtn;
    public Button MoveByGroupBtn;
    public Button MoveAllBtn;

    public GameObject ActionsObj;
    public GameObject MoveObj;
    public GameObject MoveAllOrByGroupObj;
    public GameObject AbandonApplyObj;
    public GameObject TroopsOptObj;
    public GameObject TroopsCreateNewOrNotObj;
    public GameObject TroopsCreateNewObj;
    public GameObject BuyTroopsObj;
    public GameObject CoinsInfoObj;
    public GameObject TroopsInfoObj;
    public GameObject AreaObj;

    public Text SoldiersToBuyTxt;
    public Text PriceOfSoldierTxt;
    public Text TotalPriceOfSoldiersTxt;
    public Text TroopsTxt;
    public Text CoinsTxt;
    public Text ActionTxt;
    public Text NumOfTotalTroopsInAreaTxt;
    #endregion

    public bool IsCapital { get; }
    public int ID { get; }

    public int OwnerID;
    public int Taxes;
    public int TroopsUngrouped;

    /// <summary><c>AreaStep</c> values: 
    /// 0 - No action;
    /// 1 - Action selection: troops, move, abandon, upgrade;
    /// 11 - "Move" action selected. "Move" or "devide" selection;
    /// 111 - "Move" selected. "Move all" or "separate" selection;
    /// 1111 - Move "separate" selected;
    /// 112 - "Divide" selected;
    /// 12 - "Troops" selected. "buy" or "regroup" selection;
    /// 121 - "Buy" selected;
    /// 112 - "Regroup" selected. "Connect" or "divide" selection;
    /// 1121 - "Connect" selected;
    /// 2 - Where to move selection;
    /// </summary>
    public int AreaStep;
    public int MoneySpent;
    public int TroopsToBuy;
    public int TroopPrice;
    public int MoneyCurrentBudget;
    public int TroopsCntToAddToTotal;

    public bool ThisAreaIsBeingAttacked;

    public List<int> NeighbourAreasID;
    public List<int> SelectedTroopIndex;

    public List<Troop> TroopsGroup;
    public List<Troop> TroopsSelected;
    public List<Troop> TroopsSelectedFromOtherSlot;

    public Area(
        bool IsCapital,
        int Taxes,
        Button AreaBtn,
        Button MoveBtn,
        Button TroopBtn,
        Button UpgradeBtn,
        Button AbandonBtn,
        Button TroopsBuyBtn,
        Button TroopsDivideBtn,
        Button TroopsUpgradeBtn,
        Button NewGroupTrueBtn,
        Button NewGroupFalseBtn,
        Button SoldiersToBuyUpBtn,
        Button SoldiersToBuyDownBtn,
        Button BuySoldiersBtn,
        Button AbandonTrueBtn,
        Button AbandonFalseBtn,
        Button MoveDivideBtn,
        Button MoveMoveBtn,
        Button MoveByGroupBtn,
        Button MoveAllBtn,
        Text SoldiersToBuyTxt,
        GameObject ActionsObj,
        GameObject MoveObj,
        GameObject MoveAllOrByGroupObj,
        GameObject AbandonApplyObj,
        GameObject TroopsOptObj,
        GameObject TroopsCreateNewOrNotObj,
        GameObject TroopsCreateNewObj,
        GameObject BuyTroopsObj,
        GameObject CoinsInfoObj,
        GameObject TroopsInfoObj,
        GameObject AreaObj,
        Text ActionTxt,
        Text NumOfTotalTroopsInAreaTxt,
        Text PriceOfSoldierTxt,
        Text TotalPriceOfSoldiersTxt,
        Text TroopsTxt,
        Text CoinsTxt,
        int ID,
        int OwnerID,
        List<int> NeighbourArID,
        List<Troop> TroopsGroupsInit
        )
    {
        //init lists
        TroopsGroup = new List<Troop>();
        TroopsGroup.AddRange(TroopsGroupsInit);
        TroopsSelected = new List<Troop>();
        TroopsSelectedFromOtherSlot = new List<Troop>();
        SelectedTroopIndex = new List<int>();
        NeighbourAreasID = new List<int>();
        NeighbourAreasID.AddRange(NeighbourArID);

        this.IsCapital = IsCapital;
        this.Taxes = Taxes;

        //init Unity objects
        this.AreaBtn = AreaBtn;
        this.MoveBtn = MoveBtn;
        this.MoveBtn.onClick.AddListener(MoveOptSelected);
        this.TroopBtn = TroopBtn;
        this.TroopBtn.onClick.AddListener(TroopsActionSelected);
        this.UpgradeBtn = UpgradeBtn;
        this.AbandonBtn = AbandonBtn;
        this.TroopsBuyBtn = TroopsBuyBtn;
        this.TroopsBuyBtn.onClick.AddListener(BuyTroopsAction);
        this.TroopsDivideBtn = TroopsDivideBtn;
        this.TroopsUpgradeBtn = TroopsUpgradeBtn;
        this.NewGroupTrueBtn = NewGroupTrueBtn;
        this.NewGroupTrueBtn.onClick.AddListener(CreateNewGroupFromUngroupedSoldierTrue);
        this.NewGroupFalseBtn = NewGroupFalseBtn;
        this.NewGroupFalseBtn.onClick.AddListener(CreateNewGroupFromUngroupedSoldiersFalse);
        this.SoldiersToBuyUpBtn = SoldiersToBuyUpBtn;
        this.SoldiersToBuyUpBtn.onClick.AddListener(SoldiersToBuyUpPressed);
        this.SoldiersToBuyDownBtn = SoldiersToBuyDownBtn;
        this.SoldiersToBuyDownBtn.onClick.AddListener(SoldiersToBuyDownPressed);
        this.BuySoldiersBtn = BuySoldiersBtn;
        this.BuySoldiersBtn.onClick.AddListener(BuySoldiers);
        this.AbandonTrueBtn = AbandonTrueBtn;
        this.AbandonFalseBtn = AbandonFalseBtn;
        this.MoveDivideBtn = MoveDivideBtn;
        this.MoveMoveBtn = MoveMoveBtn;
        this.MoveByGroupBtn = MoveByGroupBtn;
        this.MoveByGroupBtn.onClick.AddListener(TroopSelectionSeparate);
        this.MoveAllBtn = MoveAllBtn;
        this.MoveAllBtn.onClick.AddListener(TroopSelectionAll);
        this.SoldiersToBuyTxt = SoldiersToBuyTxt;
        this.ID = ID;
        this.OwnerID = OwnerID;
        this.ActionsObj = ActionsObj;
        this.MoveObj = MoveObj;
        this.MoveAllOrByGroupObj = MoveAllOrByGroupObj;
        this.AbandonApplyObj = AbandonApplyObj;
        this.TroopsOptObj = TroopsOptObj;
        this.TroopsCreateNewOrNotObj = TroopsCreateNewOrNotObj;
        this.TroopsCreateNewObj = TroopsCreateNewObj;
        this.BuyTroopsObj = BuyTroopsObj;
        this.CoinsInfoObj = CoinsInfoObj;
        this.TroopsInfoObj = TroopsInfoObj;
        this.AreaObj = AreaObj;
        this.ActionTxt = ActionTxt;
        this.NumOfTotalTroopsInAreaTxt = NumOfTotalTroopsInAreaTxt;
        this.PriceOfSoldierTxt = PriceOfSoldierTxt;
        this.TotalPriceOfSoldiersTxt = TotalPriceOfSoldiersTxt;
        this.TroopsTxt = TroopsTxt;
        this.CoinsTxt = CoinsTxt;
    }

    private void TroopSelectionAll()
    {
        //select all troops who did not move
        if (TroopsGroup.Count > 0)
        {
            TroopsSelected.Clear();
            foreach (var troop in TroopsGroup)
            {
                if (!troop.MovedThisTurn)
                {
                    TroopsSelected.Add(Clone(troop));
                    SelectedTroopIndex.Add(TroopsGroup.IndexOf(troop));
                }
            }
            AreaStep = 2;
        } else
        {
            AreaStep = 0;
        }
    }
    private void TroopSelectionSeparate()
    {
        if (TroopsGroup.Count > 0)
        {
            AreaStep = 1111;
        }
    }
    private void MoveOptSelected()
    {
        AreaStep = 11;
    }
    private void TroopsActionSelected()
    {
        AreaStep = 12;
    }
    private void BuyTroopsAction()
    {
        AreaStep = 121;
        SoldiersToBuyTxt.text = TroopsToBuy.ToString();
        PriceOfSoldierTxt.text = TroopPrice.ToString();
        TotalPriceOfSoldiersTxt.text = (TroopsToBuy * TroopPrice).ToString();
    }
    private void SoldiersToBuyUpPressed()
    {
        if (((TroopsToBuy*TroopPrice) + TroopPrice) <= MoneyCurrentBudget)
        {
            TroopsToBuy += 1;
        }
        SoldiersToBuyTxt.text = TroopsToBuy.ToString();
        PriceOfSoldierTxt.text = TroopPrice.ToString();
        TotalPriceOfSoldiersTxt.text = (TroopsToBuy * TroopPrice).ToString();
    }
    private void SoldiersToBuyDownPressed()
    {
        if (TroopsToBuy > 0)
        {
            TroopsToBuy -= 1;
        }
        SoldiersToBuyTxt.text = TroopsToBuy.ToString();
        PriceOfSoldierTxt.text = TroopPrice.ToString();
        TotalPriceOfSoldiersTxt.text = (TroopsToBuy * TroopPrice).ToString();
    }
    private void BuySoldiers()
    {
        MoneySpent = TroopsToBuy * TroopPrice;
    }
    private void CreateNewGroupFromUngroupedSoldierTrue()
    {
        if (TroopsToBuy > 0 && TroopsGroup.Count < 6)
        {
            TroopsGroup.Add(new Troop(TroopsToBuy, false ));
            TroopsCntToAddToTotal = TroopsToBuy;
            TroopsToBuy = 0;
        }
        if (TroopsToBuy > 0 && TroopsGroup.Count >= 6)
        {
            TroopsUngrouped += TroopsToBuy;
            TroopsCntToAddToTotal = TroopsToBuy;
            TroopsToBuy = 0;
        }
        
    }
    private void CreateNewGroupFromUngroupedSoldiersFalse()
    {
        TroopsUngrouped += TroopsToBuy;
        TroopsCntToAddToTotal = TroopsToBuy;
        TroopsToBuy = 0;
    }
    private T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        //if reference null - return null
        if (System.Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
public class Player
{
    
    public PlayerIdentificationInfo IdentificationInfo;
    public int Money;
    public ColorBlock OwedAreasColor;
    public int TroopPrice;
    public List<Area> Areas;
    public List<Troop> TroopsSelected;
    public int TroopSelecedAreaID;
    public List<int> TroopSelectedAreaNeighbourID;
    public bool isPlaying;
    
    //statistikai
    public string State;
    public int TotalMoney;
    public int MoneyAtTheEnd;
    public int TotalTroops;
    public int TroopsAtTheEnd;
    public int TroopsLost;


    public Player(PlayerIdentificationInfo IdentificationInfo, int Money, int TroopPrice, ColorBlock OwedAreasColor)
    {
        this.IdentificationInfo = IdentificationInfo;
        this.Money = Money;
        this.TroopPrice = TroopPrice;
        Areas = new List<Area>();
        TroopsSelected = new List<Troop>();
        TroopSelectedAreaNeighbourID = new List<int>();
        isPlaying = true;
        TotalMoney = Money;
        TotalTroops = 0;
        this.OwedAreasColor = OwedAreasColor;
    }
    
    public void GetTaxes()
    {
        foreach (var ar in Areas)
        {
            Money += ar.Taxes;
            TotalMoney += ar.Taxes;
        }
    }
    public void TotalTroopsNumAtTheEnd()
    {
        foreach (var area in Areas)
        {
            int sum = 0;
            foreach (var group in area.TroopsGroup)
            {
                sum += group.NumberOfTroops;
                
            }
            sum += area.TroopsUngrouped;
            TroopsAtTheEnd += sum;
        }
    }

}
public class Game : TroopsRegroup
{
    private Button _outsideBtn;
    private Button _endTurnBtn;
    private Button _exitBtn;
    private Button _exitGameBtn;

    private Text _currentPlayerIndexTxt;
    private Text _pl1StateTxt;
    private Text _pl2StateTxt;
    private Text _pl1TotalMoneyTxt;
    private Text _pl2TotalMoneyTxt;
    private Text _pl1MoneyAtTheEndTxt;
    private Text _pl2MoneyAtTheEndTxt;
    private Text _pl1TotalTroopsTxt;
    private Text _pl2TotalTroopsTxt;
    private Text _pl1TroopsAtTheEndTxt;
    private Text _pl2TroopsAtTheEndTxt;
    private Text _pl1TroopsLostTxt;
    private Text _pl2TroopsLostTxt;
    private Text _namePl1Txt;
    private Text _namePl2Txt;
    private Text _txtPlayer1Name;
    private Text _txtPlayer2Name;

    private GameObject _gameOverObj;

    private int _currentPlayerIndex;
    private int _currentAreaIndex;

    private bool _end;

    private List<Player> _players;
    public List<Area> Areas;

    private ColorBlock[] _playersColorBlocks;
    private ColorBlock _neutralColorBlock;

    private bool _troopMove;
    private List<Troop> _troopsToMoveTemp;

    public Game()
    {
        #region SetColors
        _playersColorBlocks = new ColorBlock[2];

        //player 1
        _playersColorBlocks[0].normalColor = new Color32(255, 181, 0, 255);
        _playersColorBlocks[0].highlightedColor = new Color32(255, 218, 163, 255);
        _playersColorBlocks[0].pressedColor = new Color32(255, 154, 0, 255);

        //player2
        _playersColorBlocks[1].normalColor = new Color32(39, 181, 255, 255);
        _playersColorBlocks[1].highlightedColor = new Color32(137, 200, 255, 255);
        _playersColorBlocks[1].pressedColor = new Color32(39, 154, 255, 255);

        //neutral
        _neutralColorBlock.normalColor = new Color32(0, 220, 0, 255);
        _neutralColorBlock.highlightedColor = new Color32(0, 187, 0, 255);
        _neutralColorBlock.pressedColor = new Color32(0, 187, 0, 255);
        #endregion

        //players init
        _players = new List<Player>();
        _players.Add(new Player(new PlayerIdentificationInfo(PlayMenu.PlayersSelected[0].Name, PlayMenu.PlayersSelected[0].ID), PlayMenu.Money, PlayMenu.TroopPrice, _playersColorBlocks[0]));
        _players.Add(new Player(new PlayerIdentificationInfo(PlayMenu.PlayersSelected[1].Name, PlayMenu.PlayersSelected[1].ID), PlayMenu.Money, PlayMenu.TroopPrice, _playersColorBlocks[1]));

        //areas init
        Areas = new List<Area>();
        AreasInit(ref Areas);

        //add capitals to players
        _players[0].Areas.Add(Areas[0]);
        _players[1].Areas.Add(Areas[1]);

        #region OtherInit
        _end = false;
        _currentPlayerIndex = 0;
        _troopMove = false;
        _troopsToMoveTemp = new List<Troop>();

        //init gameplay other objects
        _outsideBtn = GameObject.Find("Canvas/Play/Field/btnOutsideActions").GetComponent<Button>();
        _outsideBtn.onClick.AddListener(ResetAllAreas);
        _endTurnBtn = GameObject.Find("Canvas/Play/Field/BtnEndTurn").GetComponent<Button>();
        _endTurnBtn.onClick.AddListener(EndTurn);
        _currentPlayerIndexTxt = GameObject.Find("Canvas/Play/Field/txtCurrentPlayerIndex").GetComponent<Text>();
        _gameOverObj = GameObject.Find("Canvas/Play/Field/GameOver");
        _exitBtn = GameObject.Find("Canvas/Play/Field/GameOver/ExitButton").GetComponent<Button>();
        _exitGameBtn = GameObject.Find("Canvas/Play/ExitGameButton").GetComponent<Button>();
        _exitGameBtn.onClick.AddListener(ExitGame);
        _pl1StateTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/StateTxt").GetComponent<Text>();
        _pl2StateTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/StateTxt").GetComponent<Text>();
        _pl1TotalMoneyTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/TotalMoneyTxt").GetComponent<Text>();
        _pl2TotalMoneyTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/TotalMoneyTxt").GetComponent<Text>();
        _pl1MoneyAtTheEndTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/MoneyAtTheEndTxt").GetComponent<Text>();
        _pl2MoneyAtTheEndTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/MoneyAtTheEndTxt").GetComponent<Text>();
        _pl1TotalTroopsTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/TotalTroopsTxt").GetComponent<Text>();
        _pl2TotalTroopsTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/TotalTroopsTxt").GetComponent<Text>();
        _pl1TroopsAtTheEndTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/TroopsAtTheEndTxt").GetComponent<Text>();
        _pl2TroopsAtTheEndTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/TroopsAtTheEndTxt").GetComponent<Text>();
        _pl1TroopsLostTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/PlayerStats/TroopsLostTxt").GetComponent<Text>();
        _pl2TroopsLostTxt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/PlayerStats/TroopsLostTxt").GetComponent<Text>();
        _namePl1Txt = GameObject.Find("Canvas/Play/Field/GameOver/Player1Stats/NamePl1Txt").GetComponent<Text>();
        _namePl2Txt = GameObject.Find("Canvas/Play/Field/GameOver/Player2Stats/NamePl2Txt").GetComponent<Text>();
        _txtPlayer1Name = GameObject.Find("Canvas/Play/Field/txtPlayer1Name").GetComponent<Text>();
        _txtPlayer2Name = GameObject.Find("Canvas/Play/Field/txtPlayer2Name").GetComponent<Text>();

        //after exit pressed, update players stats table
        foreach (var player in _players)
        {
            _exitBtn.onClick.AddListener(() => UpdatePlayerStatsTable(
                CreateConnection(), 
                player.IdentificationInfo.Name + "_" + player.IdentificationInfo.ID, 
                player.State, 
                player.TotalMoney, 
                player.Money,
                player.TotalTroops,
                player.TroopsAtTheEnd,
                player.TroopsLost
                ));
        }

        //after exit pressed, update players scoreboard table
        _exitBtn.onClick.AddListener(() => UpdateScoreboardTable(CreateConnection()));
        _exitBtn.onClick.AddListener(ExitGame);

        _currentPlayerIndexTxt.text = (_currentPlayerIndex + 1).ToString();
        #endregion
        #region AddListenersAndAlphaThToAreasBtns
        foreach (var area in Areas)
        {
            //add listeners to all areas buttons
            area.AreaBtn.onClick.AddListener(() => SelectedAreaNav(area));
            area.AreaBtn.onClick.AddListener(() => SelectedArea(area, _currentPlayerIndex));
            area.TroopsDivideBtn.onClick.AddListener(() => DivideNav(area));
            area.TroopsDivideBtn.onClick.AddListener(TroopDivideBtnClicked);
            area.MoveDivideBtn.onClick.AddListener(() => DivideNav(area));
            area.MoveDivideBtn.onClick.AddListener(TroopDivideBtnClicked);
            area.MoveByGroupBtn.onClick.AddListener(UpdateTroopList);
            area.AbandonTrueBtn.onClick.AddListener(() => Abandon(area));
            area.MoveAllBtn.onClick.AddListener(() => MoveAllNav(area));
            area.MoveMoveBtn.onClick.AddListener(() => MoveMoveNav(area));
            area.AbandonBtn.onClick.AddListener(() => AbandonNav(area));
            area.MoveBtn.onClick.AddListener(() => MoveNav(area));
            area.AbandonTrueBtn.onClick.AddListener(() => AbandonTrueNav(area));
            area.AbandonFalseBtn.onClick.AddListener(() => AbandonFalseNav(area));
            area.TroopBtn.onClick.AddListener(() => TroopsOptNav(area));
            area.TroopsBuyBtn.onClick.AddListener(() => OpenBuySoldiersWindowNav(area));
            area.NewGroupTrueBtn.onClick.AddListener(() => CreateNewGroupTrueNav(area));
            area.NewGroupFalseBtn.onClick.AddListener(() => CreateNewGroupFalseNav(area));
            area.BuySoldiersBtn.onClick.AddListener(() => BuyTroopsNav(area));

            //add alpha alphaHitTestMinimumThreshold to all buttons images and individual images
            Button[] buttons = area.AreaObj.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                btn.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            }
            Image[] imgs = area.AreaObj.GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                img.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            }
        }
        #endregion
        #region AddAlphaThToTroopsObjObjects
        Button[] buttonsTroopObj = TroopsObj.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttonsTroopObj)
        {
            btn.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
        Image[] imgsTroopObj = TroopsObj.GetComponentsInChildren<Image>(true);
        foreach (var img in imgsTroopObj)
        {
            img.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
        #endregion
        #region AddListenersToTroopsBtns
        Divide1Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());
        Divide2Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());
        Divide3Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());
        Divide4Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());
        Divide5Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());
        Divide6Btn.onClick.AddListener(() => UpdateTroopsInfoAndRegroupVisibility());

        CreateGroupsBtn.onClick.AddListener(NewGroupCreate);

        Connect1Btn.onClick.AddListener(() => ConnectThisTroop(0));
        Connect2Btn.onClick.AddListener(() => ConnectThisTroop(1));
        Connect3Btn.onClick.AddListener(() => ConnectThisTroop(2));
        Connect4Btn.onClick.AddListener(() => ConnectThisTroop(3));
        Connect5Btn.onClick.AddListener(() => ConnectThisTroop(4));
        Connect6Btn.onClick.AddListener(() => ConnectThisTroop(5));

        TroopGroup1Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup1Obj));
        TroopGroup2Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup2Obj));
        TroopGroup3Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup3Obj));
        TroopGroup4Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup4Obj));
        TroopGroup5Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup5Obj));
        TroopGroup6Btn.onClick.AddListener(() => TroopSelectionNav(DivideOrRegroup6Obj));

        TroopGroup1Btn.onClick.AddListener(() => TroopGroupSelected(0));
        TroopGroup2Btn.onClick.AddListener(() => TroopGroupSelected(1));
        TroopGroup3Btn.onClick.AddListener(() => TroopGroupSelected(2));
        TroopGroup4Btn.onClick.AddListener(() => TroopGroupSelected(3));
        TroopGroup5Btn.onClick.AddListener(() => TroopGroupSelected(4));
        TroopGroup6Btn.onClick.AddListener(() => TroopGroupSelected(5));

        Connect1Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup1Obj, Areas[_currentAreaIndex]));
        Divide1Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup1Obj, Areas[_currentAreaIndex]));
        Connect2Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup2Obj, Areas[_currentAreaIndex]));
        Divide2Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup2Obj, Areas[_currentAreaIndex]));
        Connect3Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup3Obj, Areas[_currentAreaIndex]));
        Divide3Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup3Obj, Areas[_currentAreaIndex]));
        Connect4Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup4Obj, Areas[_currentAreaIndex]));
        Divide4Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup4Obj, Areas[_currentAreaIndex]));
        Connect5Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup5Obj, Areas[_currentAreaIndex]));
        Divide5Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup5Obj, Areas[_currentAreaIndex]));
        Connect6Btn.onClick.AddListener(() => TroopConnectSelectionNav(DivideOrRegroup6Obj, Areas[_currentAreaIndex]));
        Divide6Btn.onClick.AddListener(() => DivideTroopsNav(DivideOrRegroup6Obj, Areas[_currentAreaIndex]));

        CreateGroupsBtn.onClick.AddListener(() => CreateGroupsNav(Areas[_currentAreaIndex]));
        #endregion
        #region VisibilityInit
        foreach (var area in Areas)
        {
            area.ActionsObj.SetActive(false);
            area.MoveAllOrByGroupObj.SetActive(false);
            area.AbandonApplyObj.SetActive(false);
            area.TroopsOptObj.SetActive(false);
            area.TroopsCreateNewOrNotObj.SetActive(false);
            area.BuyTroopsObj.SetActive(false);
            area.TroopsCreateNewObj.SetActive(false);
            area.NumOfTotalTroopsInAreaTxt.gameObject.SetActive(false);
        }
        TroopsObj.SetActive(false);
        CreateGroupsObj.SetActive(false);
        _gameOverObj.SetActive(false);
        _exitGameBtn.gameObject.SetActive(true);
        #endregion
    }

    private void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Moving()
    {
       //check if there is selected troops to move
        foreach (var area in _players[_currentPlayerIndex].Areas)
        {
            if (area.TroopsSelected.Count > 0 && !_troopMove)
            {
                _troopMove = true;
                _troopsToMoveTemp.Clear();
                _troopsToMoveTemp.AddRange(Clone(area.TroopsSelected));
                area.TroopsSelected.Clear();
                _players[_currentPlayerIndex].TroopSelectedAreaNeighbourID.Clear();
                _players[_currentPlayerIndex].TroopSelectedAreaNeighbourID.AddRange(area.NeighbourAreasID);
                _players[_currentPlayerIndex].TroopSelecedAreaID = area.ID;
            }
        }
        //if yes, add to all areas selected troops as temp list
        if (_troopMove == true)
        {
            foreach (var area in Areas)
            {
                if (_players[_currentPlayerIndex].TroopSelectedAreaNeighbourID.Contains(area.ID) || area.ID == _players[_currentPlayerIndex].TroopSelecedAreaID)
                {
                    area.TroopsSelectedFromOtherSlot.AddRange(Clone(_troopsToMoveTemp));
                    area.AreaStep = 2;
                }
            }

            //clear temp variables
            _troopsToMoveTemp.Clear();
            _troopMove = false;
            _players[_currentPlayerIndex].TroopSelectedAreaNeighbourID.Clear();
        }

        foreach (var area in Areas)
        {
            // if troops moved somethere, clear all areas temp variables and delete moved troops from original area
            if (area.AreaStep == 0 && area.TroopsSelectedFromOtherSlot.Count > 0)
            {
                foreach (var ar in Areas)
                {
                    ar.TroopsSelectedFromOtherSlot.Clear();
                    ar.AreaStep = 0;
                    ar.ThisAreaIsBeingAttacked = false;

                    if (ar.SelectedTroopIndex.Count > 1)
                    {
                        if (_players[_currentPlayerIndex].TroopSelecedAreaID != area.ID)
                        {
                            //add those troops who could not move to temp list, delete all, and add troops from temp list back to area
                            List<Troop> troopsTemp = new List<Troop>();
                            foreach (var troop in ar.TroopsGroup)
                            {
                                if (!ar.SelectedTroopIndex.Contains(ar.TroopsGroup.IndexOf(troop)))
                                {
                                    troopsTemp.Add(Clone(troop));
                                }
                            }
                            ar.TroopsGroup.Clear();
                            ar.TroopsGroup.AddRange(Clone(troopsTemp));
                            troopsTemp.Clear();
                        }
                        ar.SelectedTroopIndex.Clear();
                    }
                    else if(ar.SelectedTroopIndex.Count == 1)
                    {
                        if (_players[_currentPlayerIndex].TroopSelecedAreaID != area.ID)
                        {
                            ar.TroopsGroup.RemoveAt(ar.SelectedTroopIndex.First());
                        }
                        ar.SelectedTroopIndex.Clear();
                    }
                }
            }
        }
    }
    public void Buying()
    {
        foreach (var area in _players[_currentPlayerIndex].Areas)
        {
            area.MoneyCurrentBudget = _players[_currentPlayerIndex].Money;
            area.TroopPrice = _players[_currentPlayerIndex].TroopPrice;
            if (area.MoneySpent > 0)
            {
                _players[_currentPlayerIndex].Money -= area.MoneySpent;
                area.MoneySpent = 0;
            }
        }
    }
    private void EndTurn()
    {
        if (_players.Count > (_currentPlayerIndex + 1))
        {
            _currentPlayerIndex += 1;
        } else
        {
            _currentPlayerIndex = 0;
        }
        _currentPlayerIndexTxt.text = (_currentPlayerIndex + 1).ToString();

        _players[_currentPlayerIndex].GetTaxes();

        foreach (var area in _players[_currentPlayerIndex].Areas)
        {
            foreach (var troop in area.TroopsGroup)
            {
                troop.MovedThisTurn = false;
            }
        }
    }
    public void GameStateCheck()
    {
        foreach (var player in _players)
        {
            if (!player.Areas.Where(x => x.IsCapital == true).Any())
            {
                player.isPlaying = false;

                if (_currentPlayerIndex == _players.IndexOf(player))
                {
                    EndTurn();
                }
            } 
        }
        
        if (!_end && _players.Where(x => x.isPlaying == true).Count() == 1)
        {
            _end = true;

            _players[_currentPlayerIndex].State = "Winner";

            foreach (var player in _players.Where(x => x.State != "Winner"))
            {
                player.State = "Looser";
            }
            foreach (var player in _players)
            {
                player.TotalTroopsNumAtTheEnd();
                player.TroopsLost = player.TotalTroops - player.TroopsAtTheEnd;
            }
        }
    }
    private void ResetAllAreas()
    {
        TroopsObj.SetActive(false);
        NewGroupsNum = 0;

        foreach (var area in Areas)
        {
            area.ActionsObj.SetActive(false);
            area.MoveAllOrByGroupObj.SetActive(false);
            area.AbandonApplyObj.SetActive(false);
            area.TroopsOptObj.SetActive(false);
            area.TroopsCreateNewOrNotObj.SetActive(false);
            area.BuyTroopsObj.SetActive(false);
            area.TroopsCreateNewObj.SetActive(false);
            area.SelectedTroopIndex.Clear();
            area.TroopsSelected.Clear();
            area.TroopsSelectedFromOtherSlot.Clear();
            area.TroopsToBuy = 0;
            area.AreaStep = 0;
        }
    }
    private void SelectedArea(Area area, int _currentPlayerIndex)
    {
        if (!_players[_currentPlayerIndex].Areas.Contains(area))
        {
            area.ThisAreaIsBeingAttacked = true;
        }
        if (area.OwnerID == _currentPlayerIndex && area.AreaStep == 0)
        {
            area.AreaStep = 1;
        }
        if (area.AreaStep == 2)
        {
            if (!area.ThisAreaIsBeingAttacked)
            {
                if (_players[_currentPlayerIndex].TroopSelecedAreaID != area.ID)
                {
                    foreach (var troop in area.TroopsSelectedFromOtherSlot)
                    {
                        troop.MovedThisTurn = true;
                    }
                    int i = 0;
                    if (area.TroopsGroup.Count + area.TroopsSelectedFromOtherSlot.Count > 6)
                    {
                        i = area.TroopsGroup.Count + area.TroopsSelectedFromOtherSlot.Count - 6;
                    }
                    area.TroopsGroup.AddRange(Clone(area.TroopsSelectedFromOtherSlot));
                    while (i != 0)
                    {
                        area.TroopsUngrouped += area.TroopsGroup.Last().NumberOfTroops;
                        area.TroopsGroup.RemoveAt(area.TroopsGroup.IndexOf(area.TroopsGroup.Last()));
                        i--;
                    }
                    area.AreaStep = 0;
                } else
                {
                    area.AreaStep = 0;
                }
                
            }
            else
            {
                int defendingTroopsLeft = 0;
                int atackingTroopsLef = 0;
                Battle(ref defendingTroopsLeft, ref atackingTroopsLef, area);
                if (defendingTroopsLeft < 0)
                {
                    area.TroopsGroup.Clear();
                    area.TroopsUngrouped = 0;
                    area.TroopsGroup.Add(new Troop(atackingTroopsLef, true));
                    if (area.OwnerID != 11)
                    {
                        _players[area.OwnerID].Areas.Remove(area);
                    }
                    area.OwnerID = _currentPlayerIndex;
                    area.MoneyCurrentBudget = _players[_currentPlayerIndex].Money;
                    area.TroopPrice = _players[_currentPlayerIndex].TroopPrice;
                    area.ThisAreaIsBeingAttacked = false;
                    _players[_currentPlayerIndex].Areas.Add(area);
                }
                else
                {
                    area.TroopsGroup.Clear();
                    area.TroopsUngrouped = 0;
                    if (defendingTroopsLeft != 0)
                    {
                        area.TroopsGroup.Add(new Troop(defendingTroopsLeft, false ));
                    }
                }
                area.AreaStep = 0;
            }
        }
    }
    private void Battle(ref int myTroopsLeft, ref int enemyTroopsLeft, Area area)
    {
        int myAllTroops = 0;
        foreach (var troop in area.TroopsGroup)
        {
            myAllTroops += troop.NumberOfTroops;
        }
        myAllTroops += area.TroopsUngrouped;
        int enemyAllTroops = 0;
        foreach (var troop in area.TroopsSelectedFromOtherSlot)
        {
            enemyAllTroops += troop.NumberOfTroops;
        }
        myTroopsLeft = myAllTroops - enemyAllTroops;
        enemyTroopsLeft = enemyAllTroops - myAllTroops;
        
    }
    private void AreasInit(ref List<Area> Areas)
    {
        //neighbour areas ID 
        List<int> neihboursOfArea1 = new List<int>() { 12, 15 };
        List<int> neihboursOfArea2 = new List<int>() { 18, 20 };
        List<int> neihboursOfArea11 = new List<int>() { 12, 13, 14, 15, 17 };
        List<int> neihboursOfArea12 = new List<int>() { 1, 11, 13, 15 };
        List<int> neihboursOfArea13 = new List<int>() { 11, 12 };
        List<int> neihboursOfArea14 = new List<int>() { 11, 15, 16, 17, 18, 19 };
        List<int> neihboursOfArea15 = new List<int>() { 1, 11, 12, 14, 16 };
        List<int> neihboursOfArea16 = new List<int>() { 14, 15, 19 };
        List<int> neihboursOfArea17 = new List<int>() { 11, 14, 18 };
        List<int> neihboursOfArea18 = new List<int>() { 2, 14, 17, 19, 20 };
        List<int> neihboursOfArea19 = new List<int>() { 14, 16, 18, 20, 21 };
        List<int> neihboursOfArea20 = new List<int>() { 2, 18, 19, 21 };
        List<int> neihboursOfArea21 = new List<int>() { 19, 20 };

        //initial area troops
        List<Troop> troopsGroupsInit1 = new List<Troop>();
        List<Troop> troopsGroupsInit2 = new List<Troop>();
        List<Troop> troopsGroupsInit11 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit12 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit13 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit14 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit15 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit16 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit17 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit18 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit19 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit20 = new List<Troop>() { new Troop(5, false) };
        List<Troop> troopsGroupsInit21 = new List<Troop>() { new Troop(5, false) };

        //create new areas
        Areas.Add(CreateArea(true, 50, 1, 0, neihboursOfArea1, troopsGroupsInit1, "C1_area"));
        Areas.Add(CreateArea(true, 50, 2, 1, neihboursOfArea2, troopsGroupsInit2, "C2_area"));
        Areas.Add(CreateArea(false, 30, 11, 11, neihboursOfArea11, troopsGroupsInit11, "F11_area"));
        Areas.Add(CreateArea(false, 30, 12, 11, neihboursOfArea12, troopsGroupsInit12, "F12_area"));
        Areas.Add(CreateArea(false, 30, 13, 11, neihboursOfArea13, troopsGroupsInit13, "F13_area"));
        Areas.Add(CreateArea(false, 30, 14, 11, neihboursOfArea14, troopsGroupsInit14, "F14_area"));
        Areas.Add(CreateArea(false, 30, 15, 11, neihboursOfArea15, troopsGroupsInit15, "F15_area"));
        Areas.Add(CreateArea(false, 30, 16, 11, neihboursOfArea16, troopsGroupsInit16, "F16_area"));
        Areas.Add(CreateArea(false, 30, 17, 11, neihboursOfArea17, troopsGroupsInit17, "F17_area"));
        Areas.Add(CreateArea(false, 30, 18, 11, neihboursOfArea18, troopsGroupsInit18, "F18_area"));
        Areas.Add(CreateArea(false, 30, 19, 11, neihboursOfArea19, troopsGroupsInit19, "F19_area"));
        Areas.Add(CreateArea(false, 30, 20, 11, neihboursOfArea20, troopsGroupsInit20, "F20_area"));
        Areas.Add(CreateArea(false, 30, 21, 11, neihboursOfArea21, troopsGroupsInit21, "F21_area"));
    }
    private Area CreateArea(bool isCapital, int taxes, int areaID, int ownerID, List<int> neihboursOfArea, List<Troop> troopsGroupsInit, string areaName)
    {
        Area area = new Area
            (
            isCapital,
            taxes,
            GameObject.Find("Canvas/Play/Field/" + areaName + "/btnArea").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/btnMove").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/btnTroop").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/btnUpgrade").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/btnAbandon").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsOpt/btnTroopsBuy").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsOpt/btnTroopsDivide").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsOpt/btnTroopsUpgrade").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNewOrNot/btnNewGroupTrue").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNewOrNot/btnNewGroupFalse").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/btnGroupsUp").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/btnGroupsDown").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/btnBuySoldiers").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/AbandonApply/btnAbandonTrue").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/AbandonApply/btnAbandonFalse").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovOpt/btnMoveDivide").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovOpt/btnMoveMove").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovAllOrGroup/btnMoveByGroup").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovAllOrGroup/btnMoveAll").GetComponent<Button>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/txtNumOfSoldiersToBuy").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovOpt"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MovAllOrGroup"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/AbandonApply"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsOpt"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNewOrNot"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MoneyInfo"),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsInfo"),
            GameObject.Find("Canvas/Play/Field/" + areaName + ""),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/ActionInfo/txtAction").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/btnArea/txtNumOfTotalTroopsInArea").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/txtPriceOfSoldier").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsCreateNew/BuyTroops/txtTotalPriceOfSoldiers").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/TroopsInfo/txtTroops").GetComponent<Text>(),
            GameObject.Find("Canvas/Play/Field/" + areaName + "/Actions/MoneyInfo/txtMoney").GetComponent<Text>(),
            areaID,
            ownerID,
            neihboursOfArea,
            troopsGroupsInit
            );

        return area;
    }
    private void TroopDivideBtnClicked()
    {
        if (Areas[_currentAreaIndex].AreaStep == 12 || Areas[_currentAreaIndex].AreaStep == 11 || Areas[_currentAreaIndex].AreaStep == 121)
        {
            Areas[_currentAreaIndex].AreaStep = 112;
        }
        if (Areas[_currentAreaIndex].TroopsGroup.Count == 0)
        {
            UpdateTroopsInfoAndRegroupVisibility();
            CreateGroupsObj.SetActive(true);
        }
        UpdateTroopList();
    }
    private void UpdateTroopList()
    {
        TroopsGroupTemp.Clear();
        TroopsGroupTemp.AddRange(Clone(Areas[_currentAreaIndex].TroopsGroup));
        if (Areas[_currentAreaIndex].AreaStep != 1111 && Areas[_currentAreaIndex].TroopsGroup.Count > 0)
        {
            Areas[_currentAreaIndex].AreaStep = 112;
        }
        TroopGroupNav();
    }
    private void NewGroupCreate()
    {
        bool atnaujintaBeNuliniu = false;
        while (!atnaujintaBeNuliniu)
        {
            foreach (var troop in TroopsGroupTemp)
            {
                if (troop.NumberOfTroops == 0)
                {
                    TroopsGroupTemp.Remove(troop);
                    break;
                }
            }
            atnaujintaBeNuliniu = true;
            foreach (var item in TroopsGroupTemp)
            {
                if (item.NumberOfTroops == 0)
                {
                    atnaujintaBeNuliniu = false;
                }
            }
        }

        if (NewGroupsNum > 0)
        {
            for (int i = 0; i < NewGroupsNum; i++)
            {
                if (i != NewGroupsNum - 1)
                {
                    TroopsGroupTemp.Add(new Troop(TroopsUngroupedTemp / NewGroupsNum, false ));
                }
                else
                {
                    TroopsGroupTemp.Add(new Troop(TroopsUngroupedTemp - (i * (TroopsUngroupedTemp / NewGroupsNum)), false ));
                }
            }
            NewGroupsNum = 0;
            TroopsUngroupedTemp = 0;
        }

        Areas[_currentAreaIndex].TroopsUngrouped = TroopsUngroupedTemp;
        Areas[_currentAreaIndex].TroopsGroup.Clear();
        Areas[_currentAreaIndex].TroopsGroup.AddRange(Clone(TroopsGroupTemp));
        Areas[_currentAreaIndex].AreaStep = 0;
        TroopsGroupTemp.Clear();
    }
    private void ConnectThisTroop(int ind)
    {
        if (TroopsGroupTemp.Count > 1 && !TroopsGroupTemp[ind].MovedThisTurn)
        {
            UpdateTroopsInfoAndRegroupVisibility();
            TroopIndSelectedToConnect = ind;
            Areas[_currentAreaIndex].AreaStep = 1121;
        }
    }
    private void UpdateTroopsInfoAndRegroupVisibility()
    {
        TroopsUngroupedTemp = Areas[_currentAreaIndex].TroopsUngrouped;
        TroopsGroupTemp.Clear();
        TroopsGroupTemp.AddRange(Clone(Areas[_currentAreaIndex].TroopsGroup));
        UpdateTroopDivideInfo();
    }
    public void FindCurrentArea()
    {
        for (int i = 0; i < Areas.Count; i++)
        {
            if (Areas[i].AreaStep != 0)
            {
                _currentAreaIndex = i;
            }
        }
    }
    public void HideDifferentAreaTypeElements()
    {
        foreach (var area in Areas)
        {
            if (area.OwnerID == 11)
            {
                area.AbandonBtn.gameObject.SetActive(false);
            }
            if (area.ID != 1 && area.ID != 2)
            {
                area.TroopsBuyBtn.gameObject.SetActive(false);
            }
        }
    }
    private void TroopGroupSelected(int ind)
    {
        //move separate
        if (Areas[_currentAreaIndex].AreaStep == 1111)
        {
            if (!Areas[_currentAreaIndex].TroopsGroup[ind].MovedThisTurn)
            {
                Areas[_currentAreaIndex].SelectedTroopIndex.Add(ind);

                Areas[_currentAreaIndex].TroopsSelected
                    .Add(Clone(Areas[_currentAreaIndex].TroopsGroup[Areas[_currentAreaIndex].SelectedTroopIndex[0]]));
                Areas[_currentAreaIndex].AreaStep = 2;
            }
        }
        //troops connection
        if (TroopIndSelectedToConnect != 11)
        {
            TroopsGroupTemp[ind].NumberOfTroops += TroopsGroupTemp[TroopIndSelectedToConnect].NumberOfTroops;
            TroopsGroupTemp.RemoveAt(TroopIndSelectedToConnect);
            TroopIndSelectedToConnect = 11;
            NewGroupCreate();
            Areas[_currentAreaIndex].AreaStep = 112;
        }
        //update info
        TroopsUngroupedTemp = Areas[_currentAreaIndex].TroopsUngrouped;
        TroopsGroupTemp.Clear();
        TroopsGroupTemp.AddRange(Clone(Areas[_currentAreaIndex].TroopsGroup));
        UpdateTroopListInfo();
        
    }
    private void Abandon(Area area)
    {
        if (area.IsCapital)
        {
            foreach (var ar in _players[_currentPlayerIndex].Areas)
            {
                ar.TroopsGroup.Clear();
                ar.TroopsUngrouped = 0;
                ar.OwnerID = 11;
                ar.MoneyCurrentBudget = 0;
            }
            _players[_currentPlayerIndex].Money = 0;
        }
        _players[_currentPlayerIndex].Areas.Remove(area);
    }
    public void UpdatePlayersTotalTroops()
    {
        foreach (var area in Areas)
        {
            if (area.TroopsCntToAddToTotal > 0)
            {
                _players[area.OwnerID].TotalTroops += area.TroopsCntToAddToTotal;
                area.TroopsCntToAddToTotal = 0;
            }
        }
    }
    private void TroopGroupNav()
    {
        if (Areas[_currentAreaIndex].AreaStep != 112)
        {
            CreateGroupsObj.SetActive(false);
        }
        TroopsObj.SetActive(true);
        TroopsListObj.SetActive(true);

        UpdateTroopListInfo();
    }
    private void TroopConnectSelectionNav(GameObject divideOrRegroup, Area area)
    {
        area.MoveObj.SetActive(false);
        divideOrRegroup.SetActive(false);
        TroopsListObj.SetActive(true);
        TroopsObj.SetActive(true);
    }
    private void DivideTroopsNav(GameObject divideOrRegroup, Area area)
    {
        area.ActionsObj.SetActive(false);
        area.MoveObj.SetActive(false);
        divideOrRegroup.SetActive(false);
        TroopsListObj.SetActive(false);
        CreateGroupsObj.SetActive(true);
    }
    private void DivideNav(Area area)
    {
        area.ActionsObj.SetActive(false);
        area.MoveObj.SetActive(false);
        DivideOrRegroup1Obj.SetActive(false);
        DivideOrRegroup2Obj.SetActive(false);
        DivideOrRegroup3Obj.SetActive(false);
        DivideOrRegroup4Obj.SetActive(false);
        DivideOrRegroup5Obj.SetActive(false);
        DivideOrRegroup6Obj.SetActive(false);
        CreateGroupsObj.SetActive(false);
        TroopsListObj.SetActive(true);
        TroopsObj.SetActive(true);
    }
    private void CreateGroupsNav(Area area)
    {
        area.ActionsObj.SetActive(false);
        area.TroopsOptObj.SetActive(false);
        area.MoveObj.SetActive(false);
        DivideOrRegroup1Obj.SetActive(false);
        DivideOrRegroup2Obj.SetActive(false);
        DivideOrRegroup3Obj.SetActive(false);
        DivideOrRegroup4Obj.SetActive(false);
        DivideOrRegroup5Obj.SetActive(false);
        DivideOrRegroup6Obj.SetActive(false);
        CreateGroupsObj.SetActive(false);
        TroopsObj.SetActive(true);
    }
    private void MoveMoveNav(Area area)
    {
        area.MoveAllOrByGroupObj.SetActive(true);
    }
    private void MoveAllNav(Area area)
    {
        area.ActionsObj.SetActive(false);
        TroopsObj.SetActive(false);
        area.MoveAllOrByGroupObj.SetActive(false);
    }
    private void AbandonNav(Area area)
    {
        area.AbandonApplyObj.SetActive(true);
    }
    private void MoveNav(Area area)
    {
        area.MoveObj.SetActive(true);
    }
    private void SelectedAreaNav(Area area)
    {
        if (area.AreaStep == 0)
        {
            area.NumOfTotalTroopsInAreaTxt.gameObject.SetActive(false);
            bool closeAllOthers = false;
            foreach (var ar in Areas)
            {
                if (ar.AreaStep != 0)
                {
                    closeAllOthers = true;
                }
            }
            if (closeAllOthers == true)
            {
                ResetAllAreas();
            }
            if (area.OwnerID == _currentPlayerIndex)
            {
                area.ActionsObj.SetActive(true);
                area.MoveObj.SetActive(false);
            }
        }
    }
    private void TroopSelectionNav(GameObject divideOrRegroup)
    {
        if (Areas[_currentAreaIndex].AreaStep != 1121 && Areas[_currentAreaIndex].AreaStep != 1111)
        {
            divideOrRegroup.SetActive(true);
        }
        if (Areas[_currentAreaIndex].AreaStep == 1111)
        {
            TroopsListObj.SetActive(false);
            Areas[_currentAreaIndex].MoveAllOrByGroupObj.SetActive(false);
            Areas[_currentAreaIndex].MoveObj.SetActive(false);
            Areas[_currentAreaIndex].ActionsObj.SetActive(false);
        }
    }
    private void AbandonTrueNav(Area area)
    {
        area.ActionsObj.SetActive(false);
        area.AbandonApplyObj.SetActive(false);
    }
    private void TroopsOptNav(Area area)
    {
        area.TroopsOptObj.SetActive(true);
    }
    private void AbandonFalseNav(Area area)
    {
        area.AbandonApplyObj.SetActive(false);
    }
    private void OpenBuySoldiersWindowNav(Area area)
    {
        area.TroopsCreateNewOrNotObj.SetActive(false);
        area.TroopsCreateNewObj.SetActive(true);
        area.BuyTroopsObj.SetActive(true);
    }
    private void CreateNewGroupTrueNav(Area area)
    {

        area.TroopsCreateNewOrNotObj.SetActive(false);
        area.TroopsCreateNewObj.SetActive(false);

    }
    private void CreateNewGroupFalseNav(Area area)
    {
        area.TroopsCreateNewOrNotObj.SetActive(false);
    }
    private void BuyTroopsNav(Area area)
    {
        area.TroopsCreateNewObj.SetActive(false);
        area.BuyTroopsObj.SetActive(false);
        area.TroopsCreateNewOrNotObj.SetActive(true);
    }
    public void AlwaysOnTopObject()
    {
        if (!_end)
        {
            if (_currentAreaIndex != 100)
            {
                Areas[_currentAreaIndex].AreaObj.transform.SetAsLastSibling();
            }
            TroopsObj.transform.SetAsLastSibling();
        }
        else
        {
            _gameOverObj.SetActive(true);
            _exitGameBtn.gameObject.SetActive(false);
            _gameOverObj.transform.SetAsLastSibling();
            GameOverTable();
        }
    }
    private void GameOverTable()
    {
        _pl1StateTxt.text = _players[0].State;
        _pl2StateTxt.text = _players[1].State;
        _pl1TotalMoneyTxt.text = _players[0].TotalMoney.ToString();
        _pl2TotalMoneyTxt.text = _players[1].TotalMoney.ToString();
        _pl1MoneyAtTheEndTxt.text = _players[0].Money.ToString();
        _pl2MoneyAtTheEndTxt.text = _players[1].Money.ToString();
        _pl1TotalTroopsTxt.text = _players[0].TotalTroops.ToString();
        _pl2TotalTroopsTxt.text = _players[1].TotalTroops.ToString();
        _pl1TroopsAtTheEndTxt.text = _players[0].TroopsAtTheEnd.ToString();
        _pl2TroopsAtTheEndTxt.text = _players[1].TroopsAtTheEnd.ToString();
        _pl1TroopsLostTxt.text = _players[0].TroopsLost.ToString();
        _pl2TroopsLostTxt.text = _players[1].TroopsLost.ToString();
        _namePl1Txt.text = _players[0].IdentificationInfo.Name;
        _namePl2Txt.text = _players[1].IdentificationInfo.Name;
    }
    public void UpdateAreasVisibleInfo()
    {
        foreach (var area in Areas)
        {
            int sum = 0;
            foreach (var item in area.TroopsGroup)
            {
                sum += item.NumberOfTroops;
            }
            sum += area.TroopsUngrouped;
            area.NumOfTotalTroopsInAreaTxt.text = sum.ToString();
        }
        _txtPlayer1Name.text = _players[0].IdentificationInfo.Name;
        _txtPlayer2Name.text = _players[1].IdentificationInfo.Name;
    }
    public void AreasColor()
    {
        foreach (var area in Areas)
        {
            Button[] buttons = area.AreaObj.GetComponentsInChildren<Button>(true);

            foreach (var btn in buttons)
            {
                ColorBlock colorBlock = btn.colors;
                if (area.OwnerID == 0)
                {
                    colorBlock.highlightedColor = _players[0].OwedAreasColor.highlightedColor;
                    colorBlock.pressedColor = _players[0].OwedAreasColor.pressedColor;
                } 
                else if(area.OwnerID == 1)
                {
                    colorBlock.highlightedColor = _players[1].OwedAreasColor.highlightedColor;
                    colorBlock.pressedColor = _players[1].OwedAreasColor.pressedColor;
                }
                else if (area.OwnerID == 11)
                {
                    colorBlock.highlightedColor = _neutralColorBlock.highlightedColor;
                    colorBlock.pressedColor = _neutralColorBlock.pressedColor;
                }
                btn.colors = colorBlock;
            }
            Image[] imgs = area.AreaObj.GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                if (area.OwnerID == 0)
                {
                    img.color = _players[0].OwedAreasColor.normalColor;
                }
                else if (area.OwnerID == 1)
                {
                    img.color = _players[1].OwedAreasColor.normalColor;
                }
                else if (area.OwnerID == 11)
                {
                    img.color = _neutralColorBlock.normalColor;
                }
            }
        }
        Button[] buttonsFromTroopsObj = TroopsObj.GetComponentsInChildren<Button>(true);

        foreach (var btn in buttonsFromTroopsObj)
        {
            ColorBlock colorBlock = btn.colors;
            if (_currentPlayerIndex == 0)
            {
                colorBlock.highlightedColor = _players[0].OwedAreasColor.highlightedColor;
                colorBlock.pressedColor = _players[0].OwedAreasColor.pressedColor;
            }
            else if (_currentPlayerIndex == 1)
            {
                colorBlock.highlightedColor = _players[1].OwedAreasColor.highlightedColor;
                colorBlock.pressedColor = _players[1].OwedAreasColor.pressedColor;
            }
            btn.colors = colorBlock;
        }
        Image[] imgsFromTroopsObj = TroopsObj.GetComponentsInChildren<Image>(true);
        foreach (var img in imgsFromTroopsObj)
        {
            if (_currentPlayerIndex == 0)
            {
                img.color = _players[0].OwedAreasColor.normalColor;
            }
            else if (_currentPlayerIndex == 1)
            {
                img.color = _players[1].OwedAreasColor.normalColor;
            }
        }
    }
    public void AreasTroopsAndMoneyInfoUpdate()
    {
        foreach (var area in Areas)
        {
            int sum = 0;
            foreach (var item in area.TroopsGroup)
            {
                sum += item.NumberOfTroops;
            }
            sum += area.TroopsUngrouped;
            area.TroopsTxt.text = sum.ToString();
            if (area.OwnerID != 11)
            {
                area.CoinsTxt.text = _players[area.OwnerID].Money.ToString();
            }
            else
            {
                area.CoinsTxt.text = "0";
            }
        }
    }
    private static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        if (System.Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    private SQLiteConnection CreateConnection()
    {
        //create path to database and connection string
        string relativePath = @"Assets\Plugins\mydb.db";
        string currentPath;
        string absolutePath;
        currentPath = Path.GetDirectoryName(Application.dataPath);
        absolutePath = currentPath + @"\" + relativePath;
        string ConnectionSt = "Data Source=" + absolutePath;
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

    private static void UpdatePlayerStatsTable(
        SQLiteConnection conn,
        string tableName,
        string state,
        int totalMoney,
        int moneyAtTheEnd,
        int totalTroops,
        int troopsAtTheEnd,
        int troopsLost)
    {
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = conn.CreateCommand();
        sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS '"
            + tableName + "' ( " + "  'ID' INTEGER PRIMARY KEY, " +
            "'State' TEXT NOT NULL, " +
            "'Total_money' INTEGER NOT NULL, " +
            "'Money_at_the_end' INTEGER NOT NULL, " +
            "'Total_troops' INTEGER NOT NULL, " +
            "'Troops_at_the_end' INTEGER NOT NULL, " +
            "'Troops_lost' INTEGER NOT NULL);";
        sqlite_cmd.ExecuteNonQuery();
        sqlite_cmd.CommandText = "INSERT INTO " + tableName +
            " (State, Total_money, Money_at_the_end, Total_troops, Troops_at_the_end, Troops_lost) " +
            "VALUES('" +
            state + "', " +
            totalMoney.ToString() + ", " +
            moneyAtTheEnd.ToString() + ", " +
            totalTroops.ToString() + ", " +
            troopsAtTheEnd.ToString() + ", " +
            troopsLost.ToString() + ");";
        sqlite_cmd.ExecuteNonQuery();

        conn.Close();
    }
    private void UpdateScoreboardTable(SQLiteConnection conn)
    {
        SQLiteDataReader sqlite_datareader;
        SQLiteCommand sqlite_cmd;
        sqlite_cmd = conn.CreateCommand();

        for (int i = 0; i < _players.Count; i++)
        {
            int victories = 0;
            int defeats = 0;

            sqlite_cmd.CommandText = "SELECT * FROM Scoreboard WHERE ID = '" + _players[i].IdentificationInfo.ID + "'";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                victories = Convert.ToInt32(sqlite_datareader["Victories"]);
                defeats = Convert.ToInt32(sqlite_datareader["Defeats"]);
            }
            sqlite_datareader.Close();
            if (_players[i].State == "Winner")
            {
                victories += 1;
            }
            else
            {
                defeats += 1;
            }
            sqlite_cmd.CommandText = "UPDATE Scoreboard SET Victories = " + victories + ", Defeats = " + defeats + " WHERE ID = '" + _players[i].IdentificationInfo.ID + "';";
            sqlite_cmd.ExecuteNonQuery();
            
        }

        conn.Close();

    }
}
public class TroopsRegroup
{
    #region MainClassVariables
    private Text _troopGroup1Txt;
    private Text _troopGroup2Txt;
    private Text _troopGroup3Txt;
    private Text _troopGroup4Txt;
    private Text _troopGroup5Txt;
    private Text _troopGroup6Txt;

    private Text _divideTroopNum1Txt;
    private Text _divideTroopNum2Txt;
    private Text _divideTroopNum3Txt;
    private Text _divideTroopNum4Txt;
    private Text _divideTroopNum5Txt;
    private Text _divideTroopNum6Txt;

    private Text _numOfFreeSoldiersTxt;
    private Text _groupsCntTxt;

    private Button _groupsUpBtn;
    private Button _groupsDownBtn;

    private Button _divide1UpBtn;
    private Button _divide1DownBtn;
    private Button _divide2UpBtn;
    private Button _divide2DownBtn;
    private Button _divide3UpBtn;
    private Button _divide3DownBtn;
    private Button _divide4UpBtn;
    private Button _divide4DownBtn;
    private Button _divide5UpBtn;
    private Button _divide5DownBtn;
    private Button _divide6UpBtn;
    private Button _divide6DownBtn;


    public Button TroopGroup1Btn;
    public Button Connect1Btn;
    public Button Divide1Btn;
    public Button TroopGroup2Btn;
    public Button Connect2Btn;
    public Button Divide2Btn;
    public Button TroopGroup3Btn;
    public Button Connect3Btn;
    public Button Divide3Btn;
    public Button TroopGroup4Btn;
    public Button Connect4Btn;
    public Button Divide4Btn;
    public Button TroopGroup5Btn;
    public Button Connect5Btn;
    public Button Divide5Btn;
    public Button TroopGroup6Btn;
    public Button Connect6Btn;
    public Button Divide6Btn;

    public GameObject CreateGroupsObj;
    public Button CreateGroupsBtn;

    private GameObject _troop1Obj;
    private GameObject _troop2Obj;
    private GameObject _troop3Obj;
    private GameObject _troop4Obj;
    private GameObject _troop5Obj;
    private GameObject _troop6Obj;

    private GameObject _group1Obj;
    private GameObject _group2Obj;
    private GameObject _group3Obj;
    private GameObject _group4Obj;
    private GameObject _group5Obj;
    private GameObject _group6Obj;

    public GameObject TroopsListObj;
    public GameObject DivideOrRegroup1Obj;
    public GameObject DivideOrRegroup2Obj;
    public GameObject DivideOrRegroup3Obj;
    public GameObject DivideOrRegroup4Obj;
    public GameObject DivideOrRegroup5Obj;
    public GameObject DivideOrRegroup6Obj;
    public GameObject TroopsObj;
    #endregion

    public List<Troop> TroopsGroupTemp;
    public int NewGroupsNum;
    public int TroopsUngroupedTemp;
    public int TroopIndSelectedToConnect;

    public TroopsRegroup()
    {
        
        TroopIndSelectedToConnect = 11;

        TroopsGroupTemp = new List<Troop>();

        _troopGroup1Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1/TroopGroup1/btnTroopGroup1/txtCurrentTroopNum1").GetComponent<Text>();
        _troopGroup2Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2/TroopGroup2/btnTroopGroup2/txtCurrentTroopNum2").GetComponent<Text>();
        _troopGroup3Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3/TroopGroup3/btnTroopGroup3/txtCurrentTroopNum3").GetComponent<Text>();
        _troopGroup4Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4/TroopGroup4/btnTroopGroup4/txtCurrentTroopNum4").GetComponent<Text>();
        _troopGroup5Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5/TroopGroup5/btnTroopGroup5/txtCurrentTroopNum5").GetComponent<Text>();
        _troopGroup6Txt = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6/TroopGroup6/btnTroopGroup6/txtCurrentTroopNum6").GetComponent<Text>();

        _divideTroopNum1Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group1/txtDivideTroopNum1").GetComponent<Text>();
        _divideTroopNum2Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group2/txtDivideTroopNum2").GetComponent<Text>();
        _divideTroopNum3Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group3/txtDivideTroopNum3").GetComponent<Text>();
        _divideTroopNum4Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group4/txtDivideTroopNum4").GetComponent<Text>();
        _divideTroopNum5Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group5/txtDivideTroopNum5").GetComponent<Text>();
        _divideTroopNum6Txt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group6/txtDivideTroopNum6").GetComponent<Text>();

        _numOfFreeSoldiersTxt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/txtNumOfFreeSoldiers").GetComponent<Text>();
        _groupsCntTxt = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/txtGroupsCnt").GetComponent<Text>();


        _groupsUpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/btnGroupsUp").GetComponent<Button>();
        _groupsUpBtn.onClick.AddListener(NewGroupNumUp);

        _groupsDownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/btnGroupsDown").GetComponent<Button>();
        _groupsDownBtn.onClick.AddListener(NewGroupNumDown);

        _divide1UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group1/btnDivide1Up").GetComponent<Button>();
        _divide1UpBtn.onClick.AddListener(() => DivideGrUp(0));

        _divide1DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group1/btnDivide1Down").GetComponent<Button>();
        _divide1DownBtn.onClick.AddListener(() => DivideGrDown(0));

        _divide2UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group2/btnDivide2Up").GetComponent<Button>();
        _divide2UpBtn.onClick.AddListener(() => DivideGrUp(1));

        _divide2DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group2/btnDivide2Down").GetComponent<Button>();
        _divide2DownBtn.onClick.AddListener(() => DivideGrDown(1));

        _divide3UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group3/btnDivide3Up").GetComponent<Button>();
        _divide3UpBtn.onClick.AddListener(() => DivideGrUp(2));

        _divide3DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group3/btnDivide3Down").GetComponent<Button>();
        _divide3DownBtn.onClick.AddListener(() => DivideGrDown(2));

        _divide4UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group4/btnDivide4Up").GetComponent<Button>();
        _divide4UpBtn.onClick.AddListener(() => DivideGrUp(3));

        _divide4DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group4/btnDivide4Down").GetComponent<Button>();
        _divide4DownBtn.onClick.AddListener(() => DivideGrDown(3));

        _divide5UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group5/btnDivide5Up").GetComponent<Button>();
        _divide5UpBtn.onClick.AddListener(() => DivideGrUp(4));

        _divide5DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group5/btnDivide5Down").GetComponent<Button>();
        _divide5DownBtn.onClick.AddListener(() => DivideGrDown(4));

        _divide6UpBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group6/btnDivide6Up").GetComponent<Button>();
        _divide6UpBtn.onClick.AddListener(() => DivideGrUp(5));

        _divide6DownBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group6/btnDivide6Down").GetComponent<Button>();
        _divide6DownBtn.onClick.AddListener(() => DivideGrDown(5));

        CreateGroupsObj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups");
        CreateGroupsBtn = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/btnCreateGroups").GetComponent<Button>();


        TroopGroup1Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1/TroopGroup1/btnTroopGroup1").GetComponent<Button>();
        TroopGroup2Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2/TroopGroup2/btnTroopGroup2").GetComponent<Button>();
        TroopGroup3Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3/TroopGroup3/btnTroopGroup3").GetComponent<Button>();
        TroopGroup4Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4/TroopGroup4/btnTroopGroup4").GetComponent<Button>();
        TroopGroup5Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5/TroopGroup5/btnTroopGroup5").GetComponent<Button>();
        TroopGroup6Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6/TroopGroup6/btnTroopGroup6").GetComponent<Button>();
        Connect1Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1/DivideOrRegroup1/btnConnect1").GetComponent<Button>();
        Divide1Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1/DivideOrRegroup1/btnDivide1").GetComponent<Button>();
        

        Connect2Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2/DivideOrRegroup2/btnConnect2").GetComponent<Button>();
        Divide2Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2/DivideOrRegroup2/btnDivide2").GetComponent<Button>();
        
        Connect3Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3/DivideOrRegroup3/btnConnect3").GetComponent<Button>();
        Divide3Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3/DivideOrRegroup3/btnDivide3").GetComponent<Button>();
        
        Connect4Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4/DivideOrRegroup4/btnConnect4").GetComponent<Button>();
        Divide4Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4/DivideOrRegroup4/btnDivide4").GetComponent<Button>();
        
        Connect5Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5/DivideOrRegroup5/btnConnect5").GetComponent<Button>();
        Divide5Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5/DivideOrRegroup5/btnDivide5").GetComponent<Button>();
        
        Connect6Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6/DivideOrRegroup6/btnConnect6").GetComponent<Button>();
        Divide6Btn = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6/DivideOrRegroup6/btnDivide6").GetComponent<Button>();
        

        _troop1Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1");
        _troop2Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2");
        _troop3Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3");
        _troop4Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4");
        _troop5Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5");
        _troop6Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6");

        _group1Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group1");
        _group2Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group2");
        _group3Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group3");
        _group4Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group4");
        _group5Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group5");
        _group6Obj = GameObject.Find("Canvas/Play/Field/Troops/CreateGroups/Group6");

        TroopsListObj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList");
        DivideOrRegroup1Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop1/DivideOrRegroup1");
        DivideOrRegroup2Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop2/DivideOrRegroup2");
        DivideOrRegroup3Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop3/DivideOrRegroup3");
        DivideOrRegroup4Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop4/DivideOrRegroup4");
        DivideOrRegroup5Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop5/DivideOrRegroup5");
        DivideOrRegroup6Obj = GameObject.Find("Canvas/Play/Field/Troops/TroopsList/Troop6/DivideOrRegroup6");
        TroopsObj = GameObject.Find("Canvas/Play/Field/Troops");

        DivideOrRegroup1Obj.SetActive(false);
        DivideOrRegroup2Obj.SetActive(false);
        DivideOrRegroup3Obj.SetActive(false);
        DivideOrRegroup4Obj.SetActive(false);
        DivideOrRegroup5Obj.SetActive(false);
        DivideOrRegroup6Obj.SetActive(false);
        TroopsListObj.SetActive(false);
        
    }
    private void NewGroupNumUp()
    {
        if (NewGroupsNum < TroopsUngroupedTemp && NewGroupsNum < 6 - TroopsGroupTemp.Count)
        {
            NewGroupsNum += 1;
        }
        UpdateTroopDivideInfo();
    }
    private void NewGroupNumDown()
    {
        if (NewGroupsNum > 0)
        {
            NewGroupsNum -= 1;
        }
        UpdateTroopDivideInfo();
    }
    public void UpdateTroopDivideInfo()
    {

        if (TroopsGroupTemp.Count > 0)
        {
            _divideTroopNum1Txt.text = TroopsGroupTemp[0].NumberOfTroops.ToString();
            _group1Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum1Txt.text = "0";
            _group1Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 1)
        {
            _divideTroopNum2Txt.text = TroopsGroupTemp[1].NumberOfTroops.ToString();
            _group2Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum2Txt.text = "0";
            _group2Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 2)
        {
            _divideTroopNum3Txt.text = TroopsGroupTemp[2].NumberOfTroops.ToString();
            _group3Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum3Txt.text = "0";
            _group3Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 3)
        {
            _divideTroopNum4Txt.text = TroopsGroupTemp[3].NumberOfTroops.ToString();
            _group4Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum4Txt.text = "0";
            _group4Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 4)
        {
            _divideTroopNum5Txt.text = TroopsGroupTemp[4].NumberOfTroops.ToString();
            _group5Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum5Txt.text = "0";
            _group5Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 5)
        {
            _divideTroopNum6Txt.text = TroopsGroupTemp[5].NumberOfTroops.ToString();
            _group6Obj.SetActive(true);
        }
        else
        {
            _divideTroopNum6Txt.text = "0";
            _group6Obj.SetActive(false);
        }

        _numOfFreeSoldiersTxt.text = TroopsUngroupedTemp.ToString();
        _groupsCntTxt.text = NewGroupsNum.ToString();

    }
    public void UpdateTroopListInfo()
    {

        if (TroopsGroupTemp.Count > 0)
        {
            _troopGroup1Txt.text = TroopsGroupTemp[0].NumberOfTroops.ToString();
            _troop1Obj.SetActive(true);
        }
        else
        {
            _troopGroup1Txt.text = "0";
            _troop1Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 1)
        {
            _troopGroup2Txt.text = TroopsGroupTemp[1].NumberOfTroops.ToString();
            _troop2Obj.SetActive(true);
        }
        else
        {
            _troopGroup2Txt.text = "0";
            _troop2Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 2)
        {
            _troopGroup3Txt.text = TroopsGroupTemp[2].NumberOfTroops.ToString();
            _troop3Obj.SetActive(true);
        }
        else
        {
            _troopGroup3Txt.text = "0";
            _troop3Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 3)
        {
            _troopGroup4Txt.text = TroopsGroupTemp[3].NumberOfTroops.ToString();
            _troop4Obj.SetActive(true);
        }
        else
        {
            _troopGroup4Txt.text = "0";
            _troop4Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 4)
        {
            _troopGroup5Txt.text = TroopsGroupTemp[4].NumberOfTroops.ToString();
            _troop5Obj.SetActive(true);
        }
        else
        {
            _troopGroup5Txt.text = "0";
            _troop5Obj.SetActive(false);
        }
        if (TroopsGroupTemp.Count > 5)
        {
            _troopGroup6Txt.text = TroopsGroupTemp[5].NumberOfTroops.ToString();
            _troop6Obj.SetActive(true);
        }
        else
        {
            _troopGroup6Txt.text = "0";
            _troop6Obj.SetActive(false);
        }

    }
    private void DivideGrUp(int ind)
    {
        if (TroopsGroupTemp.Count >= (ind + 1))
        {
            if (TroopsUngroupedTemp >= 1 && !TroopsGroupTemp[ind].MovedThisTurn)
            {
                TroopsGroupTemp[ind].NumberOfTroops += 1;
                TroopsUngroupedTemp -= 1;
                if (NewGroupsNum > TroopsUngroupedTemp)
                {
                    NewGroupsNum -= 1;
                }
            }
        }
        UpdateTroopDivideInfo();
    }
    private void DivideGrDown(int ind)
    {
        if (TroopsGroupTemp.Count >= (ind + 1))
        {
            if (TroopsGroupTemp[ind].NumberOfTroops > 0 && !TroopsGroupTemp[ind].MovedThisTurn)
            {
                TroopsGroupTemp[ind].NumberOfTroops -= 1;
                TroopsUngroupedTemp += 1;
            }
        }
        UpdateTroopDivideInfo();
    }
}