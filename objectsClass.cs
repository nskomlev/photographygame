using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectsClass : MonoBehaviour {
    public Vector2 curPos;//map
    private Vector3 nextPos;//map
    public int ID;
    public float speed = 4.0f;
    public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    private float inverseMoveTime;          //Used to make movement more efficient.
    private int AIstopResponseTimes = 0;
    public enum curent_state_enum { collected, free, canbeshot, cantbeshot, instore, ingallery };
    public curent_state_enum CURRENT_STATE;
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
    private enum object_type_element { SEA, GROUND, FLY };
    private object_type_element OBJECT_TYPE;

    private Animator animator;
    public int animationFrames; //ИЗ ПРЕФАБА
    private float objectlivedays = 0;
    public int movePriotity;
    public int RandomInt;

    private int daysFromLastAction=0; //Количество повтороений хода

    private int tmpnumOfRepeatSteps=0; // отсчет дней с момента первог ыповторения
    public bool findNewPath=false; // отсчет дней с момента первог ыповторения
    private List<Vector2> laststeps; // массив с споследними координатами объекта

    public objectsClass GO_TO_RAGET;
    public int COLLECT_TO_ACTION = 0;
    public bool GOT_THE_TARGET = false;


    public bool toDELETE = false;

    GameObject checkMove;
    GameObject panel;

    GameObject Night = null;
    GameObject Day = null;


    public Transform tr;
    public int COST;

    Dictionary<string, Vector2> moveTypes = new Dictionary<string, Vector2>()
    {
        {"up",new Vector2(-1,0)},
        {"down",new Vector2(1,0)},
        {"left",new Vector2(0,-1)},
        {"right",new Vector2(0,1)},
        {"none",new Vector2(0,0)}
    };

    Dictionary<string, Vector2> moveTypesReal = new Dictionary<string, Vector2>()
    {
        {"up",new Vector2(0,1)},
        {"down",new Vector2(0,-1)},
        {"left",new Vector2(-1,0)},
        {"right",new Vector2(1,0)},
        {"none",new Vector2(0,0)}
    };

    Dictionary<string, Vector2> actionVecsMap = new Dictionary<string, Vector2>()
    {
        {"up",new Vector2(-1,0)},
        {"down",new Vector2(1,0)},
        {"left",new Vector2(0,-1)},
        {"right",new Vector2(0,1)}
    };


    List<Vector2> CURRENTactionVecsMap = new List<Vector2>();

    private string objTypeEnum;

    void Start() {
        daysFromLastAction = 0;
        animator = GetComponent<Animator>();

        inverseMoveTime = 1f / moveTime;

        tr = transform;
        objTypeEnum = tr.name;
        checkMove = GameObject.Find("GameLogic");
        panel = GameObject.Find("Canvas");
        if (objTypeEnum != "exitufo" && objTypeEnum != "exitsea")
            movePriotity = gameManager.movePriotity[objTypeEnum];

        if (buildMap.allObjectsCanAppearInGameOn.Contains(objTypeEnum))
            gameManager.instance.allObjectsOnScene[objTypeEnum] = gameManager.instance.allObjectsOnScene[objTypeEnum] + 1;

        gameManager.instance.AddAllObjects(this); // ДОБАВЛЯЕМ ВСЕ ВООБЩЕ ОБЪЕКТЫ В ОДИН МАССИВ

        if (buildMap.allObjectsCanWalk.Contains(objTypeEnum))
        {
            gameManager.instance.AddEnemyToList(this);
            CURRENT_STATE = curent_state_enum.canbeshot;
        }
        if (buildMap.allObjectsCanBeDhoot.Contains(objTypeEnum))
        {
            gameManager.instance.AddObjectsToList(this);
            CURRENT_STATE = curent_state_enum.canbeshot;
        }
        if (buildMap.allObjectsNotWork.Contains(objTypeEnum))
        {
            gameManager.instance.AddObjectsNoTWalkToList(this);
        }
        if (buildMap.allObjectsCollectItems.Contains(objTypeEnum))
        {
            gameManager.instance.AddCollectItemsToList(this);
        }

        if (buildMap.allObjectsCanBeDhootNotAlways.Contains(objTypeEnum))
        {
            CURRENT_STATE = curent_state_enum.cantbeshot;
        }
        if (buildMap.allObjectsCanAppearNotWork.Contains(objTypeEnum))
        {
            animator.speed = 0.00F;
            animator.Play(objTypeEnum, 0, 0);
        }
        if (buildMap.allObjectsCanNotWalkNotDisaper.Contains(objTypeEnum))//дома
        {
            RandomInt = Mathf.RoundToInt(Random.value * 100);
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            CURRENT_STATE = curent_state_enum.cantbeshot;
        }
        if (buildMap.allObjectsCanNotWalkNotDisaper.Contains(objTypeEnum))
        {
            gameManager.instance.AddPlayerObjectsBuildings(this);
        }
        if (objTypeEnum == "Enemy1")
        {
            CURRENT_STATE = curent_state_enum.canbeshot;
        }
        if (objTypeEnum == "Player")
        {
            changePlayerCameraType();

            drawPossiblePlayerActionsDots();
            CURRENT_STATE = curent_state_enum.free;
        }


        if (objTypeEnum == "Film") { CURRENT_STATE = curent_state_enum.free; }
        if (objTypeEnum == "Girl") { GO_TO_RAGET = findTarget("Flower"); }
        if (objTypeEnum == "Ufo") { GO_TO_RAGET = findTarget("Girl"); makeAction("play-sound", 100, "ufoin", "nature"); }
        if (objTypeEnum == "Enemy1") { GO_TO_RAGET = findTarget("Player"); }
        if (objTypeEnum == "Boat") { GO_TO_RAGET = findTarget("Delf"); makeAction("play-sound", 100, "shipin", "nature"); }
        if (objTypeEnum == "Delf") { makeAction("play-sound", 100, "delfin", "nature"); }

        //ВКЛЮЧАЕМ ОТКЛЮЧАЕМ НОЧЬ
        if (this.gameObject.transform.Find("night") != null)
            Night = this.gameObject.transform.Find("night").gameObject;
        if (this.gameObject.transform.Find("day") != null)
            Day = this.gameObject.transform.Find("day").gameObject;

        setDayNight();
        laststeps = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        //УПРАЛВЕНИЕ ИГРОКОМ
        if (objTypeEnum == "Player")
        {
            if (gameManager.curGameState == gameManager.curGameStateEnum.playerMove && !gameManager.doingSetup)
            {

                int horizontal = 0;     //Used to store the horizontal move direction.
                int vertical = 0;       //Used to store the vertical move direction.


#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));
                vertical = (int)(Input.GetAxisRaw("Vertical"));

                if (Input.GetMouseButtonUp(0))
                {
                   // panels.instance.devInfoUpdate("HERE3");
                    /* gameManager.playrPressVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                     print(gameManager.playrPressVector);*/

                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                    if (hit.collider != null)
                    {

                        //Debug.Log(hit.transform.name);
                        Transform objectHit = hit.transform;
                        Action(objectHit);

                    }

                }
                else if (horizontal != 0 || vertical != 0)
                {
                    Vector2 axisPlayer = new Vector2(horizontal, vertical);
                    foreach (KeyValuePair<string, Vector2> entry in moveTypesReal)
                    {
                        if (entry.Value == axisPlayer)
                        {
                            if (horizontal > 0)
                                makeAction("right");
                            if (horizontal < 0)
                                makeAction("left");

                            Move(entry.Key);
                        }
                    }
                }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

            //Check if Input has registered more than zero touches
            if (Input.touchCount > 0 && CURRENT_STATE==curent_state_enum.free)
            {
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];
                
                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began)
                {
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }
                
                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >0 && Vector2.Distance(touchOrigin,myTouch.position)>2)
                {
                // panels.instance.devInfoUpdate("HERE1");

                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;
                    
                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;
                    
                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;
                    
                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;
                    
                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        horizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        vertical = y > 0 ? 1 : -1;


                    Vector2 axisPlayer = new Vector2(horizontal, vertical);
                    foreach (KeyValuePair<string, Vector2> entry in moveTypesReal)
                    {
                        if (entry.Value==axisPlayer)
                        {
                            if (horizontal > 0)
                                makeAction("right");
                            if(horizontal < 0)
                                makeAction("left");

                            Move(entry.Key);
                        }
                    }  
                }
                else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >0 && Vector2.Distance(touchOrigin,myTouch.position)<=4){ // если тап меньше 4 пикселей
               // panels.instance.devInfoUpdate("HERE2");
                touchOrigin.x = -1;
                   Vector3 pos = Camera.main.ScreenToWorldPoint(myTouch.position);
                    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                    if (hit.collider != null)
                    {
                        //Debug.Log(hit.transform.name);
                        Transform objectHit = hit.transform;
                        Action(objectHit);
                    }
                }
            }
