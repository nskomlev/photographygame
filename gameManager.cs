using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/*  $$$
 * 
 * 
 * 
 * БАГИ
 * ЭФЕКТЫ
 * 
 * TODO: мини бд про все игровые объекты 
 *  
 * ГЕЙМПЛЕЙ
 * TODO: Когда дождь много цветов и дельфинов, а мент отсанавливается!!!
 * TODO: РЕДКИЙ ЦВЕТОК ЛИ ОБЪЕКТ!!!
 * TODO: 3 тий штраф отбирают додектив
 * TODO: реклама раз в уровень!
 * TODO: как и за что увеличить дань полицейского, тогда возможно покупка новой камеры будет уместна
 * 
 * +создать внутри игровые покупки
 * +кОНЕЦ ИГРЫ \\ сделать уровни coming soon
 * +КАК ИГРАТЬ
 * +Кнопки пикселями 
 * +Сохранения уровня на мобильном
 * +Рекламу в магазине на 3тий раз
 * +звук когда лодка поймал дельфина ИЛИ когда дельфин вылез - чайка
 * +магазин ночью
 * -Когда телефон вспышка и пленка не нужна???
 * +Управление тачем, при простом касание ничего не делать!!
 * +звуки ночи сверчки
 * +подсветка триггнров на полицейском и на горящем дома
 * +ДОждь не перестает лить (убрал убрав случанйость )
 * +когда телефон, нет объективов, но если купил то остаются
 * +НОВЫЕ ОБЪЕКТИВЫ (например только по диагонали бъют)
 * +ПОГОДА!!! девушки не выходят / или с зонтом / цветы растут повсюду
 * +сохранить сколько денег, что куплену, пленку, текущий уровень
 * +главное меню
 * +деньги после уровня
 * +сложность проще - может по началу по одному заданию
 * +добавить камеры телефон, простой фотоапаарат
 * +ЕСЛИ застрял то проверить на наличие 3ех одиноковых ходов или придумать как обогнутьф
 * +пленка исчезает
 * +при съмке в дождь в полтора раза, при съемке ночи в 2 раза очков за фотографию
 * +Вспышка кончается ? батарейки?
 * +звук покупки вспышки
 * +звук когда снимаешь полицейского на вспышку
 * +вспышка под игроком ночью
 * +пожар от дома слышаг гораздо дальше
 * +Ослепить полицескогого
 * +ПРОДУМАТЬ УРОВНИ и СОХРАНЕНИЯ
 * +ЦВЕТЫ НА ДОМЕ!!!
 * +ЗВУКИ пояления кита
 * +вспышка и когда ночью нет вспыки
 * +ЗВУК НОВОГО УРОВНЯ
 * +Если в доме то нельзя полицейскому взят ьштраы
 * +Задачи - СФОТКАТЬ 3 КОРАБЛЯ ИТП
 * +ЗВУКИ пояления нло
 * +ЗВУКИ если сфоткал с умноджением
 * +Ночь
 * +ДЕЛЬФИН только когда вынырунл (но видно его заранее)
 * +АНДРОЙД если в магазине то не действует кнопки выхода
 * +ДЕВШУКА - ИДЕТ НА ЦВЕТОК - ПОТОМ - ДОМОЙ
 * +НЛО - ИЩЕТ ДЕВУШКУ - УЛЕТАЕТ
 * +КОРАБЛЬ - ЛОВИТ ДЕЛЬФИНА- в угол экрана
 * +ВСе объекты врменные - девушки вообще пусть идут из одного дома в другой
 * +Цветы должны расти / будет аналоги с китом который поялется исчезает
 * +Девушки появлеютс из домов?
 * +Шорящий дом
 * +Полицейский всегда слеав сверху
 * +Игрук у машины всегда начинает
 * +НЛО - НЛО ЗАБИРАЕТ
 * +ДЕВУШКА - ДЕВУШКА С ЦВЕТКОМ
 * +КОРАБЛЬ - КОРАБЛЬ ПОЙМАЛ ДЕЛЬФИНА
 * +ДОМ ГОРИТ
 * +ЦВЕТОК
 * 
 */
public enum curWeatherEnum { sun, rain, night };

public class gameManager : MonoBehaviour {

    private List<objectsClass> enemies;                          //List of all Enemy units, used to issue them move commands.
    private List<objectsClass> objects;
    private List<objectsClass> items;
    private List<objectsClass> objectsNotWalk;
    public List<objectsClass> objectsBuildings;
    public List<objectsClass> allObjects;
    private List<string> allObjectsCanAppear;

    public static gameManager instance;
    public enum curGameStateEnum {playerMove, enemyMoving, enemyWait};

    public enum game_state { startGame, continueGame, pauseGame };
    public static game_state GAME_STATE;

    public float turnDelay = 0.3f;
    public float restartLevelDelay = 1f;
    public static curGameStateEnum curGameState;
    public static curWeatherEnum curWeather;

    public static Vector3 playrPressVector;


    public GameObject PLAYER;
    public GameObject Rain;
    public GameObject Night;
    public static int turnsNumber =0;
    public static int timeFromLastShoot = 0;
    public static int timeFromLastCollect = 0;
    public int timeShopVisit = 0;

    private int level = 0;
    public float levelStartDelay = 2f;
    private Text levelText;
    private GameObject levelImg;
    public static bool doingSetup;

    public static bool canAddNeSet;


    public GameObject score;
    public GameObject films;
    public GameObject flashs;
    public GameObject penalty;

