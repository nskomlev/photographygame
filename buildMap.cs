using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *
 *
 * TODO: 
 * море
 * пляж
 * пляж+море
 * трава
 * трава+пляж
 * дерево
 * лужа
 * камень
 * дома
 * магазин
 * дорога прямая
 * дорога поворот
 * дорога вверх
 * мост + вода
 * мост + вода + песок
 * фотограф
 * полицейский
 * модель
 * цветок
 * нло
 * лодка
 * дельфин
 *  * 
 * 
 * 
 */

public class buildMap : MonoBehaviour
    {
        public static Vector2 mapSize = new Vector2(8, 14);
        //private int totalPositions;
        private int totalPositionsFree;

        public string[,] tileMapBack = new string[(int)mapSize.x, (int)mapSize.y]; // КАРТА земли и воды
        public string[,] tileMap2 = new string[(int)mapSize.x, (int)mapSize.y]; // КАРТА природных обектов
        public string[,] tileObjects = new string[(int)mapSize.x, (int)mapSize.y];// КАРТА игровых объектоа на заемле


    public static List<string> allObjectsBackGround = new List<string>()  //ДЛЯ tileMapBack Объекты фона земля и вода
    {"grass1","sand1","sand2","sea0"};

    public static List<string> allObjectsBackGroundSea = new List<string>()  //ДЛЯ tileMapBack Объекты воды
    {"sea0"};

    public static List<string> allObjectsGameBackGroundSpecial = new List<string>()  //ДЛЯ tileMapBack Объекты на которые можно ходить
    {"road0","road1","road2","bridge0","bridge1"};

    public static List<string> allObjectsWalls = new List<string>()  //ДЛЯ tileMap2 Объекты природы которыйе мешает проходу
    {"sea1","rock1", "rock0","rock2","tree1","sea0"};

    public static List<string> allObjectsCanBeDhoot = new List<string>()  //Объекты которыйе можно снимать всегда
    {"Girl","Flower","Ufo","Delf","Boat"};

    public static List<string> allObjectsCanBeDhootNotAlways = new List<string>()  //Объекты которыйе можно снимать иногда
    {"Building1","Building2","Building3","Delf"};

    public static List<string> allObjectsCanWalk = new List<string>()  //Объекты которыйе могут ходить
    {"Girl","Enemy1","Ufo","Boat"};

    public static List<string> allObjectsCanAppearNotWork = new List<string>()  //Объекты которыйе появлеются и не могут ходит
    {"Flower","Delf"};//

    public static List<string> allObjectsNotWork = new List<string>()  //Объекты которыйе появлеются и не могут ходит
    {"Flower","Delf","Film","Building1","Building2","Building3"};//

    public static List<string> allObjectsCanAppearWalkAndCanBeShoot = new List<string>()  //Объекты которыйе мотгут ходить и которые можно гствоткать
    {"Girl","Ufo","Boat","Boat"};

    public static List<string> allObjectsCanNotWalk = new List<string>()  //Объекты которыйе не могут ходить
    {"Flower","Delf","Building1","Building2","Building3"};

    public static List<string> allObjectsCanNotWalkNotDisaper = new List<string>()  //Объекты которыйе не исчезаеут при съемке
    {"Building1","Building2","Building3"};

    public static List<string> allObjectsCollectItems = new List<string>()  //Объекты которыйе можно взять
    {"Film"};

    public static List<string> allObjectsSea = new List<string>()  //Все объекты моря
    {"Delf","Boat"};

    public static List<string> allObjectsCanAppearInGameOn = new List<string>()  //Все объекты которые могут появляться и исчезать
    {"Girl","Ufo","Boat","Film","Flower","Delf"};

    public static List<string> allObjectsCanBeWalkThru = new List<string>()  //Все объекты которые могут появляться и исчезать
    {"Film","Flower","Building1","Building2","Building3"};

    public static List<string> allObjectsEnemies = new List<string>()  //Объекты которыйе могут ходить
    {"Enemy1"};

    Dictionary<Vector2, string> buildMapSpecial = new Dictionary<Vector2, string>(); // Добавляенм на карту внешщнией вид дороги мосты ипрочее

    /* ТРИ СЛОЯ
    * Фоновой - разная трава, большая вода
    * Фоновой-обектный - таблички, камни, горы и деревья и мешающие проходу
    * Объеты действия - игрок, враги , дома, животные
    */
    Dictionary<string, int> objDic = new Dictionary<string, int>() // Количество вещей ставить на карту По умолчанию
	{
        {"Player",1},
        {"Enemy1",1},
        {"Building1",1},
        {"Building2",1},
        {"Building3",1}
    };
    
    Dictionary<string, int> mapDic2 = new Dictionary<string, int>() // что и в каких пропорциях выставлять на карту указыны в процентах Объекты ПРИРОДЫ
    {
        //в процентах! Если процент объекта больше 100 то вставить количствл поформуле 102-100 - 2 объекта
        {"sea1",7},
        {"rock0",101}, // вывеска запрет фото
        {"tree1",7},
        {"car",101}// машина
    };

    Dictionary<string, Vector2> tileObjectsMap = new Dictionary<string, Vector2>(); // что и в каких пропорциях выставлять на карту указыны в процентах Объекты ПРИРОДЫ

    GameObject player;
        public float TileSize;
        public Vector2 StartPoint;//top left corner of the map
        public Vector2 ExitPoint;//top left corner of the map
        public Vector2 mapBounds;
        public List<Vector2> freePos;
        public List<Vector2> freePosWater;
        private int iii;

        public void SetupScene(int level)
        {
            mapBounds.x = tileMap2.GetUpperBound(0);
            mapBounds.y = tileMap2.GetUpperBound(1);
            //totalPositions = ((int)mapBounds.x + 1) * ((int)mapBounds.y + 1);
            totalPositionsFree = ((int)mapBounds.x - 1) * ((int)mapBounds.y - 1);
            TileSize = 1;
            //StartPoint = new Vector2(mapSize.x-4, mapSize.y-8);
            StartPoint = new Vector2(5F, 8.5F);
            ExitPoint = new Vector2(mapBounds.x, mapBounds.y);
        /*
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 1), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 2), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 3), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 4), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 5), "road1");
        buildMapSpecial.Add(new Vector2(0, mapSize.y - 5), "road2");*/

        buildMapSpecial.Add(new Vector2(1, mapSize.y - 1), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 2), "road0");
        buildMapSpecial.Add(new Vector2(1, mapSize.y - 3), "road1");
        buildMapSpecial.Add(new Vector2(0, mapSize.y - 3), "road2");

        buildMapSpecial.Add(new Vector2(5, 4), "bridge1");
        buildMapSpecial.Add(new Vector2(5, 3), "bridge0");
        buildMapSpecial.Add(new Vector2(5, 2), "bridge0");
        buildMapSpecial.Add(new Vector2(0, mapSize.y - 2), "store");

        tileObjectsMap.Add("Player", new Vector2((int)mapBounds.x, (int)mapBounds.y));
        tileObjectsMap.Add("Enemy1", new Vector2(0, (int)mapBounds.y));



       /* setObjectsOnMapViaLevel(level);*/
            ClearAll();
            PopulateTileMap();
        }
       /* private void setObjectsOnMapViaLevel(int level)
        {
            // ВЫСТАВЛЯЕМ ОБЪЕКТЫ КАЖДЫЙ Уровень
            objDic["Player"] = 1;
            objDic["Enemy1"] = 1;
            objDic["Ufo"] = 1;
            objDic["Girl"] = (int)Mathf.Floor(level / 4);
            objDic["Flower"] = 1 + (int)Mathf.Floor(level / 2);
            objDic["Film"] = 1 + (int)Mathf.Floor(level / 2);
            // objDic["Building1"] = 2+(int)Mathf.Floor(level / 2);
        }*/
    private void ClearAll()
        {
            iii = 0;
            tileMapBack = new string[(int)mapSize.x, (int)mapSize.y];
            tileMap2 = new string[(int)mapSize.x, (int)mapSize.y];
            tileObjects = new string[(int)mapSize.x, (int)mapSize.y];
            freePos = new List<Vector2>();
    }

        public void PopulateTileMap()
        {

            // СОСТАВЛЕМ карту ПРИРОДЫ - воды - дорог - травы

            for (int i = 0; i <= mapBounds.x; i++)
            {
                for (int j = 0; j <= mapBounds.y; j++)
                {
                
                    if (buildMapSpecial.ContainsKey(new Vector2(i, j)))
                    {
                        tileMapBack[i, j] = buildMapSpecial[new Vector2(i, j)];
                    }
                    else if (j <= 3) // ВОДА СЛЕВА
                    {
                        tileMapBack[i, j] = "sea0";
                    }
                    else if (j == 4) // ПЕСОК СПРАВА
                    {
                        tileMapBack[i, j] = "sand2";
                    }
                    else if (j == 5) // ПЕСОК И ВОДА СПРАВА
                    {
                        tileMapBack[i, j] = "sand1";
                    }
                    else
                    {
                        tileMapBack[i, j] = allObjectsBackGround[0]; //d[Random.Range(0, allObjectsBackGround.Count - 1)];
                    }
                }
            }
            // РИСУЕМ карту ПРИРОДЫ
            for (int i = 0; i <= mapBounds.x; i++)
            {
                for (int j = 0; j <= mapBounds.y; j++)
                {
                    GameObject prefab = Resources.Load(tileMapBack[i, j]) as GameObject;
                    GameObject tile = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    var fromGlobalPos = new Vector2(StartPoint.x - (mapBounds.x - TileSize * j) - (TileSize / 2), StartPoint.y - (TileSize * i) - (TileSize / 2));
                    tile.transform.position = new Vector3(fromGlobalPos.x, fromGlobalPos.y, 0);
                    tile.name = tileMapBack[i, j];

                    if (tileMapBack[i, j]=="store")
                    {
                        objectsClass thisObj = tile.GetComponent<objectsClass>();
                        thisObj.ID = iii;
                        thisObj.curPos = new Vector2(i, j);
                    }

                    if (!allObjectsWalls.Contains(tileMapBack[i, j])&& !allObjectsGameBackGroundSpecial.Contains(tileMapBack[i, j]) && i != 0 && j != 4 && i != mapBounds.x && j != mapBounds.y) // Если прирорда не находиться в массиве объектов непроходимых , добавить его координаты как свободные
                    {
                        freePos.Add(new Vector2(i, j)); //СОСТАВЛЯЕМ массив свободных мест
                    }

                }
            }

            //КООРДИНАТЫ ВСЕХ МОРСКОИХ ТОЧЕК
            for (int i = 0; i <= mapBounds.x; i++)
            {
                for (int j = 0; j <= mapBounds.y; j++)
                {
                    if (tileMapBack[i, j] == "sea0")
                    {
                        freePosWater.Add(new Vector2(i, j));
                    }
                }
            }



        iii = 1;

        //ВЫСТАВИТЬ ЖИВЫЕ ОБЪЕКТЫ - добавлем объекты в места которые есть в freePos
        foreach (KeyValuePair<string, int> entry in objDic)
        {
            // do something with entry.Value or entry.Key
            for (var ii = 0; ii < entry.Value; ii++)
            {
                Vector2 freeVec = new Vector2();

                if (tileObjectsMap.ContainsKey(entry.Key)) // Выставляем живые объекты там где нам надо
                {
                    freeVec = tileObjectsMap[entry.Key];
                    freePos.Remove(freeVec);
                }
                else //остальные случайно
                {

                    //var iiii = 0;
                    if (entry.Key == "Building1" || entry.Key == "Building2" || entry.Key == "Building3")
                    {
                        var randRange = Random.Range(0, freePos.Count);
                        freeVec = freePos[randRange];

                        Vector2 tmpVex =  new Vector2(freeVec.x + 1, freeVec.y);
                        Vector2 tmpVex2 = new Vector2(freeVec.x + 2, freeVec.y);
                        Vector2 tmpVex3 = new Vector2(freeVec.x + 1, freeVec.y - 1);
                        Vector2 tmpVex4 = new Vector2(freeVec.x + 1, freeVec.y + 1);

                        freePos.Remove(tmpVex); freePos.Remove(tmpVex2); freePos.Remove(tmpVex3); freePos.Remove(tmpVex4); 
                        freePos.Remove(freeVec);
                        //iiii++;
                    }
                    else
                    {
                        var randRange = Random.Range(0, freePos.Count);
                        freeVec = freePos[randRange];
                        freePos.RemoveAt(randRange);
                    }
                    
                }

                
                var i = freeVec.x;
                var j = freeVec.y;

                GameObject prefab2 = Resources.Load(entry.Key) as GameObject;
                GameObject tile2 = Instantiate(prefab2, Vector3.zero, Quaternion.identity) as GameObject;//,GameObject.Find("Layer_objects").transform

                var fromGlobalPos = new Vector2(StartPoint.x - (mapBounds.x - TileSize * j) - (TileSize / 2), StartPoint.y - (TileSize * i) - (TileSize / 2));
                tile2.transform.position = new Vector3(fromGlobalPos.x, fromGlobalPos.y, gameManager.allObjectsLayers[entry.Key]);
                tileObjects[(int)i, (int)j] = entry.Key;
                tile2.name = entry.Key;
                objectsClass thisObj = tile2.GetComponent<objectsClass>();
                thisObj.ID = iii;
                thisObj.curPos = new Vector2(i, j);
                // tile2.SendMessage("setCurPos", new Vector2(i, j));
                iii++;
            }

        }



        //СОСТАВЛЕМ карту ОБЪЕКТОВ ПРИРОДЫ
        foreach (KeyValuePair<string, int> entry in mapDic2)
            {
                int trmpNum = 0;
                if (entry.Value < 100)
                    trmpNum = (int)Mathf.Floor(((float)entry.Value / 100) * totalPositionsFree);
                else
                    trmpNum = entry.Value - 100;

                for (var ii = 0; ii < trmpNum; ii++)
                {

                    Vector2 randomPoint = new Vector2();

                    if (freePos.Count > 0 && entry.Key != "car")
                    {
                        randomPoint = freePos[Random.Range(0, freePos.Count)];
                        tileMap2[(int)randomPoint.x, (int)randomPoint.y] = entry.Key;


                        if (allObjectsWalls.Contains(entry.Key))
                        {
                            freePos.Remove(randomPoint);
                        }

                    }

                    if (entry.Key == "car")
                    {
                        tileMap2[(int)mapBounds.x, (int)mapBounds.y] = entry.Key;
                    }

                }
            }

            // РИСУЕМ карту ОБЪЕТОВ ПРИРОДЫ

            for (int i = 0; i <= mapBounds.x; i++)
            {
                for (int j = 0; j <= mapBounds.y; j++)
                {
                    if (tileMap2[i, j] != null)
                    {
                        GameObject prefab = Resources.Load(tileMap2[i, j]) as GameObject;
                        GameObject tile = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                        var fromGlobalPos = new Vector2(StartPoint.x - (mapBounds.x - TileSize * j) - (TileSize / 2), StartPoint.y - (TileSize * i) - (TileSize / 2));
                        tile.transform.position = new Vector3(fromGlobalPos.x, fromGlobalPos.y, -1);
                        tile.name = tileMap2[i, j];
                    }
                }
            }

        }

        public Vector2 mapToReal(Vector2 mapPoint)
        {
            var fromGlobalPos = new Vector2(mapPoint.y, -mapPoint.x);
            return fromGlobalPos;
        }


        /*
        public bool changeTileArr (Vector3 move)
        {
            return SetPlayerPosInArr (new Vector2(curPlayerPos.x-move.y,curPlayerPos.y+move.x));
    }
    */


        public bool inBounds(Vector2 pos)
        {
            if (pos.x > mapBounds.x || pos.y > mapBounds.y || pos.x < 0 || pos.y < 0)
            { return false; }
            else
            { return true; }
        }

        public string sendCollisonWithObjects(Vector2 pos)
        {
            string collideWith = tileObjects[(int)pos.x, (int)pos.y];
            return collideWith;
        }
        public string sendCollisonWithBackGround(Vector2 pos)
        {
            string collideWith = tileMapBack[(int)pos.x, (int)pos.y];
            return collideWith;
        }
        public string sendCollisonWithBackGroundObjets(Vector2 pos)
        {
            string collideWith = tileMap2[(int)pos.x, (int)pos.y];
            return collideWith;
        }


        public void changPlayerPosOnMap(Vector2 posCur, Vector2 posNext, string Name)
        {
 

            if      (getObjectPosByName("Building1") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building1";
            else if (getObjectPosByName("Building2") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building2";
            else if (getObjectPosByName("Building3") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building3";
            else
            {
                tileObjects[(int)posCur.x, (int)posCur.y] = null;
            }

             tileObjects[(int)posNext.x, (int)posNext.y] = Name;

    }
        public void removeObjFromMap(Vector2 posCur,string element)
        {
            if (element == "ground")
            {
              /*  if (!allObjectsCanNotWalkNotDisaper.Contains(sendCollisonWithObjects(posCur)))// чтобы дома не исчезали почле того как в них исчезнет девушка , не добавлялись координаты дома в freevec
                    freePos.Add(posCur);*/

            if      (getObjectPosByName("Building1") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building1";
            else if (getObjectPosByName("Building2") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building2";
            else if (getObjectPosByName("Building3") == posCur) tileObjects[(int)posCur.x, (int)posCur.y] = "Building3";
            else
            {
                freePos.Add(posCur);
            }

            //print(sendCollisonWithObjects(posCur) + "---"+ element);

        }
            else if (element == "sea")
            { freePosWater.Add(posCur); }

            tileObjects[(int)posCur.x, (int)posCur.y] = null;
        }
        
        //ДОБАВЛЯЕМ КОНКРЕТНОГО ОБЪЕКТА
        public void addObjectInGame(string objName)
        {
            int randRange;
            Vector2 freeVec;
           // Vector2 wereFrom=new Vector2(0,0);

            if (objName == "Delf" || objName == "Boat") {
                randRange = Random.Range(0, freePosWater.Count);
                freeVec = freePosWater[randRange];
                freePosWater.RemoveAt(randRange);
            }
            else if (objName == "Girl")
            {
            
                List<Vector2> tmpVec  = new List<Vector2>();
                freePos.Remove(new Vector2 (mapBounds.x,mapBounds.y)); // Удалем выход для ufo
                //найти масисив домов
                foreach (objectsClass building in gameManager.instance.objectsBuildings)
                {
                
                    //проверить не занято ли они - дыижушиеся 
                    if (sendCollisonWithObjects(  new Vector2 (building.curPos.x+1, building.curPos.y)   )==null)
                    {
                        Vector2 tmpVec2 = new Vector2(building.curPos.x + 1, building.curPos.y);
                        tmpVec.Add(tmpVec2);
                      //  wereFrom = tmpVec2;
                        break;
                    }
                }
                //найти координаты выхода случайного дома
                if (tmpVec.Count > 0)
                {
                    freeVec = tmpVec[Random.Range(0, tmpVec.Count)];
                }
                else
                    return;

            }
            else if (objName == "Ufo")
            {
                GameObject objTemp = GameObject.Find("exitufo");
                freeVec = objTemp.GetComponent<objectsClass>().curPos;
            }
            else if (objName == "Boat")
            {
                GameObject objTemp = GameObject.Find("exitsea");
                freeVec = objTemp.GetComponent<objectsClass>().curPos;
            }
            else
            {
                randRange = Random.Range(0, freePos.Count);
                freeVec = freePos[randRange];
                freePos.RemoveAt(randRange);
            }

        
                var i = freeVec.x;
                var j = freeVec.y;

                if (objName == "exitsea")
                {
                    i = 0;
                    j = 0;
                }
                if (objName == "exitufo")
                {
                    i = mapBounds.x;
                    j = mapBounds.y;
                }
        

                GameObject prefab2 = Resources.Load(objName) as GameObject;
                GameObject tile2 = Instantiate(prefab2, Vector3.zero, Quaternion.identity) as GameObject;//,GameObject.Find("Layer_objects").transform
                var fromGlobalPos = new Vector2(StartPoint.x - (mapBounds.x - TileSize * j) - (TileSize / 2), StartPoint.y - (TileSize * i) - (TileSize / 2));
                tile2.transform.position = new Vector3(fromGlobalPos.x, fromGlobalPos.y, gameManager.allObjectsLayers[objName]);

        if (objName != "exitufo" && objName != "exitsea")
        {
            tileObjects[(int)i, (int)j] = objName;
        }
                tile2.name = objName;
                objectsClass thisObj = tile2.GetComponent<objectsClass>();
                thisObj.ID = iii;
                thisObj.curPos = new Vector2(i, j);


                iii++;
        }

        public Vector2 getObjectPosByName(string Name)
        {
            GameObject tempObj = GameObject.Find(Name);
            return tempObj.GetComponent<objectsClass>().curPos;
        }

    }