/*
            //Check if Input has registered more than zero touches
            if (Input.touchCount > 0 && CURRENT_STATE==curent_state_enum.free)
            {
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];
                
                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began)
                {
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }
                
                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 1)
                {
                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;
                    
                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;
                    
                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;
                    
                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;
                    
                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        horizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        vertical = y > 0 ? 1 : -1;


                    Vector2 axisPlayer = new Vector2(horizontal, vertical);
                    foreach (KeyValuePair<string, Vector2> entry in moveTypesReal)
                    {
                        if (entry.Value==axisPlayer)
                        {
                            if (horizontal > 0)
                                makeAction("right");
                            if(horizontal < 0)
                                makeAction("left");

                            Move(entry.Key);
                        }
                    }  
                }
                else if(myTouch.phase==TouchPhase.Stationary){
                   Vector3 pos = Camera.main.ScreenToWorldPoint(myTouch.position);
                    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                    if (hit.collider != null)
                    {
                        //Debug.Log(hit.transform.name);
                        Transform objectHit = hit.transform;
                        Action(objectHit);

                    }
                }
            }
*/
#endif
                //МЕНЮ ПАУЗА 
                if (gameManager.GAME_STATE == gameManager.game_state.continueGame && Input.GetKeyDown(KeyCode.Escape))
                {
                    panel.GetComponent<panels>().startContinueGame(false); 
                }
            }
        }
    }
    private objectsClass findTarget(string typeOfTarget) // Посик ДЛЯ СТАТИЧНЫХ ОБЪЕКТОВ
    {
        objectsClass newTarget = null;
        List<objectsClass> tmpVec = new List<objectsClass>();

        if (typeOfTarget == "exitgirl")
        {
            typeOfTarget = buildMap.allObjectsCanNotWalkNotDisaper[Random.Range(0, buildMap.allObjectsCanNotWalkNotDisaper.Count)];
        }

        foreach (objectsClass allobject in gameManager.instance.allObjects)
        {
            if (allobject.name == typeOfTarget)
            {
                tmpVec.Add(allobject);
            }
        }


        if (tmpVec.Count > 0) // Если найден хоть один объект выбираем случаный
            newTarget = tmpVec[Random.Range(0, tmpVec.Count)];

        return newTarget;
    }
    private string wayToTarget(objectsClass typeOfTarget) // расчет пути
    {
        List<string> freePos = new List<string>();
        freePos = checkForMove(curPos); // ПОЛУЧАЕМ СВОБОБНЫЕ КЛУТКИ ДЛЯ ХОТЬБЫ

        if (!findNewPath)
        {
            laststeps.Add(curPos);
            if (laststeps.Count >= 7)
            {
                laststeps.Clear();
            }

            Dictionary<Vector2, int> shoppingDictionary = new Dictionary<Vector2, int>();
            foreach (Vector2 item in laststeps)
            {
                if (!shoppingDictionary.ContainsKey(item))
                {
                    shoppingDictionary.Add(item, 1);
                }
                else
                {
                    int count = 0;
                    shoppingDictionary.TryGetValue(item, out count);
                    shoppingDictionary.Remove(item);
                    shoppingDictionary.Add(item, count + 1);
                }
            }
            foreach (KeyValuePair<Vector2, int> entry in shoppingDictionary)
            {
                if (entry.Value >= 3)
                {
                   shoppingDictionary.Clear();
                   laststeps.Clear();
                   findNewPath = true;
                    break;
                    
                }
            }
        }
        else
        {
            tmpnumOfRepeatSteps++;
            if (tmpnumOfRepeatSteps >= 6)
            {
                findNewPath = false;
                tmpnumOfRepeatSteps = 0;
            }
        }

        Vector2 diffBetweenPlayerAndEnemy = new Vector2(typeOfTarget.curPos.x - curPos.x, typeOfTarget.curPos.y - curPos.y);

        var iiii = 0;
        float lowestVec = 99999999990f;
        float highesVec = 0f;
        string tempStr = "";

        if (!findNewPath)
        {
            foreach (var entry in freePos)
            {
                //freePos = checkForMove(curPos);
                Vector2 temVec = moveTypes[entry] - diffBetweenPlayerAndEnemy;
                if (lowestVec > temVec.sqrMagnitude)
                {
                    lowestVec = temVec.sqrMagnitude;
                    tempStr = entry;
                }
                iiii++;

            }
        }
        else
        {
           // if (objTypeEnum=="Enemy1") print("findNewPath = true;");
            foreach (var entry in freePos)
            {
                //freePos = checkForMove(curPos);
                Vector2 temVec = moveTypes[entry] - diffBetweenPlayerAndEnemy;
                if (highesVec < temVec.sqrMagnitude)
                {
                    highesVec = temVec.sqrMagnitude;
                    tempStr = entry;
                }
                iiii++;

            }
        }
        if (tmpnumOfRepeatSteps>=3)
        {
            tempStr = freePos[Random.Range(0, freePos.Count)];
        }
        return tempStr;
    }

    public void changePlayerCameraType()
    {
        // CURRENTactionVecsMap = gameManager.playerItem[gameManager.instance.currentLens.name].POSSIBLE_DOTS;

        itemStore enemyInList = gameManager.playerItem.Find(x => x.name == gameManager.instance.currentLens.name);
        CURRENTactionVecsMap = enemyInList.POSSIBLE_DOTS;
        /*
        switch (gameManager.instance.currentLens.name)
        {
            case "m24":
                CURRENTactionVecsMap = actionVecsMap24mm;
                break;
            case "m50":
                CURRENTactionVecsMap = actionVecsMap50mm;
                break;
            case "m135":
                CURRENTactionVecsMap = actionVecsMap135mm;
                break;
            case "m70200":
                CURRENTactionVecsMap = actionVecsMap70200mm;
                break;
            case "m815":
                CURRENTactionVecsMap = actionVecsMap815mm;
                break;
            case "m24105":
                CURRENTactionVecsMap = actionVecsMap24105mm;
                break;
            case "nolens":
                CURRENTactionVecsMap = null;
                break;
        }*/
        drawPossiblePlayerActionsDots();
    }


    public bool Action(Transform whatTodo = null)
    {

        if (objTypeEnum == "Player")
        {
            string name = whatTodo.transform.name;
            Vector2 posOfObject = whatTodo.GetComponent<objectsClass>().getCurPos();
            if (inPlayerTrigger(posOfObject))
            {
                if (gameManager.instance.currentFilms.playerHave > 0)
                {
                    if ((gameManager.curWeather == curWeatherEnum.night && gameManager.instance.currentFlash.playerHave > 0) || (gameManager.curWeather != curWeatherEnum.night))
                    {

                        /********************************/
                        /*       ИГРОК ДЕКЙСТВИЯ        */
                        /********************************/

                        var countScore = 0;
                        var showscore = "";
                        if (buildMap.allObjectsCanBeDhoot.Contains(name) || buildMap.allObjectsCanBeDhootNotAlways.Contains(name))
                        {
                            if (whatTodo.GetComponent<objectsClass>().CURRENT_STATE == curent_state_enum.canbeshot)
                            {
                                // РАСЧЕТ ОЧКОВ ЗА ФОТОГРФАИИИ !!!
                                countScore =   (int)Mathf.Round( gameManager.COST[name] * 
                                                gameManager.instance.currentCamera.SPECIAL * 
                                                ((whatTodo.GetComponent<objectsClass>().GOT_THE_TARGET) ? (2) : (1)) *
                                                ((gameManager.curWeather == curWeatherEnum.night) ?(2):(1)) *
                                                ((gameManager.curWeather == curWeatherEnum.rain) ? (3/2) : (1))); //
                                // ЧТО ПОКАЗАТЬ ОЧКОВ ЗА ФОТОГРФАИИИ !!!
                                showscore = "" + countScore;

                                //РАСПИСАННЫЕ ОЧКИ
                                /*showscore = gameManager.COST[name] + "" + 
                                                ((gameManager.instance.currentCamera.SPECIAL == 1) ? ("") : ("x" + gameManager.instance.currentCamera.SPECIAL)) + 
                                                ((whatTodo.GetComponent<objectsClass>().GOT_THE_TARGET) ? ("x2") : (""))+
                                                ((gameManager.curWeather == curWeatherEnum.night) ? ("x2") : ("")) +
                                                ((gameManager.curWeather == curWeatherEnum.rain) ? ("x1.5") : (""));*/




                                gameManager.instance.setNewScore(countScore);
                                whatTodo.GetComponent<objectsClass>().wasHit("+" + showscore);
                                gameManager.curGameState = gameManager.curGameStateEnum.enemyWait;

                                makeAction("shoot");

                                if (gameManager.curWeather == curWeatherEnum.night && gameManager.instance.currentFlash.playerHave >0)
                                    makeAction("play-sound", 100, "flash", "nature");
                                else
                                    makeAction("play-sound", 100, "makePhoto", "nature");


                                if (whatTodo.GetComponent<objectsClass>().GOT_THE_TARGET)
                                {
                                    makeAction("play-sound", 50, "bigWin", "fx");
                                    gameManager.instance.checkCurLevel(name, true);
                                }
                                else
                                {
                                    makeAction("play-sound", 50, "smallWin", "fx");
                                    gameManager.instance.checkCurLevel(name, false);
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (name == "Enemy1" && gameManager.instance.currentFlash.playerHave > 0 && whatTodo.GetComponent<objectsClass>().CURRENT_STATE==curent_state_enum.canbeshot)
                        {
                            makeAction("shoot",0 ,"enemy1");
                            makeAction("play-sound", 100, "flash", "nature");
                            whatTodo.GetComponent<objectsClass>().AIstopResponseTimes = 12;
                            whatTodo.gameObject.transform.GetComponent<SpriteRenderer>().color = Color.red;
                        }
                        else { return false; }
                    }
                    else
                    {
                        makeAction("play-sound", 100, "emptyPhoto", "nature");
                        return false;
                    }
                }
                else
                {
                    makeAction("play-sound", 100, "emptyPhoto", "nature");
                }
            }
            else
            {
                return false;
            }
        }
        else if (objTypeEnum == "Enemy1")
        {

            List<string> possibleActions = new List<string>(); //возможные действия
            possibleActions = checkForAction();

            if (possibleActions.Count > 0)
            {
                objectsClass PLAYER = GameObject.Find("Player").GetComponent<objectsClass>();
                if (possibleActions.Contains("Player") && PLAYER.CURRENT_STATE != curent_state_enum.instore && PLAYER.CURRENT_STATE != curent_state_enum.ingallery)
                {
                    // print("hitplayer");
                    int countscore = (int)(Mathf.Round(gameManager.COST[name] * 0.5F *(gameManager.LEVEL+1)));// * gameManager.instance.currentCamera.SPECIAL; // Сколько берет мент штрафа в зависмость от типа камеры?
                    makeAction("play-sound", 100, "policeMoney", "nature");
                    AIstopResponseTimes = 4;
                    this.gameObject.transform.GetComponent<SpriteRenderer>().color = Color.red;

                    gameManager.instance.setNewScore(countscore);

                    PLAYER.wasHit("" + countscore);


                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            // List<string> possibleActions = new List<string>(); //возможные действия
            // possibleActions = checkForAction();
        }

        return true;
    }
    public void clearPole()
    {
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("playerTriggers");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    public bool drawPossiblePlayerActionsDots() {
        clearPole();
        foreach (var checkPoint in CURRENTactionVecsMap)
        {

            if (checkMove.GetComponent<buildMap>().inBounds(curPos + checkPoint))  // в границах
                                                                                   // if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGround(curPos + checkPoint))) // не мещает природа
            {
                Vector2 tempVec = curPos + checkPoint;
                GameObject prefab2 = Resources.Load("playerTrigger") as GameObject;
                GameObject tile2 = Instantiate(prefab2, Vector3.zero, Quaternion.identity) as GameObject;
                Vector2 fromMapToGlobVec = new Vector2();
                fromMapToGlobVec = checkMove.GetComponent<buildMap>().mapToReal(checkPoint);
                tile2.transform.position = new Vector3(tr.transform.position.x + fromMapToGlobVec.x, tr.transform.position.y + fromMapToGlobVec.y, -2);
                triggers thisObj = tile2.GetComponent<triggers>();
                thisObj.curPos = new Vector2(tempVec.x, tempVec.y);
            }

        }

        return true;
    }

    private bool inPlayerTrigger(Vector2 vecToCheck) // ПРОВЕРКА ДЛЯ ИГРОКА
    {
        foreach (var entry in CURRENTactionVecsMap)
        {
            if (vecToCheck == new Vector2(entry.x + curPos.x, entry.y + curPos.y))
            {
                return true;
            }
        }
        return false;
    }
    public bool Move(string whereTo)
    {
        // ПРОВЕРКА КУДА МОЖНО ПОЙТИ
        List<string> freePos = new List<string>(); //свободные пути
        freePos = checkForMove(curPos); //Получаем свободные соседние клиетки для песронажа

        if (AIstopResponseTimes > 0)
        {
            AIstopResponseTimes--;
            if (objTypeEnum=="Enemy1") CURRENT_STATE = curent_state_enum.cantbeshot;
        }
        if (AIstopResponseTimes == 0)
        {
            this.gameObject.transform.GetComponent<SpriteRenderer>().color = Color.white;
            GOT_THE_TARGET = false;
            if(objTypeEnum == "Enemy1") CURRENT_STATE = curent_state_enum.canbeshot;
        }
        //Проверем не должен ли объект 

        if (freePos.Count > 0 && AIstopResponseTimes == 0)
        {
            Vector2 WhereToMoveMapVec = new Vector2();
            Vector2 WhereToMoveLifeVec = new Vector2();

            if (objTypeEnum == "Ufo")
            {
                /* if (COLLECT_TO_ACTION >= gameManager.needToCollectForNextAction[objTypeEnum])*/


                if (GO_TO_RAGET == null)
                {
                    string randPos = freePos[Random.Range(0, freePos.Count)];

                    WhereToMoveLifeVec = moveTypesReal[randPos] * 2;
                    WhereToMoveMapVec = moveTypes[randPos] * 2;

                    if (COLLECT_TO_ACTION >= gameManager.needToCollectForNextAction[objTypeEnum])
                    { GO_TO_RAGET = findTarget("exitufo"); }
                    else
                    { GO_TO_RAGET = findTarget("Girl"); }
                }
                else
                {
                    string goToSting = wayToTarget(GO_TO_RAGET);

                    WhereToMoveLifeVec = moveTypesReal[goToSting] * 2;
                    WhereToMoveMapVec = moveTypes[goToSting] * 2;
                }

            }
            else if (objTypeEnum == "Boat")
            {
                if (GO_TO_RAGET == null)
                {
                    string randPos = freePos[Random.Range(0, freePos.Count)];

                    WhereToMoveLifeVec = moveTypesReal[randPos];
                    WhereToMoveMapVec = moveTypes[randPos];

                    if (COLLECT_TO_ACTION >= gameManager.needToCollectForNextAction[objTypeEnum])
                    { GO_TO_RAGET = findTarget("exitsea"); }
                    else
                    { GO_TO_RAGET = findTarget("Delf"); }
                }
                else
                {

                    string goToSting = wayToTarget(GO_TO_RAGET);

                    WhereToMoveLifeVec = moveTypesReal[goToSting];
                    WhereToMoveMapVec = moveTypes[goToSting];
                }
            }
            else if (objTypeEnum == "Girl")
            {
                makeAction("flowerOFF");
                if (Random.Range(0, 8) != 4 || toDELETE != true) //СЛУЧАЙНЫЕ ОСТАНОВКИ
                {


                    if (GO_TO_RAGET == null)
                    {
                        string randPos = freePos[Random.Range(0, freePos.Count)];

                        WhereToMoveLifeVec = moveTypesReal[randPos];
                        WhereToMoveMapVec = moveTypes[randPos];

                        if (COLLECT_TO_ACTION >= gameManager.needToCollectForNextAction[objTypeEnum])
                        { GO_TO_RAGET = findTarget("exitgirl"); }
                        else
                        { GO_TO_RAGET = findTarget("Flower"); }


                        //GO_TO_RAGET = findTarget(buildMap.allObjectsCanNotWalkNotDisaper[Random.Range(0, buildMap.allObjectsCanNotWalkNotDisaper.Count)]);
                    }
                    else
                    {

                        string goToSting = wayToTarget(GO_TO_RAGET);

                        WhereToMoveLifeVec = moveTypesReal[goToSting];
                        WhereToMoveMapVec = moveTypes[goToSting];
                    }


                }
                else
                    return false;


            }
            else if (objTypeEnum == "Enemy1")
            {
                //ЛОГИКА КУДА ИДТИ ИЛИ ЧТО ДЕЛАТЬ

                if (Action()) // МОЖЕМ ЛИ ПОЙМАТЬ ФОТОГРАФА
                {
                    return false;
                }
                else if(gameManager.curWeather == curWeatherEnum.rain) // Есди дождь то полицейский не ходит
                {
                    return false;
                }
                else // ИДЕМ
                {
                    if (Random.Range(0, 8) != 4) //СЛУЧАЙНЫЕ ОСТАНОВКИ
                    {

                        if (GO_TO_RAGET == null)
                        {
                            string randPos = freePos[Random.Range(0, freePos.Count)];

                            WhereToMoveLifeVec = moveTypesReal[randPos];
                            WhereToMoveMapVec = moveTypes[randPos];
                            GO_TO_RAGET = findTarget("Player");
                        }
                        else
                        {
                            string goToSting = wayToTarget(GO_TO_RAGET);

                            WhereToMoveLifeVec = moveTypesReal[goToSting];
                            WhereToMoveMapVec = moveTypes[goToSting];
                        }
                    }
                    else
                        return false;
                }
            }
            else if (objTypeEnum == "Player")
            {

                if (freePos.Contains(whereTo))
                {
                    WhereToMoveLifeVec = moveTypesReal[whereTo];
                    WhereToMoveMapVec = moveTypes[whereTo];
                    gameManager.curGameState = gameManager.curGameStateEnum.enemyWait;
                    gameManager.turnsNumber++; // добавляем + 1 к ходу игры
                    gameManager.timeFromLastShoot++; // добавляем + 1 к ходу игры с послденей съемки
                    gameManager.timeFromLastCollect++; // добавляем + 1 к ходу игры с последнеего подбора
                    gameManager.canAddNeSet = true;

                    makeAction("play-sound", 100, "step", "nature");
                }
                else
                {
                    gameManager.curGameState = gameManager.curGameStateEnum.playerMove;
                    return false;
                }
            }


            //ИДЕМ
            nextPos = transform.position + new Vector3(WhereToMoveLifeVec.x, WhereToMoveLifeVec.y, 0); // СЛЕДУШАЯ ИТОГОВАЯ ПОЗИЦИЯ В ЖИЗНИ 
            checkMove.GetComponent<buildMap>().changPlayerPosOnMap(curPos, curPos + WhereToMoveMapVec, objTypeEnum); // ИЗМЕНЯЕМ ПОЗИЦИЮ НА КАРТЕ
            curPos += WhereToMoveMapVec;// ИЗМЕНЯЕМ ТЕКУЩУЮ ПОЗИЦИЮ ИГРОКА КАРТЫ НА СЛЕДУЮЩУЮ

            if (WhereToMoveMapVec != new Vector2(0, 0) || toDELETE == false)
                StartCoroutine(SmoothMovement(nextPos));

            if (objTypeEnum == "Enemy1") { Action(); }
            if (objTypeEnum == "Player")// ПРОВЕРЯЕМ СЪЕЛ ЛИ ОБЪЕКТ ИЛИ МАГАИЗН
            {
                gameManager.instance.checkForItemCollect(curPos);

                objectsClass nearBuildings = checkIfSmthIsNearOneStep(curPos, new string[] { "Building1", "Building2", "Building3" },2);

                if (nearBuildings != null)
                {
                    if (nearBuildings.CURRENT_STATE == curent_state_enum.canbeshot)
                    {
                        makeAction("play-sound", 100, "fire", "fireChanel");
                    }
                    else
                    {
                        makeAction("play-sound", 0, "fire", "fireChanel");
                    }
                }
                else
                {
                    makeAction("play-sound", 0, "fire", "fireChanel");
                }

                if (curPos.y <= 8)
                    makeAction("play-sound", (8 / curPos.y - 1) * 100, "sea", "seaChanel");

                if (curPos.y >= 6)
                    makeAction("play-sound", ((curPos.y - 5) / 6) * 100, "forest", "forestChanel");

                if (curPos == checkMove.GetComponent<buildMap>().getObjectPosByName("store"))
                {
                    //Посетил магазин
                    panel.GetComponent<panels>().openInGameMenu(true);
                    CURRENT_STATE = curent_state_enum.instore;
                    makeAction("play-sound", 100, "door", "nature");
                    gameManager.instance.timeShopVisit++;
                    //Показать баннер на 3тье посещенеие магазина
                    if (gameManager.instance.timeShopVisit%3==0)
                    admob.instance.pauseGameButton();
                    
                }
                else if (curPos == checkMove.GetComponent<buildMap>().getObjectPosByName("Building1")
                     || curPos == checkMove.GetComponent<buildMap>().getObjectPosByName("Building2")
                     || curPos == checkMove.GetComponent<buildMap>().getObjectPosByName("Building3"))
                {
                    panel.GetComponent<panels>().openInGameGallery(true);
                    CURRENT_STATE = curent_state_enum.ingallery;
                    makeAction("play-sound", 100, "door", "nature");
                }
                else
                {
                    panel.GetComponent<panels>().openInGameMenu(false);
                    panel.GetComponent<panels>().openInGameGallery(false);
                    CURRENT_STATE = curent_state_enum.free;
                }


            }
            if (objTypeEnum == "Girl")// ВЗАИМОДЕЙСТВИЕ С ЦЕЛЬЮ
            {
                if (GO_TO_RAGET)
                    if (GO_TO_RAGET.curPos == this.curPos)
                    {
                        //ПОЙМАЛИ ЦЕЛЬ
                        if (GO_TO_RAGET.name != "exitufo" && !buildMap.allObjectsCanNotWalkNotDisaper.Contains(GO_TO_RAGET.name))
                        {
                            GO_TO_RAGET.toDELETE = true;
                            COLLECT_TO_ACTION++;
                            AIstopResponseTimes = 2; // замерли
                            makeAction("flowerON");
                            GO_TO_RAGET.gameObject.SetActive(false);
                            GOT_THE_TARGET = true;
                            makeAction("gotthetarget");
                            return false;
                        }
                        else // НА ВЫХОД
                        {
                            toDELETE = true;
                        }
                    }
            }
            if (objTypeEnum == "Ufo")
            {
                if (GO_TO_RAGET)
                    if (GO_TO_RAGET.curPos == this.curPos)
                    {   //ПОЙМАЛИ ЦЕЛЬ
                        if (GO_TO_RAGET.name != "exitufo")
                        {
                            makeAction("play-sound", 100, "ufocatch", "nature");
                            GO_TO_RAGET.toDELETE = true;
                            COLLECT_TO_ACTION++;
                            AIstopResponseTimes = 2; // замерли
                            GOT_THE_TARGET = true;
                            makeAction("gotthetarget");
                            return false;
                        }// НА ВЫХОД
                        else
                        {
                            toDELETE = true;
                        }
                    }
            }
            if (objTypeEnum == "Boat")
            {
                if (GO_TO_RAGET)
                    if (GO_TO_RAGET.curPos == this.curPos)
                    {   //ПОЙМАЛИ ЦЕЛЬ
                        if (GO_TO_RAGET.name != "exitsea")
                        {
                            makeAction("play-sound", 100, "shipin", "nature");
                            GO_TO_RAGET.toDELETE = true;
                            COLLECT_TO_ACTION++;
                            AIstopResponseTimes = 2; // замерли
                            GOT_THE_TARGET = true;
                            makeAction("gotthetarget");
                            return false;
                        }
                        else// НА ВЫХОД
                        {
                            toDELETE = true;
                        }
                    }
            }
            //ПЕРЗАГРУЗКА НА НОВЫЙ УРОВЕНЬ
            //if (objTypeEnum == "Player" && curPos == checkMove.GetComponent<buildMap>().ExitPoint) { gameManager.instance.Restart(); enabled = false; }
            //if (objTypeEnum == "Player" && buildMap.allObjectsCollectItems.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(curPos + entry.Value))) { gameManager.instance.Restart(); enabled = false; }

            return true;
        }
        else
        {


            if (objTypeEnum == "Player")
                gameManager.curGameState = gameManager.curGameStateEnum.enemyWait;

            // if (objTypeEnum == "Girl")
            // gameManager.curGameState = gameManager.curGameStateEnum.playerMove;
            if (objTypeEnum == "Enemy1")
            {
                if (AIstopResponseTimes == 0) { Action(); }
                //  gameManager.curGameState = gameManager.curGameStateEnum.playerMove;
            }

            return false;
        }

    }
    public void MoveStatic() // ЗДЕСЬ ЛОГИКА НЕ ДВИЖУЩИЙСЯ ШТУК
    {
        int multiplaer = 1;
        objectlivedays++;
        daysFromLastAction++;

        if (objTypeEnum == "Flower" && toDELETE == false)
        {
            // СКОЛЬКО ДНЕЙ ЖИВЕТ ОБЪЕКТ
            multiplaer = 10;
            float tmp = (float)(objectlivedays / (animationFrames * multiplaer));

            if (objectlivedays < (animationFrames * multiplaer))
            {
                animator.speed = 0.00F;
                animator.Play("Flower", 0, (float)tmp);
            }
            if (objectlivedays == (animationFrames * multiplaer))
            {
                destroyMe();
            }
        }
        else if (objTypeEnum == "Delf")
        {
            // СКОЛЬКО ДНЕЙ ЖИВЕТ ОБЪЕКТ
            multiplaer = 3;
            float tmp = (float)(objectlivedays / (animationFrames * 3));

            if (objectlivedays < (animationFrames * 3))
            {
                animator.speed = 0.00F;
                animator.Play("Delf", 0, (float)tmp);
                if ((float)tmp >= 0.4F && (float)tmp < 0.6F)
                {
                    CURRENT_STATE = curent_state_enum.canbeshot;
                }
                else
                    CURRENT_STATE = curent_state_enum.cantbeshot;

            }
            if (objectlivedays == (animationFrames * 3))
            {
                destroyMe();
            }
        }
        else if (buildMap.allObjectsCanNotWalkNotDisaper.Contains(objTypeEnum)) //ВСЕ ДОМА
        {
            if ((daysFromLastAction) % (50 + RandomInt) == 0)
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                CURRENT_STATE = curent_state_enum.canbeshot;
            }
            if ((daysFromLastAction - animationFrames) % (50 + RandomInt) == 0 && CURRENT_STATE == curent_state_enum.canbeshot)
            {
                daysFromLastAction = 0;
                this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                CURRENT_STATE = curent_state_enum.cantbeshot;
            }
        }
        else if (objTypeEnum == "Film")
        {
            //print("FILM");
            if(objectlivedays >= 25) destroyMe();
        }
    }
    private objectsClass checkIfSmthIsNearOneStep(Vector2 startPos, string[] namesOfObjects,int nearCells=1)  //ВОЗВОАЩАЕТ ОБЪЕКТ КОТОРЫЙ РЯДОМ
    {
        for (var i = 0; i < namesOfObjects.Length; i++)
        {
            /*foreach (KeyValuePair<string, Vector2> entry in moveTypes)
            {
                if (checkMove.GetComponent<buildMap>().getObjectPosByName(namesOfObjects[i]) == startPos + entry.Value)
                {
                    return GameObject.Find(namesOfObjects[i]).GetComponent<objectsClass>();
                }
            }*/
            for(var ii=-nearCells;ii<= nearCells; ii++)
            {
                for (var iii = -nearCells; iii <= nearCells; iii++)
                {
                    if (checkMove.GetComponent<buildMap>().getObjectPosByName(namesOfObjects[i]) == startPos + new Vector2(ii,iii))
                    {
                        return GameObject.Find(namesOfObjects[i]).GetComponent<objectsClass>();
                    }
                }
            }

        }
        return null;
    }


    private List<string> checkForMove(Vector2 startPos)  //ПРОВЕРКА ДЛЯ ВСЕХ ВОЗМОЖНЫХ ХОДОВ
    {
        List<string> freePos = new List<string>(); //свободные пути

        foreach (KeyValuePair<string, Vector2> entry in moveTypes)
        {
            if (objTypeEnum == "Ufo")
            {

                if (checkMove.GetComponent<buildMap>().inBounds(startPos + entry.Value * 2))  // в границах  || (GO_TO_RAGET != null) ? ((GO_TO_RAGET.name == "exitufo") ? (true) : (false)) : (false)
                    if (checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value * 2) == null
                    || checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value * 2) == "Girl") // не мещает игровые объекты
                    {
                        freePos.Add(entry.Key);
                    }


            }
            else if (objTypeEnum == "Boat")
            {
                if (checkMove.GetComponent<buildMap>().inBounds(startPos + entry.Value))  // в границах  || (GO_TO_RAGET!=null)?((GO_TO_RAGET.name == "exitboat")?(true):(false)) :(false)
                    if (checkMove.GetComponent<buildMap>().sendCollisonWithBackGround(startPos + entry.Value) == "sea0") // не мещщает земля || GO_TO_RAGET.name == "exitsea"
                        if (checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value) == null
                            || checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value) == "Delf") // не мещает игровые объекты
                        {
                            freePos.Add(entry.Key);
                        }
            }
            else if (objTypeEnum == "Girl")
            {
                if (checkMove.GetComponent<buildMap>().inBounds(startPos + entry.Value))  // в границах
                    if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGround(startPos + entry.Value))) // не мещщает земля
                        if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGroundObjets(startPos + entry.Value))) // не мещают природные объеты
                            if (checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value) == null ||
                                buildMap.allObjectsCollectItems.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value))
                                || buildMap.allObjectsCanBeWalkThru.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value)))// не мещает игровые объекты
                            {
                                freePos.Add(entry.Key);
                            }
            }
            else if (objTypeEnum == "Enemy1")
            {
                if (checkMove.GetComponent<buildMap>().inBounds(startPos + entry.Value))  // в границах
                    if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGround(startPos + entry.Value))) // не мещщает земля
                        if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGroundObjets(startPos + entry.Value))) // не мещают природные объеты
                            if ((checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value) == null
                                || buildMap.allObjectsCollectItems.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value))))// не мещает игровые объекты
                            {
                                if ((checkMove.GetComponent<buildMap>().getObjectPosByName("Building1") != startPos + entry.Value) &&
                                    (checkMove.GetComponent<buildMap>().getObjectPosByName("Building2") != startPos + entry.Value) &&
                                    (checkMove.GetComponent<buildMap>().getObjectPosByName("Building3") != startPos + entry.Value)
                                )
                                    freePos.Add(entry.Key);
                            }
            }
            else
            {
                if (checkMove.GetComponent<buildMap>().inBounds(startPos + entry.Value))  // в границах
                    if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGround(startPos + entry.Value))) // не мещщает земля
                        if (!buildMap.allObjectsWalls.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithBackGroundObjets(startPos + entry.Value))) // не мещают природные объеты
                            if (checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value) == null
                                || buildMap.allObjectsCollectItems.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value))
                                || buildMap.allObjectsCanBeWalkThru.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(startPos + entry.Value)))// не мещает игровые объекты
                            {
                                freePos.Add(entry.Key);
                            }
            }
        }

        return freePos;

    }

    private List<string> checkForAction() // ПРОВЕРКА ДЛЯ ВРАГОВ
    {
        List<string> freePos = new List<string>(); //свободные пути
        foreach (KeyValuePair<string, Vector2> entry in actionVecsMap)
        {

            if (checkMove.GetComponent<buildMap>().inBounds(curPos + entry.Value))  // в границах
                if (checkMove.GetComponent<buildMap>().sendCollisonWithObjects(curPos + entry.Value) != null) // не мещает объекты
                {
                    freePos.Add(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(curPos + entry.Value));
                }
        }

        return freePos;

    }
    public void setCurPos(Vector2 myPos)
    {
        curPos = myPos;
    }
    public Vector2 getCurPos()
    {
        return curPos;
    }
    public void wasHit(string score = "")
    {
        daysFromLastAction = 0;

        if (buildMap.allObjectsCanBeDhoot.Contains(objTypeEnum))
        {
            //GetComponent<SpriteRenderer>().color = Color.red;
           // Vector3 tmpVec = Camera.main.WorldToScreenPoint(this.transform.position);
            panel.GetComponent<panels>().showScorePlus(this.transform.position, score);

            destroyMe();

        }
        else if (buildMap.allObjectsCanNotWalkNotDisaper.Contains(objTypeEnum)) //ВСЕ ДОМА
        {
            panel.GetComponent<panels>().showScorePlus(this.transform.position, score);

            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            CURRENT_STATE = curent_state_enum.cantbeshot;
            makeAction("play-sound", 0, "fire", "fireChanel");
        }
        else if (objTypeEnum == "Player")
        {
            //РУСУЕМ + ОЧКИ
            panel.GetComponent<panels>().showScorePlus(this.transform.position, score);
        }
    }
    public void wasHitByOthers()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        destroyMe();
    }
    public void setDayNight() // NIGHT НОЧЬ
    {
        if (Night != null)
        {
            if (gameManager.curWeather == curWeatherEnum.night)
                Night.SetActive(true);
            if (gameManager.curWeather == curWeatherEnum.sun)
                Night.SetActive(false);

        }

        if (Day != null)
        {
            if (gameManager.curWeather == curWeatherEnum.sun)
                Day.SetActive(true);
            if (gameManager.curWeather == curWeatherEnum.night)
                Day.SetActive(false);
        }
    }
    public void destroyMe()
    {
        gameManager.instance.RemoveEnemyToList(this, curPos);
        Destroy(gameObject);
    }
    public void collectItem()
    {
        if (objTypeEnum == "Film")
        {
            gameManager.timeFromLastCollect = 0;
            GetComponent<SpriteRenderer>().color = Color.red;
            CURRENT_STATE = curent_state_enum.collected;
            gameManager.instance.RemoveEnemyToList(this, curPos);
            GameObject imgFilm = GameObject.Find("ImgFilm");
            RectTransform rectImgFilm = imgFilm.GetComponent<RectTransform>();
            StartCoroutine(SmoothMovement(new Vector3(Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).x, Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).y, 0)));
            //transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).x, Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).y, -2), 1.5f * Time.deltaTime);
        }

        //transform.position = new Vector3(Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).x, Camera.main.ScreenToWorldPoint(rectImgFilm.transform.position).y,-2);
        //Destroy(gameObject);
    }
    public void makeAction(string actionType, float ifNeed = 0, string ifNeedStr1 = "", string inNeedStr2 = "") //PLAYERS ACTIONS
    {
        switch (actionType)
        {
            case "shoot":
                if (gameManager.curWeather == curWeatherEnum.night && gameManager.instance.currentFlash.playerHave >0)
                {
                    playerFlashLightSpriteToggle();
                    gameManager.instance.setNewFlashs(-1);
                    Invoke("playerFlashLightSpriteToggle", 0.15F);
                }
                else if (ifNeedStr1=="enemy1" && gameManager.instance.currentFlash.playerHave > 0)
                {
                    gameManager.instance.setNewFlashs(-1);
                }

                animator.SetTrigger("shoot");
                gameManager.timeFromLastShoot = 0;
                gameManager.instance.setNewFilms(-1);
                gameManager.STATA["shoots"]++;
                break;

            case "right": transform.localRotation = Quaternion.Euler(0, 180, 0); break;
            case "left": transform.localRotation = Quaternion.Euler(0, 0, 0); break;
            case "flowerON": animator.SetBool("flower", true); break;
            case "flowerOFF": animator.SetBool("flower", false); break;
            case "gotthetarget": this.gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 30, 255); break;
            case "play-sound": soundManager.instance.PlaySingle(ifNeedStr1, inNeedStr2, ifNeed); break;
        }
    }
    private void playerFlashLightSpriteToggle()
    {
        SpriteRenderer playerSprtRend = this.GetComponent<SpriteRenderer>();
        if (playerSprtRend.sortingLayerName == "triggers")
        {
            playerSprtRend.sortingLayerName = "objects";
            //playerSprtRend.color = new Color32(0, 255, 255, 255);
        }
        else
        {
            playerSprtRend.sortingLayerName = "triggers";
            // playerSprtRend.color = new Color32(255, 255, 255, 255);
        }
    }


    protected IEnumerator SmoothMovement(Vector3 nextPos)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - nextPos).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            var fasttime=0F;

            if (objTypeEnum == "Film")
                fasttime = inverseMoveTime*2.5f;
            else
                fasttime = inverseMoveTime;

            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(transform.position, nextPos, Time.deltaTime * fasttime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                //rb2D.MovePosition(newPostion);
                transform.position = newPostion;
            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - nextPos).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        if (objTypeEnum == "Film")
        {
            Destroy(gameObject);
        }
    }
}