    Text txt;
    Text txtFilm;
    Text txtFlashs;
    Text txtPenalty;

    // Атрибуты Игрока
    public static int playerMoney = 0;
    public static int playerLevel = 1;
    public static int playerPenalty = 0;
    public itemStore currentFilms;
    public itemStore currentLens;
    public itemStore currentCamera;
    public itemStore currentFlash;


    public buildMap BuildMap;
    GameObject panel;


    public static Dictionary<string, int> STATA = new Dictionary<string, int>()
    {
        {"days",0},
        {"shoots",0},
    };
    public static Dictionary<string, int> COST = new Dictionary<string, int>()
    {
        {"Enemy1",-40},

        {"Flower",12},
        {"Girl",25},
        {"Building1",40},
        {"Building2",40},
        {"Building3",40},
        {"Ufo",50},
        {"Boat",70},
        {"Delf",130},

        {"Film",5},
        {"600SL",100},
        {"Money",0},

        {"m50",200},
        {"m24",450},
        {"m135",600},
        {"m815",1400},
        {"m24105",2900},
        {"m70200",6800},
        {"m30300",18000},

        {"1F",3300},
        {"6F",1800},
        {"550F",850},
        {"275F",330},
        {"phoneF",250}

    };


    public static Dictionary<string, Dictionary<string, int>> allObjectsPlaceCond = new Dictionary<string, Dictionary<string, int>>
    {
        { "Flower", new Dictionary<string, int> { { "notMoreThen", 6 }, { "everyNdays", 4 },  { "fromLastShoot", 4 } } },
        { "Film", new Dictionary<string, int>   { { "notMoreThen", 1 }, { "everyNdays", 7 },  { "fromLastShoot", 5 } } },
        { "Girl", new Dictionary<string, int>   { { "notMoreThen", 2 }, { "everyNdays", 10 }, { "fromLastShoot", 6 } } },
        { "Delf", new Dictionary<string, int>   { { "notMoreThen", 1 }, { "everyNdays", 19 }, { "fromLastShoot", 7 } } },
        { "Boat", new Dictionary<string, int>   { { "notMoreThen", 1 }, { "everyNdays", 31 }, { "fromLastShoot", 9 } } },
        { "Ufo", new Dictionary<string, int>    { { "notMoreThen", 1 }, { "everyNdays", 43 }, { "fromLastShoot", 11 } } },

    };
    

    public static Dictionary<string, Dictionary<string, int>> environmentEvents = new Dictionary<string, Dictionary<string, int>> // события природы Дождь продолжительност, частота в ходах игрока, процент случайности
    {
        { "Rain", new Dictionary<string, int>  { { "during", 18 }, { "every", 80 }, { "randomProc", 0 }, { "startDay", 0 } } },
        { "Night", new Dictionary<string, int> { { "during", 12 }, { "every", 20 }, { "randomProc", 0 },  { "startDay", 0 } } }
    };

    public Dictionary<string, int> allObjectsOnScene = new Dictionary<string, int>() // Сюда пишем количсевто игровых объектов на сцене по типу
    {
        {"Girl",0},
        {"Enemy1",0},
        {"Flower",0},
        {"Delf",0},
        {"Ufo",0},
        {"Boat",0},
        {"Building1",0},
        {"Building2",0},
        {"Building3",0},
        {"Film",0},
        {"TOTAL",0}
    };

    public static Dictionary<string, int> allObjectsLayers = new Dictionary<string, int>() // Слои?
    {
        {"Girl",-3},
        {"Enemy1",-3},
        {"Player",-3},
        {"Flower",-2}, 
        {"Delf",-3},
        {"Ufo",-5},
        {"Boat",-4},
        {"Building1",-1},
        {"Building2",-1},
        {"Building3",-1},
        {"Film",-2},
        {"exitufo",-2},
        {"exitsea",-2},
    };

    public static Dictionary<string, int> needToCollectForNextAction = new Dictionary<string, int>() // Сколько надо совершить объект для сдужцщего действия (уйти домой, повысить)
    {
        {"Girl",3},
        {"Enemy1",3},
        {"Player",15},
        {"Ufo",2},
        {"Boat",2},
    };
    public static Dictionary<string, int> movePriotity = new Dictionary<string, int>() // Порядок движения объектов
    {
        {"Player",100},
        {"Enemy1",85},
        {"Girl",50},
        {"Ufo",90},
        {"Boat",80},
        {"Delf",70},
        {"Flower",10},
        {"Building1",10},
        {"Building2",10},
        {"Building3",10},
        {"store",10},
        {"Film",10}
    };


    public Dictionary<string, List<string>> levelTargetClasses = new Dictionary<string, List<string>>
    {
        { "Class1", new List<string> { "Flower", "Girl" , "Girl2" } },
        { "Class2", new List<string> { "Building", "Ufo" , "Ufo2" } },
        { "Class3", new List<string> { "Boat", "Delf", "Boat2" } }
    };

    public Dictionary<int, Dictionary<string, curWeatherEnum>> levelTargetTemplates = new Dictionary<int, Dictionary<string, curWeatherEnum>>
    {
         { 1, new Dictionary<string, curWeatherEnum>   { { "Class1", curWeatherEnum.sun },      { "Class2", curWeatherEnum.sun },   { "Class3", curWeatherEnum.sun } } },
         { 2, new Dictionary<string, curWeatherEnum>   { { "Class1", curWeatherEnum.night },     { "Class2", curWeatherEnum.sun },   { "Class3", curWeatherEnum.sun } } },
         { 3, new Dictionary<string, curWeatherEnum>   { { "Class1", curWeatherEnum.night },    { "Class2", curWeatherEnum.rain },  { "Class3", curWeatherEnum.sun } } },
         { 4, new Dictionary<string, curWeatherEnum>   { { "Class1", curWeatherEnum.rain },    { "Class2", curWeatherEnum.night },  { "Class3", curWeatherEnum.sun } } }
    };


    public List<levelTypes> levelTypeList = new List<levelTypes>();


    public static List<levelClass> levels = new List<levelClass>();
    public static int LEVEL=0;

    public static List<itemStore> playerItem = new List<itemStore>();

    // Use this for initialization
    void Awake () {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        ////////////////////////////////////////////////////////DontDestroyOnLoad(gameObject);
        GAME_STATE = game_state.startGame;

        //soundManager.instance.PlaySingle("none", "music", 80);
        float needRatio = 1.75F;
        float ratio2;
        float ratio = (float)   Screen.width / Screen.height;
        //print(ratio);

        if (needRatio < ratio)
            ratio2 = 800;
        else
            ratio2 = 1400 / ratio;

        double ortSize = ratio2 / 200f;
        Camera.main.orthographicSize = (float) ortSize;


        enemies = new List<objectsClass>();
        objects = new List<objectsClass>();
        objectsNotWalk = new List<objectsClass>();
        objectsBuildings = new List<objectsClass>();
        items = new List<objectsClass>();
        BuildMap = GetComponent<buildMap>();
        
        var levelNum = 1;
       // var levelTotal = 1;
        soundManager.instance.mute(false,false);

        //очищаем , нужно для перезапуска restart
        levels.Clear();
        playerItem.Clear();

        //Типы уровней по нврастанию сложночти
        levelTypeList.Add(new levelTypes(1, "Flower",       3, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(2, "Flower",       3, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(3, "Girl",         3, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(4, "Flower",       3, curWeatherEnum.rain));
        levelTypeList.Add(new levelTypes(5, "Building",     2, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(6, "Girl2",        3, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(7, "Girl",         3, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(8, "Ufo",          2, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(9, "Girl",         3, curWeatherEnum.rain));
        levelTypeList.Add(new levelTypes(10, "Boat",        1, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(11, "Building",    2, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(12, "Ufo2",        2, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(13, "Building",    2, curWeatherEnum.rain));
        levelTypeList.Add(new levelTypes(14, "Girl2",       3, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(15, "Delf",        1, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(16, "Ufo",         2, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(17, "Boat",        1, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(18, "Boat2",       1, curWeatherEnum.sun));
        levelTypeList.Add(new levelTypes(19, "Ufo2",        1, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(20, "Delf",        1, curWeatherEnum.night));
        levelTypeList.Add(new levelTypes(21, "Boat2",       1, curWeatherEnum.night));


        for (int i=0;i<=2;i++)
        {
            if (i==0) //простые уровни 1  первые 7 
            {
                for (int ii=0;ii<7;ii++)
                {
                    levels.Add(new levelClass(levelNum, levelTypeList[ii].name, 0, levelTypeList[ii].weather, levelTypeList[ii].amount,
                                                        "", 0, levelTypeList[ii + 7].weather, 0,
                                                        "", 0, levelTypeList[ii + 14].weather, 0, //"", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0,                                                                                  //
                                                        (levelNum + 1) * 15
                        ));
                    levelNum++;
                }
            }
            else if (i == 1) //суть сложнее уровни 2 
            {
                for (int ii = 0; ii < 7; ii++)
                {
                    levels.Add(new levelClass(levelNum, levelTypeList[ii].name, 0, levelTypeList[ii].weather, levelTypeList[ii].amount,
                                                        levelTypeList[ii+7].name, 0, levelTypeList[ii+7].weather, levelTypeList[ii+7].amount,
                                                        "", 0, levelTypeList[ii + 14].weather, 0, //"", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0,                                                                                  //
                                                        (levelNum + 1) * 20
                        ));
                    levelNum++;
                }
            }
            else if (i == 2) //обычные уровни 2 
            {
                for (int ii = 0; ii < 7; ii++)
                {
                    levels.Add(new levelClass(levelNum, levelTypeList[ii].name, 0, levelTypeList[ii].weather, levelTypeList[ii].amount,
                                                        levelTypeList[ii + 7].name, 0, levelTypeList[ii + 7].weather, levelTypeList[ii + 7].amount,
                                                        levelTypeList[ii + 14].name, 0, levelTypeList[ii + 14].weather, levelTypeList[ii + 14].amount,                                                                                  //
                                                        (levelNum + 1) * 25
                        ));
                    levelNum++;
                }
            }
            //Первые 7ьм уровней
            
        }

        //ДЕЛАЕМ УРОВНИ

        /*
        foreach (KeyValuePair<int, Dictionary<string, curWeatherEnum>> targTempl in levelTargetTemplates)
        {

            for (int i = 0; i < levelTargetClasses["Class3"].Count; i++)
            {
                for (int ii = 0; ii < levelTargetClasses["Class2"].Count; ii++)
                {
                    for (int iii = 0; iii < levelTargetClasses["Class1"].Count; iii++)
                    {
                       // if (levelTotal % 2 == 0)//(levelNum%3==0)?(levelNum*20) :(0)  
                      //  {
                            if(levelNum<=3)
                            levels.Add(new levelClass(levelNum, levelTargetClasses["Class1"][iii],  0,  levelTargetTemplates[targTempl.Key]["Class1"], 3,
                                                                "", 0, levelTargetTemplates[targTempl.Key]["Class2"], 0,
                                                                "", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0, //"", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0,                                                                                  //
                                                                (levelNum+1) * 10
                                ));
                            else if (levelNum >3 && levelNum <= 9)
                                levels.Add(new levelClass(levelNum, levelTargetClasses["Class1"][iii], 0, levelTargetTemplates[targTempl.Key]["Class1"], 3,
                                                                    levelTargetClasses["Class2"][ii], 0, levelTargetTemplates[targTempl.Key]["Class2"], 2,
                                                                    "", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0, //"", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0,                                                                                  //
                                                                    (levelNum + 1) * 15
                                    ));
                            else if(levelNum > 9)
                                levels.Add(new levelClass(levelNum, levelTargetClasses["Class1"][iii], 0, levelTargetTemplates[targTempl.Key]["Class1"], 3,
                                                                    levelTargetClasses["Class2"][ii], 0, levelTargetTemplates[targTempl.Key]["Class2"], 2,
                                                                    levelTargetClasses["Class3"][i], 0, levelTargetTemplates[targTempl.Key]["Class3"], 1, //"", 0, levelTargetTemplates[targTempl.Key]["Class3"], 0,                                                                                  //
                                                                    (levelNum + 1) * 20
                                    ));

                            levelNum++;
                      //  }
                        levelTotal++;
                        if (levelNum > 50) break; // TODO
                    }
                    if (levelNum > 50) break; // TODO
                }
                if (levelNum > 50) break; // TODO
            }
            if (levelNum > 50) break; // TODO
        }*/
    }

    public void checkCurLevel(string name,bool super) //Проверка на наступления новый уровень
    {
        if (super) name = name + "2";
        if (buildMap.allObjectsCanNotWalkNotDisaper.Contains(name)) name = "Building";
        int nextLEVEL = LEVEL;
       // print(name);
        if      (levels[nextLEVEL].Class1Name == name && (levels[nextLEVEL].Class1Weather == curWeather))// || levels[nextLEVEL].Class2Weather == curWeatherEnum.sun
        {
            print(name+"_1");
            if (levels[nextLEVEL].Class1Count < levels[nextLEVEL].Class1Total)
            { levels[nextLEVEL].Class1Count++;
                panel.GetComponent<panels>().updateLevelsItems(1);
            }
        }
        else
        if (levels[nextLEVEL].Class2Name == name && (levels[nextLEVEL].Class2Weather == curWeather))// || levels[nextLEVEL].Class2Weather == curWeatherEnum.sun
        {
            print(name + "_2");
            if (levels[nextLEVEL].Class2Count < levels[nextLEVEL].Class2Total)
            {
                levels[nextLEVEL].Class2Count++;
                panel.GetComponent<panels>().updateLevelsItems(2);
            }
        }
        else
        if (levels[nextLEVEL].Class3Name == name && (levels[nextLEVEL].Class3Weather == curWeather))// || levels[nextLEVEL].Class3Weather == curWeatherEnum.sun
        {
            print(name + "_3");
            if (levels[nextLEVEL].Class3Count < levels[nextLEVEL].Class3Total)
            { levels[nextLEVEL].Class3Count++;
                panel.GetComponent<panels>().updateLevelsItems(3);
            }
        }

        if (levels[nextLEVEL].Class1Count== levels[nextLEVEL].Class1Total && levels[nextLEVEL].Class2Count == levels[nextLEVEL].Class2Total && levels[nextLEVEL].Class3Count == levels[nextLEVEL].Class3Total)
        {
            //NEW LEVEL
            LEVEL++;
            panel.GetComponent<panels>().updateLevelsItems(0);
            makeAction("newLevel","", nextLEVEL); // TODO: деньги в кассу!! 
        }

    }
    // Update is called once per frame
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (curGameState == curGameStateEnum.playerMove)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        if (curGameState == curGameStateEnum.enemyWait)
        {
            StartCoroutine(MoveEnemies(enemies));
            StartNonWalkObjectsUpdateSatet();
            CheckForEverydayOptions(allObjects);
        }
        CheckIfNewObjectsNeedToPlace();
        CheckIfGameOver();

    }

    public void InitGame()
    {
       // print(allObjectsPlaceCond["Flower"]["notMoreThen"]);
        soundManager.instance.mute(false, true);
        curWeather = curWeatherEnum.sun;
        playerItem.Add(new itemStore("m30300", "30-300mm", "m30300", "Telephoto zoom lens\nAll options", 0, 0, "lens", 0, new List<Vector2>() { new Vector2(-2, 0), new Vector2(-3, 0), new Vector2(2, 0), new Vector2(3, 0), new Vector2(0, -2), new Vector2(0, -3), new Vector2(0, 2), new Vector2(0, 3), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, 1), new Vector2(0, 1) }));
        playerItem.Add(new itemStore("m70200", "70-200mm", "m70200", "Telephoto lens\nOnly far distance", 0, 0, "lens", 0, new List < Vector2 >(){        new Vector2(-2,0),new Vector2(-3,0),        new Vector2(2,0),new Vector2(3,0),        new Vector2(0,-2),new Vector2(0,-3),        new Vector2(0,2),new Vector2(0,3)}));
        playerItem.Add(new itemStore("m24105", "24-105mm", "m24105", "Telephoto lens\nFar distance", 0, 0, "lens", 0, new List<Vector2>() {        new Vector2(-1,0),new Vector2(-2,0),        new Vector2(1,0),new Vector2(2,0),        new Vector2(0,-1),new Vector2(0,-2),        new Vector2(0,1),new Vector2(0,2)}));
        playerItem.Add(new itemStore("m815", "8-15mm", "m815", "Very wide-angle lens\nAll near", 0, 0, "lens", 0, new List<Vector2>() {        new Vector2(1,1),new Vector2(-1, 0),        new Vector2(-1,-1),new Vector2(1, 0),        new Vector2(1,-1),new Vector2(0, -1),        new Vector2(-1,1),new Vector2(0, 1)}));
        playerItem.Add(new itemStore("m135", "135mm", "m135", "Telephoto lens\nFar distance", 0, 0, "lens", 0, new List<Vector2>() {        new Vector2(-2,0),        new Vector2(2,0),        new Vector2(0,-2),        new Vector2(0,2)}));
        playerItem.Add(new itemStore("m24", "24mm", "m24", "Wide-angle lens\nAll coners", 0, 0, "lens", 0, new List<Vector2>() {        new Vector2(-1,1),        new Vector2(1,1),        new Vector2(1,-1),        new Vector2(-1,-1)}));
        playerItem.Add(new itemStore("m50", "50mm", "m50", "Classic lens\nBasic", 0, 1, "lens", 0, new List<Vector2>() {        new Vector2(-1,0),        new Vector2(1,0),        new Vector2(0,-1),        new Vector2(0,1)}));

        playerItem.Add(new itemStore("1F", "1F", "1F", "Pro full-frame camera\nscore X1.5", 0, 0, "camera",1.5F));
        playerItem.Add(new itemStore("6F", "6F", "6F", "Enthusiast full-frame camera\nscore X1.2", 0, 0, "camera", 1.2F));
        playerItem.Add(new itemStore("550F", "550F", "550F", "Beginner half-frame camera\nscore X1", 0, 0, "camera", 1F));
        playerItem.Add(new itemStore("275F", "275F", "275F", "Simple digital camera\nscore X0.7", 0, 0, "camera", 0.7F));
        playerItem.Add(new itemStore("phoneF", "phoneF", "phoneF", "Smartphone with camera\nscore X0.5", 0, 1, "camera", 0.5F));

        currentFlash = new itemStore("600SL", "600SL", "600SL", "Сamera flash X10 \nFor night/police", 1, 0, "flash");
        playerItem.Add(currentFlash);
        currentFilms = new itemStore("Film", "Films", "Film", "Film for photos\n X1 Film", 1, 0, "Film");
        playerItem.Add(currentFilms);

#if UNITY_ANDROID || UNITY_EDITOR
        playerItem.Add(new itemStore("Money", "Buy 4000$", "Money", "Buy 4000$ \n for 1 USD", 1, 0, "Money"));
#endif


        //LOAD PLAYER CAMERAS AND LENS

        string setCurCamTmp = "phoneF";
        string  setCurLensTmp = "m50";
        
        if (PlayerPrefs.HasKey("currentCamera"))
        setCurCamTmp = PlayerPrefs.GetString("currentCamera");

        if (PlayerPrefs.HasKey("currentLens"))
        setCurLensTmp = PlayerPrefs.GetString("currentLens");

        //TEMP
        //setCurLensTmp = "m30300";


        for (int i = 0; i < playerItem.Count; i++)
        {
            if (playerItem[i].TYPE == "camera" || playerItem[i].TYPE == "lens")
            {
                if(PlayerPrefs.HasKey(playerItem[i].name + "_have")) // ЕСЛИ ЕСТЬ информация по сохраению камеры или линзы
                    playerItem[i].playerHave=PlayerPrefs.GetInt(playerItem[i].name + "_have"); //LOAD То сохранить в переменную
            }

            if (setCurCamTmp== playerItem[i].name)                currentCamera = playerItem[i];
            if (setCurLensTmp == playerItem[i].name)                currentLens = playerItem[i];
        }


        // LOAD DATA SAVEDATA





        playerPenalty = PlayerPrefs.GetInt("Penalty");

        currentFilms.playerHave = PlayerPrefs.GetInt("Films");
        if (currentFilms.playerHave < 3)
            currentFilms.playerHave = 3;

        currentFlash.playerHave = PlayerPrefs.GetInt("Flashs");
        if (currentFlash.playerHave < 3)
            currentFlash.playerHave = 3;

        LEVEL = 0;
        LEVEL = PlayerPrefs.GetInt("LEVEL",0);
        //TEMP
        //LEVEL = 16;

        playerMoney = PlayerPrefs.GetInt("Money");
        if (playerMoney < 25)
        {
            float tmpMoneyForLevel = 90 * LEVEL * 0.5F;
            playerMoney = (int) Mathf.Round(tmpMoneyForLevel);
        }
        //TEMP
        if(playerMoney>99999)
        playerMoney = 4000;
        //print(LEVEL);

        //ПОКА ТЕСТ ВРЕМЕННО TEST tEST TEST

        /*
        LEVEL = 0;
        playerMoney = 5000;
        currentFlash.playerHave = 0;
        currentFilms.playerHave = 0;
        */



        // END LOAD DATA

        panel = GameObject.Find("Canvas");
        curGameState = curGameStateEnum.playerMove;
        score = GameObject.Find("scoretext");
        films = GameObject.Find("filmtext");
        flashs = GameObject.Find("flashtext");
        penalty = GameObject.Find("penaltytext");

        txt = score.GetComponent<Text>();
        txtFilm = films.GetComponent<Text>();
        txtFlashs = flashs.GetComponent<Text>();
        //  txtPenalty = penalty.GetComponent<Text>();
        playerPenalty = 0;

        setNewScore(playerMoney,false);
        setNewFilms(currentFilms.playerHave, false);
        setNewFlashs(currentFlash.playerHave, false);
        setNewPenalty(playerPenalty, false);

        doingSetup = true;
        levelImg = GameObject.Find("LevelImg");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Begin your photo trip!";// + level;
        levelImg.SetActive(true);
        

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
        objects.Clear();
        items.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        BuildMap.SetupScene(level);
        BuildMap.addObjectInGame("exitsea");
        BuildMap.addObjectInGame("exitufo");
        Invoke("HideLevelImage", levelStartDelay);
    }
    //**********************
    // ГЕНЕРАЦИЯ ОБЪЕКТОВ //
    //**********************


    void CheckIfNewObjectsNeedToPlace() // ГЕНЕРАЦИЯ ПЕРСОНАЖЕЙ и СОБЫТИЙ ПРИРОДЫ
    {
        if(GAME_STATE==game_state.continueGame)
        if (canAddNeSet)
        {
            allObjectsOnScene["TOTAL"] = 0;
            foreach (KeyValuePair<string, Dictionary<string, int>> attachStat in allObjectsPlaceCond)
            {
                allObjectsOnScene["TOTAL"] = allObjectsOnScene["TOTAL"]+ allObjectsOnScene[attachStat.Key];
            }

            foreach (KeyValuePair<string, Dictionary<string, int>> attachStat in allObjectsPlaceCond)
            {
                if (allObjectsOnScene[attachStat.Key] < allObjectsPlaceCond[attachStat.Key]["notMoreThen"] &&
                    turnsNumber % allObjectsPlaceCond[attachStat.Key]["everyNdays"] == 0 &&
                    canAddNeSet && timeFromLastShoot >= allObjectsPlaceCond[attachStat.Key]["fromLastShoot"] &&
                    allObjectsOnScene["TOTAL"] <= 10)
                {
                    BuildMap.addObjectInGame(attachStat.Key);
                }
            }
            // **************** ************** 
            // ********  RAIN  ************** 
            // ******** ******** ************** 
            foreach (KeyValuePair<string, Dictionary<string, int>> attachStat in environmentEvents)
            {
               /* float tmpSH = ((float)environmentEvents[attachStat.Key]["randomProc"] / 100) * (float)environmentEvents[attachStat.Key]["every"];
                print(tmpSH);*/

                float tmpRand1 = Mathf.Ceil((((Random.value - 0.5) > 0) ? (1) : (-1)) * Random.Range(0, (((float)environmentEvents[attachStat.Key]["randomProc"] / 100) * environmentEvents[attachStat.Key]["every"])));
                float tmpRand2 = Mathf.Ceil((((Random.value - 0.5) > 0) ? (1) : (-1)) * Random.Range(0, (((float)environmentEvents[attachStat.Key]["randomProc"] / 100) * environmentEvents[attachStat.Key]["during"])));
                

                if (turnsNumber % ((environmentEvents[attachStat.Key]["every"]+ tmpRand1)) == 0)
                {
                    environmentEvents[attachStat.Key]["startDay"] = turnsNumber;
                    makeAction(attachStat.Key, "start");
                }

                if ((environmentEvents[attachStat.Key]["startDay"] + tmpRand2 + environmentEvents[attachStat.Key]["during"] - turnsNumber == 0) && environmentEvents[attachStat.Key]["startDay"]>0)
                {
                    environmentEvents[attachStat.Key]["startDay"] = 0;
                    makeAction(attachStat.Key, "stop");
                }
            }

            canAddNeSet = false; // Делает true Игрок см его класс
        }
    }


    /*-------------------------------*/
    /*------   MAKEACTION  ------------*/
    /*-------------------------------*/


    void makeAction(string what, string toDo = "", int ifNeed = 0)
    {
        switch (what)
        {
            case "Rain":

                if (toDo == "start")
                {
                    if (curWeather == curWeatherEnum.sun)
                    {
                        curWeather = curWeatherEnum.rain;
                        Rain.SetActive(true);
                        soundManager.instance.PlaySingle("rain", "rainChanel", 100);
                    }
                }
                else
                {
                    if (curWeather == curWeatherEnum.rain)
                    {
                        curWeather = curWeatherEnum.sun;
                        Rain.SetActive(false);
                        soundManager.instance.PlaySingle("rain", "rainChanel", 0);
                    }
                }
                break;
            case "Night":

                if (toDo == "start")
                {
                    if (curWeather == curWeatherEnum.sun)
                    {
                        curWeather = curWeatherEnum.night;
                        StartCoroutine(smoothEverything(Night.GetComponent<SpriteRenderer>(),"alpha",0,0.5F));
                        Night.SetActive(true);
                        soundManager.instance.PlaySingle("night", "nightChanel", 65);
                        soundManager.instance.PlaySingle("forest", "forestChanel", 0);
                    }
                }
                else
                {
                    if (curWeather == curWeatherEnum.night)
                    {
                        curWeather = curWeatherEnum.sun;
                        StartCoroutine(smoothEverything(Night.GetComponent<SpriteRenderer>(), "alpha", 0.5F, 0));
                        Night.SetActive(false);
                        soundManager.instance.PlaySingle("night", "nightChanel", 0);
                        soundManager.instance.PlaySingle("forest", "forestChanel",100);
                    }
                }
                break;

            case "newLevel":
                print("NEW LEVEL!!! "+ ifNeed);

                if (levels[LEVEL - 1].plusScore>0) { 
                    //Vector3 tmpVec = Camera.main.WorldToScreenPoint(PLAYER.transform.position);
                    panel.GetComponent<panels>().showScorePlus(PLAYER.transform.position, ("+" + levels[LEVEL-1].plusScore));
                    panel.GetComponent<panels>().showText(new Vector3(0f , 0f,0f), ("NEW LEVEL " + (LEVEL+1)));
                    setNewScore(+levels[LEVEL - 1].plusScore);
                }
                // ДЕНЬГИ ЗА У

                PlayerPrefs.SetInt("LEVEL", LEVEL);
                soundManager.instance.PlaySingle("newlevel", "fx", 75);
                admob.instance.pauseGameButton();
                break; 

        }
    }
    protected IEnumerator smoothEverything(SpriteRenderer sprite,string what,float begin, float end)
    {
        switch (what)
        {
            case "alpha":
                Color tmpClr = sprite.color;

                float bg = begin;
                while (bg < end)
                {
                    bg += 0.02F;
                    sprite.color = new Color(tmpClr.r, tmpClr.g, tmpClr.b, bg);
                    yield return null;
                }

                break;
        }


    }

    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImg.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    public void AddEnemyToList(objectsClass script)    {        enemies.Add(script);    }
    public void AddObjectsToList(objectsClass script)    {        objects.Add(script);    }
    public void AddCollectItemsToList(objectsClass script)    {        items.Add(script);    }
    public void AddObjectsNoTWalkToList(objectsClass script)    {        objectsNotWalk.Add(script);    }
    public void AddPlayerItemsToList(itemStore script)    {        playerItem.Add(script);    }
    public void AddPlayerObjectsBuildings(objectsClass script)    {        objectsBuildings.Add(script);    }
    public void AddAllObjects(objectsClass script) { allObjects.Add(script); }

    public List<itemStore> getPlayerItems()
    {
        return playerItem;
    }

    public void RemoveEnemyToList(objectsClass script,Vector2 curPos)
    {
        
        //wprint(script);
        //Remove Enemy to List enemies.
        if(buildMap.allObjectsSea.Contains(script.name))
        BuildMap.removeObjFromMap(curPos,"sea");
        else
        BuildMap.removeObjFromMap(curPos,"ground");

        allObjectsOnScene[script.name]=allObjectsOnScene[script.name] - 1;
        enemies.Remove(script);
        objects.Remove(script);
        objectsNotWalk.Remove(script);
        items.Remove(script);
        allObjects.Remove(script);
        //if (script.name == "Girl")

    }

    IEnumerator MoveEnemies(List<objectsClass> tmpObjs)
    {
        if (GAME_STATE == game_state.continueGame)
        {

            PLAYER = GameObject.Find("Player");
            PLAYER.GetComponent<objectsClass>().clearPole();

            tmpObjs = tmpObjs.OrderBy(go => go.movePriotity).ToList();

            //While enemiesMoving is true player is unable to move.
            curGameState = curGameStateEnum.enemyMoving;

            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies spawned (IE in first level):
            if (tmpObjs.Count == 0)
            {
                //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                yield return new WaitForSeconds(turnDelay);
            }

            //Loop through List of Enemy objects.
            for (int i = 0; i < tmpObjs.Count; i++)
            {
                //Call the MoveEnemy function of Enemy at index i in the enemies List.
                if (tmpObjs[i].toDELETE == false)
                    tmpObjs[i].Move("enemy");
                //Wait for Enemy's moveTime before moving next Enemy, 
                yield return new WaitForSeconds((float)tmpObjs[i].moveTime / tmpObjs.Count);
            }
            //Once Enemies are done moving, set playersTurn to true so player can move.
            //playersTurn = true;

            //Enemies are done moving, set enemiesMoving to false.
            STATA["days"]++;


            // panel.GetComponent<panels>().devInfoUpdate();


            PLAYER.GetComponent<objectsClass>().drawPossiblePlayerActionsDots();
            curGameState = curGameStateEnum.playerMove;
        }
    }
    private void CheckForEverydayOptions(List<objectsClass> tmpObjs)
    {
        for (int i = 0; i < tmpObjs.Count; i++)
        {
            //УДАЛИТЬ ЕСЛИ НУЖНО
            if (tmpObjs[i].toDELETE)
            {
                tmpObjs[i].destroyMe();
            }
            else
            tmpObjs[i].setDayNight();


        }

        // СОХРАНИТЬ ПЛЕНКИ И ЛЕНС

        //SAVE PLAYER CAMERAS AND LENS
        for (int i = 0; i < playerItem.Count; i++)
        {
            if (playerItem[i] == currentCamera)
            {
                PlayerPrefs.SetString("currentCamera", playerItem[i].name); //SAVEDATA
            }
            if (playerItem[i] == currentLens)
            {
                PlayerPrefs.SetString("currentLens", playerItem[i].name); //SAVEDATA
            }
        }
    }
    private void StartNonWalkObjectsUpdateSatet()
    {
        for (int i = 0; i < objectsNotWalk.Count; i++)
        {
            objectsNotWalk[i].MoveStatic();
        }
    }

    public void checkForItemCollect(Vector2 pos)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (pos== items[i].curPos)
            {
                if (items[i].name=="Film")
                {
                    items[i].collectItem();
                    setNewFilms(1);
                }   
                break;
            }
        }
    }
    private void changePlayerLens(itemStore whatTobuy)
    {
        currentLens = whatTobuy;
        PLAYER = GameObject.Find("Player");
        PLAYER.GetComponent<objectsClass>().changePlayerCameraType();
        soundManager.instance.PlaySingle("getFilm");
        PlayerPrefs.SetInt(whatTobuy.name + "_have",1);
    }
    private void changePlayerCamera(itemStore whatTobuy)
    {
        soundManager.instance.PlaySingle("getFilm");
        currentCamera = whatTobuy;
        PlayerPrefs.SetInt(whatTobuy.name + "_have",1);
    }

    //  ------------- ПОКУПКА  BUY ---------------

    public void buySomething(itemStore whatTobuy)
    {
        if (whatTobuy.TYPE == "lens" && whatTobuy.playerHave==1) // ЕСЛИ ИГРОК ИМЕЕТ ОБЪЕКТИВ то ПОМЕНЯТЬ
        {
            if(currentLens== whatTobuy)
            { }
            else
            changePlayerLens(whatTobuy);
        }
        else if (playerMoney >=  whatTobuy.COST)
        {
            if (whatTobuy.TYPE == "Film")
            {
                setNewFilms(1);
                setNewScore(-whatTobuy.COST);
            }
            else if(whatTobuy.TYPE == "lens"){

                if (currentCamera.name != "phoneF")
                {
                        changePlayerLens(whatTobuy);
                        whatTobuy.playerHave = 1;
                        setNewScore(-whatTobuy.COST);
                }
                else
                {
                    //TODO: Сделать систему текстов ошибок!!
                    soundManager.instance.PlaySingle("noMoney");
                }
            }
            else if (whatTobuy.TYPE == "camera" && whatTobuy.playerHave == 0)
            {
                changePlayerCamera(whatTobuy);
                whatTobuy.playerHave = 1;
                setNewScore(-whatTobuy.COST);
            }
            else if(whatTobuy.TYPE == "flash")
            {
                //changePlayerFlash(whatTobuy);
                //whatTobuy.playerHave = 1;
                setNewFlashs(10);
                setNewScore(-whatTobuy.COST);
            }
            else if (whatTobuy.TYPE == "Money")
            {
                CompleteProject.IAPManager.instance.Buy4000money();
            }

        }
        else
        {
            soundManager.instance.PlaySingle("noMoney");
        }
    }
    public void setNewScore(int score, bool add = true)
    {
        if(add) playerMoney += score;
        else playerMoney = score;
        txt.text = "$ " + playerMoney;
        PlayerPrefs.SetInt("Money", playerMoney); //SAVEDATA 
    }

    public void setNewPenalty(int penalty, bool add = true)
    {
        if (add) playerPenalty += penalty;
        else playerPenalty = penalty;

        if (playerPenalty>=3)
        {
            playerPenalty = 0;
            //TODO: забрать камеру

        }
        //txtPenalty.text = playerPenalty+"/3";
        PlayerPrefs.SetInt("Penalty", playerPenalty); //SAVEDATA 
    }

    public void setNewFilms(int films,bool add=true)
    {
        if (add) currentFilms.playerHave += films;
        else currentFilms.playerHave = films;

        txtFilm.text = "" + currentFilms.playerHave;
        PlayerPrefs.SetInt("Films", currentFilms.playerHave);//SAVEDATA
        if (films>0 && GAME_STATE==game_state.continueGame)
        {
            soundManager.instance.PlaySingle("getFilm");
        }
    }
    public void setNewFlashs(int flashs, bool add = true)
    {
        if(add)currentFlash.playerHave += flashs;
        else currentFilms.playerHave = flashs;

        txtFlashs.text = "" + currentFlash.playerHave;
        PlayerPrefs.SetInt("Flashs", currentFlash.playerHave);//SAVEDATA
        if (flashs > 0 && GAME_STATE == game_state.continueGame)
        {
            soundManager.instance.PlaySingle("getFilm");
        }
    }

   /* void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        //level++;

        //Call InitGame to initialize our level.
        InitGame();
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        //SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled. 
        //Remember to always have an unsubscription for every delegate you subscribe to!
        //SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }*/
    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        // levelText.text = "After " + level + " days, you starved.";
        levelText.text = "Photo trip is over :(";
        //Enable black background image gameObject.
        Image image = levelImg.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
        levelImg.SetActive(true);
        // image.CrossFadeAlpha(1f, 1, true);
        gameManager.doingSetup = true;
         //Disable this GameManager.
         enabled = false;
    }

    //Restart reloads the scene when called.
    public void Restart()
    {
        //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
        Invoke("RestartDelay", restartLevelDelay);

    }
    private void RestartDelay()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        print("restart");
        panel.GetComponent<panels>().hideopenRestartBtn(false);
        PlayerPrefs.SetInt("Money", 25); //SAVEDATA
        PlayerPrefs.SetInt("Films", 3); //SAVEDATA
        PlayerPrefs.SetInt("Flashs", 3); //SAVEDATA
        SceneManager.LoadScene("1");
    }

    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (playerMoney < 0)
        {
            print("gameover");
            panel.GetComponent<panels>().hideopenRestartBtn(true);
            //Call the GameOver function of GameManager.
            GameOver();

        }
    }


}
