using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class panels : MonoBehaviour {

    public static panels instance = null;     //Allows other scripts to call functions from SoundManager.   

    Button restart;
    Button exitmenu;
    Button exitgallery;
    Button exitcredits;
    Button exithowtoplay;

    Button exitgamebtn;
    Button creditsbtn;
    Button howtoplaybtn;
    Button continuebtn;
    Toggle soundbtn;
    Button pausebtn;

    GameObject ingamemenu;
    GameObject ingamegallery;
    GameObject mainmenu;
    GameObject credits;
    GameObject howtoplay;
    Text devText;
    public bool isSound = true;

    Transform ScrollViewLens;
    Transform ScrollViewCameras;
    Transform ScrollViewStaff;


    // SLIDER

    private RectTransform RTingamegallery;
    private RectTransform RTCenterToCompare;
    private GameObject[] pics;

    private float[] distance;
    private int imgDistance;
    private int minImgNum;


    // end SLIDER

    /*
     * + Меню магазина 3 части объективы (на выбор) / фотоапаараты (апгрейд) / пленка, вспышка, штатив
     * Перенести все сбда для панелей
     * Подсветка при изменнеии количества пленки , очков 
     * 
     * 
     * 
     */

    void Awake()
    {
        
            //Check if there is already an instance of SoundManager
            if (instance == null)
                //if not, set it to this.
                instance = this;
            //If instance already exists:
            else if (instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy(gameObject);

        ScrollViewLens      = GameObject.Find("ScrollViewLens").transform.GetChild(0).transform.GetChild(0);
        ScrollViewCameras   = GameObject.Find("ScrollViewCameras").transform.GetChild(0).transform.GetChild(0);
        ScrollViewStaff     = GameObject.Find("ScrollViewStaff").transform.GetChild(0).transform.GetChild(0);

        devText         = GameObject.Find("devtext").GetComponent<Text>();
        ingamemenu      = GameObject.Find("ingamemenu");
        ingamegallery   = GameObject.Find("ingamegallery");
        mainmenu        = GameObject.Find("mainmenu");
        credits         = GameObject.Find("credits");
        howtoplay       = GameObject.Find("howtoplay");
        //RTingamegallery= ingamegallery.GetComponent<RectTransform>();
        //RTCenterToCompare = GameObject.Find("CenterToCompare").GetComponent<RectTransform>();

        exitmenu        = GameObject.Find("exitmenu").GetComponent<Button>();
        exitgallery     = GameObject.Find("exitgallery").GetComponent<Button>();
        exitcredits     = GameObject.Find("exitcredits").GetComponent<Button>();
        exithowtoplay   = GameObject.Find("exithowtoplay").GetComponent<Button>();

        restart         = GameObject.Find("btnRestart").GetComponent<Button>();
        pausebtn        = GameObject.Find("pausebtn").GetComponent<Button>();


        exitgamebtn     =   GameObject.Find("exitgamebtn").GetComponent<Button>();
        creditsbtn      =   GameObject.Find("creditsbtn").GetComponent<Button>();
        howtoplaybtn    =   GameObject.Find("howtoplaybtn").GetComponent<Button>();
        continuebtn     =   GameObject.Find("continuebtn").GetComponent<Button>();
        soundbtn        =   GameObject.Find("soundbtn").GetComponent<Toggle>();

        pausebtn.onClick.AddListener(() => { startContinueGame(false); });

        soundbtn.onValueChanged.AddListener(delegate { mute(soundbtn); });
        exitgamebtn.onClick.AddListener(() => { closeGame(); });
        continuebtn.onClick.AddListener(() => { startContinueGame(true); });
        exitmenu.onClick.AddListener(() => { btnCloseMenu(); });
        exitgallery.onClick.AddListener(() => { btnCloseGllery(); });
        exitcredits.onClick.AddListener(() => { toggleCredits(false); });
        exithowtoplay.onClick.AddListener(() => { toggleHowtoplay(false); });
        howtoplaybtn.onClick.AddListener(() => { toggleHowtoplay(true); });

        restart.onClick.AddListener(() => { restartFun(); });
        creditsbtn.onClick.AddListener(() => { toggleCredits(true); });
        howtoplay.SetActive(false);
        credits.SetActive(false);
        ingamemenu.SetActive(false);
        ingamegallery.SetActive(false);
        restart.gameObject.SetActive(false);
         
        //updateLevelsItems();
        // SLIDER
        /*
        int imgarrlegth = pics.Length;
        distance = new float[imgarrlegth];
        imgDistance = (int)Mathf.Abs(pics[1].GetComponent<RectTransform>().anchoredPosition.x - pics[0].GetComponent<RectTransform>().anchoredPosition.x);*/
        // end SLIDER
    }




    private void mute(Toggle change)
    {
        isSound = change.isOn;
    }

    public void goToPhoto(Button btn)
    {
        print(btn.name);
    }
        public void goToKomlevs()
    {
        if (Application.systemLanguage == SystemLanguage.Russian)
        {
            Application.OpenURL("https://komlevs.ru/main/?utm=pm");
        }
        else
        {
            Application.OpenURL("https://komlevs.ru/main/?utm=pm&lang=neru");
        }
       
    }
    public void startContinueGame(bool todo)
    {
        // print("ssssss");
        if (todo)// ПРОДОЛЖИТЬ
        {
            admob.instance.playGameButton();
            mainmenu.SetActive(false);
            if (gameManager.GAME_STATE == gameManager.game_state.startGame)
            {
                gameManager.instance.InitGame();
                createIngameItems();
                updateIngameItems();
                createLevelsItems();
            }
            gameManager.GAME_STATE = gameManager.game_state.continueGame;
            soundManager.instance.mute(false, true);
        }
        else // ПАУЗА
        {
          //  admob.instance.pauseGameButton();
            //PlayerPrefs.DeleteAll();
            gameManager.GAME_STATE = gameManager.game_state.pauseGame;
            soundManager.instance.mute(false,false);
            mainmenu.SetActive(true);
        }

        //print("start|continue");
    }
    private void closeGame()
    {
        Application.Quit();
        print("exit");
    }

    public void updateLevelsItems(int classToUpdt=0)
    {
        if (classToUpdt > 0) // если просто обновить ткущий уровень
        {
            print(classToUpdt + "___________________levels[nextLEVEL].Class1Count");

            int nextLEVEL = gameManager.LEVEL+1;
            GameObject tile = ingamegallery.transform.Find("ScrollPanel/level" + (nextLEVEL)).gameObject;
            if (classToUpdt == 1) { tile.transform.Find("targets/target1").transform.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[gameManager.LEVEL].Class1Count + "/" + gameManager.levels[gameManager.LEVEL].Class1Total; }
            if (classToUpdt == 2) { tile.transform.Find("targets/target2").transform.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[gameManager.LEVEL].Class2Count + "/" + gameManager.levels[gameManager.LEVEL].Class2Total; }
            if (classToUpdt == 3) { tile.transform.Find("targets/target3").transform.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[gameManager.LEVEL].Class3Count + "/" + gameManager.levels[gameManager.LEVEL].Class3Total; }
        }
        else // новый уровень
        {   /* OLD  */
            GameObject tile = ingamegallery.transform.Find("ScrollPanel/level" + (gameManager.LEVEL)).gameObject;

            Sprite tmpPic;
            Sprite uknownPic = Resources.Load<Sprite>("Pictures/unknown");
            tmpPic = Resources.Load<Sprite>("Pictures/pic (" + (gameManager.LEVEL) + ")");
            if (tmpPic == null)
                tmpPic = uknownPic;
            tile.transform.Find("image").gameObject.GetComponent<Image>().sprite = tmpPic;
            tile.transform.Find("image").gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            tile.transform.Find("targets").gameObject.SetActive(false);
            tile.transform.Find("panelphoto").gameObject.GetComponent<Image>().color = new Color32(11, 2, 2, 255);
            /* NEW  */
            GameObject tileNew = ingamegallery.transform.Find("ScrollPanel/level" + (gameManager.LEVEL+1)).gameObject;
            tileNew.transform.Find("hidePanel").gameObject.SetActive(false);
            tileNew.transform.Find("targets").gameObject.SetActive(true);
            tileNew.transform.Find("panelphoto").gameObject.GetComponent<Image>().color = new Color32(60, 0, 69, 255);
            tileNew.transform.Find("image").gameObject.GetComponent<Image>().color = new Color32(77, 77, 77, 40);
            updateLevelsGalleryPos();
        }
    }
    private void updateLevelsGalleryPos()
    {
        RectTransform tile = ingamegallery.transform.Find("ScrollPanel").gameObject.GetComponent<RectTransform>();
        tile.localPosition = new Vector3((-1 * (gameManager.LEVEL) * 500- 250), tile.localPosition.y, tile.localPosition.z);
    }
    private void createLevelsItems()
    {

        // gameManager.levels

        GameObject tile;
        float swdvig = 20;

        Sprite uknownPic =  Resources.Load<Sprite>("Pictures/unknown");
        Sprite lockPic =    Resources.Load<Sprite>("Pictures/lock");
        Sprite rain =       Resources.Load<Sprite>("Weather/rain");
        Sprite sun =        Resources.Load<Sprite>("Weather/sun");
        Sprite night =      Resources.Load<Sprite>("Weather/night");

        Sprite girl =       Resources.Load<Sprite>("Weather/girl");
        Sprite flower =     Resources.Load<Sprite>("Weather/flower");
        Sprite building =   Resources.Load<Sprite>("Weather/building");
        Sprite boat =       Resources.Load<Sprite>("Weather/boat");
        Sprite delf =       Resources.Load<Sprite>("Weather/delf2");
        Sprite ufo =        Resources.Load<Sprite>("Weather/ufo"); 

        for (int i = 0; i < gameManager.levels.Count+1; i++)
        {

            tile = Instantiate(Resources.Load("levelPanel"), new Vector3(swdvig, -20, 0), Quaternion.identity) as GameObject;
            tile.GetComponent<RectTransform>().localPosition = new Vector3(swdvig, -20, 0);
            tile.transform.SetParent(ingamegallery.transform.GetChild(2), false);
            tile.transform.localScale = new Vector3(1F, 1F, 1F);

            swdvig += 20 + 480;
            Sprite tmpPic;

            if (gameManager.LEVEL > i) // ВСЕ ПРОЙДЕННЫЕ
            {
                tmpPic = Resources.Load<Sprite>("Pictures/pic (" + (i + 1) + ")");
                if (tmpPic == null)
                    tmpPic = uknownPic;
                tile.transform.Find("hidePanel").gameObject.SetActive(false);
            }
            else if (gameManager.LEVEL == i) // ЕСЛИ ЭТОТ УРОВЕНЬ
            {
                tmpPic = lockPic;
                tile.transform.Find("panelphoto").gameObject.GetComponent<Image>().color = new Color32(60, 0, 69, 255);
                tile.transform.Find("hidePanel").gameObject.SetActive(false);
            }
            else // ЕСЛИ ЗАКРЫТЫЙ УРОВЕНЬ
            {
                tmpPic = lockPic;
                tile.transform.Find("panelphoto").gameObject.GetComponent<Image>().color = new Color32(77, 77, 77, 255);
                tile.transform.Find("targets").gameObject.SetActive(false);
                tile.transform.Find("hidePanel").gameObject.SetActive(false);
            }


            tile.transform.Find("image").gameObject.GetComponent<Image>().sprite = tmpPic;

            
            if (gameManager.LEVEL == i)
            {
                tile.transform.Find("image").gameObject.GetComponent<Image>().color = new Color32(77, 77, 77, 40);
            }

            //COMING SOON

            if (i == gameManager.levels.Count)
            {
                tile.transform.Find("targets").gameObject.SetActive(false);
                tile.transform.Find("leveltext").gameObject.GetComponent<Text>().text = "COMING SOON";

            }
            else
            {
                tile.transform.Find("leveltext").gameObject.GetComponent<Text>().text = "LEVEL " + (i + 1);
                tile.name = "level" + gameManager.levels[i].num;


                if (gameManager.levels[i].plusScore > 0)
                    tile.transform.Find("targets/winMoney").gameObject.GetComponent<Text>().text = "+$" + gameManager.levels[i].plusScore;

                /* ----------- TAEGET 1 ----------- */
                if (gameManager.LEVEL < i + 1)
                {
                    Sprite tmpWeather = null;
                    Sprite tmpTarget = null;

                    bool Super = false;

                    // ПЕРВАЯ

                    tmpWeather = null;
                    tmpTarget = null;

                    Transform Target1 = tile.transform.Find("targets/target1");
                    Transform Target2 = tile.transform.Find("targets/target2");
                    Transform Target3 = tile.transform.Find("targets/target3");

                    Target2.gameObject.SetActive(false);
                    Target3.gameObject.SetActive(false);

                    switch (gameManager.levels[i].Class1Weather)
                    {
                        case curWeatherEnum.sun: tmpWeather = sun; break;
                        case curWeatherEnum.rain: tmpWeather = rain; break;
                        case curWeatherEnum.night: tmpWeather = night; break;
                    }

                    switch (gameManager.levels[i].Class1Name)
                    {
                        case "Girl": tmpTarget = girl; break;
                        case "Girl2": tmpTarget = girl; Super = true; break;
                        case "Flower": tmpTarget = flower; break;
                        case "Building": tmpTarget = building; break;
                        case "Boat": tmpTarget = boat; break;
                        case "Boat2": tmpTarget = boat; Super = true; break;
                        case "Delf": tmpTarget = delf; break;
                        case "Ufo": tmpTarget = ufo; break;
                        case "Ufo2": tmpTarget = ufo; Super = true; break;
                    }

                    Target1.GetChild(0).gameObject.GetComponent<Image>().sprite = tmpWeather;
                    Target1.GetChild(1).gameObject.GetComponent<Image>().sprite = tmpTarget;
                    if (Super)
                    {
                        Target1.GetChild(1).gameObject.GetComponent<Image>().color = new Color32(255, 255, 30, 255);
                        Super = false;
                    }
                    if (gameManager.LEVEL > i)
                    {
                        Target1.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class1Total + "/" + gameManager.levels[i].Class1Total; ;
                        Target1.GetChild(2).gameObject.GetComponent<Text>().color = new Color32(0, 154, 77, 255);
                    }
                    else
                    {
                        Target1.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class1Count + "/" + gameManager.levels[i].Class1Total;
                    }




                    // ВТРОАЯ
                    if (gameManager.levels[i].Class2Name != "")
                    {

                        Target2.gameObject.SetActive(true);
                        tmpWeather = null;
                        tmpTarget = null;


                        switch (gameManager.levels[i].Class2Weather)
                        {
                            case curWeatherEnum.sun: tmpWeather = sun; break;
                            case curWeatherEnum.rain: tmpWeather = rain; break;
                            case curWeatherEnum.night: tmpWeather = night; break;
                        }

                        switch (gameManager.levels[i].Class2Name)
                        {
                            case "Girl": tmpTarget = girl; break;
                            case "Girl2": tmpTarget = girl; Super = true; break;
                            case "Flower": tmpTarget = flower; break;
                            case "Building": tmpTarget = building; break;
                            case "Boat": tmpTarget = boat; break;
                            case "Boat2": tmpTarget = boat; Super = true; break;
                            case "Delf": tmpTarget = delf; break;
                            case "Ufo": tmpTarget = ufo; break;
                            case "Ufo2": tmpTarget = ufo; Super = true; break;
                        }

                        Target2.GetChild(0).gameObject.GetComponent<Image>().sprite = tmpWeather;
                        Target2.GetChild(1).gameObject.GetComponent<Image>().sprite = tmpTarget;
                        if (Super)
                        {
                            Target2.GetChild(1).gameObject.GetComponent<Image>().color = new Color32(255, 255, 30, 255);
                            Super = false;
                        }
                        if (gameManager.LEVEL > i)
                        {
                            Target2.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class2Total + "/" + gameManager.levels[i].Class2Total;
                            Target2.GetChild(2).gameObject.GetComponent<Text>().color = new Color32(0, 154, 77, 255);
                        }
                        else
                        {
                            Target2.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class2Count + "/" + gameManager.levels[i].Class2Total;
                        }
                    }
                    // ТРЕТИЙ
                    if (gameManager.levels[i].Class3Name != "")
                    {

                        Target3.gameObject.SetActive(true);
                        tmpWeather = null;
                        tmpTarget = null;

                        switch (gameManager.levels[i].Class3Weather)
                        {
                            case curWeatherEnum.sun: tmpWeather = sun; break;
                            case curWeatherEnum.rain: tmpWeather = rain; break;
                            case curWeatherEnum.night: tmpWeather = night; break;
                        }

                        switch (gameManager.levels[i].Class3Name)
                        {
                            case "Girl": tmpTarget = girl; break;
                            case "Girl2": tmpTarget = girl; Super = true; break;
                            case "Flower": tmpTarget = flower; break;
                            case "Building": tmpTarget = building; print("building"); break;
                            case "Boat": tmpTarget = boat; break;
                            case "Boat2": tmpTarget = boat; Super = true; break;
                            case "Delf": tmpTarget = delf; break;
                            case "Ufo": tmpTarget = ufo; break;
                            case "Ufo2": tmpTarget = ufo; Super = true; break;
                        }

                        Target3.GetChild(0).gameObject.GetComponent<Image>().sprite = tmpWeather;
                        Target3.GetChild(1).gameObject.GetComponent<Image>().sprite = tmpTarget;

                        if (Super)
                        {
                            Target3.GetChild(1).gameObject.GetComponent<Image>().color = new Color32(255, 255, 30, 255);
                            Super = false;
                        }
                        if (gameManager.LEVEL > i)
                        {
                            Target3.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class3Total + "/" + gameManager.levels[i].Class3Total;
                            Target3.GetChild(2).gameObject.GetComponent<Text>().color = new Color32(0, 154, 77, 255);
                        }
                        else
                        {
                            Target3.GetChild(2).gameObject.GetComponent<Text>().text = gameManager.levels[i].Class3Count + "/" + gameManager.levels[i].Class3Total;
                        }

                    }
                }
                else
                {
                    tile.transform.Find("targets").gameObject.SetActive(false);
                }
            }

        }//FOR

        RectTransform tile2 = ingamegallery.transform.Find("ScrollPanel").gameObject.GetComponent<RectTransform>();
        tile2.sizeDelta = new Vector2(500*gameManager.levels.Count+100, 500);

        updateLevelsGalleryPos();


    }

    private void createIngameItems()
    {
        print("createIngameItems");

        var tmpCamera = 0;
        var tmpLens = 0;
        var tmpOther = 0;

        List<itemStore> getList = gameManager.instance.getPlayerItems();
        for (int i = 0; i < getList.Count; i++)
        {
            //GameObject prefab = Resources.Load("menuBtn") as GameObject;

            GameObject tile;
            if (getList[i].TYPE == "lens")
            {tile = Instantiate(Resources.Load("menuBtn"), new Vector3(0, -35 - 100 * tmpLens, 0), Quaternion.identity) as GameObject;
                tile.GetComponent<RectTransform>().localPosition = new Vector3(0, -35 - 100 * tmpLens, 0); tmpLens++; tile.transform.SetParent(ScrollViewLens, false); }
            else if (getList[i].TYPE == "camera")
            {tile = Instantiate(Resources.Load("menuBtn"), new Vector3(0, -35 - 100 * tmpCamera, 0), Quaternion.identity) as GameObject;
                tile.GetComponent<RectTransform>().localPosition = new Vector3(0, -35 - 100 * tmpCamera, 0); tmpCamera++; tile.transform.SetParent(ScrollViewCameras, false); }
            else
            {tile = Instantiate(Resources.Load("menuBtn"), new Vector3(0, -35 - 100 * tmpOther, 0), Quaternion.identity) as GameObject;
                tile.GetComponent<RectTransform>().localPosition = new Vector3(0, -35 - 100 * tmpOther, 0);
                tmpOther++; tile.transform.SetParent(ScrollViewStaff, false); }

            

            //добавляем деталей
            tile.transform.Find("name").gameObject.GetComponent<Text>().text = getList[i].NAME;
            if(getList[i].TYPE=="Money")
            tile.transform.Find("cost").gameObject.GetComponent<Text>().text = "1 USD";
            else
            tile.transform.Find("cost").gameObject.GetComponent<Text>().text = getList[i].COST + "$";
            tile.transform.Find("descr").gameObject.GetComponent<Text>().text = getList[i].DESCRIPTION;
            tile.transform.Find("image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/"+ getList[i].IMAGENAME);
            tile.name = getList[i].name;


            if (getList[i].TYPE == "lens")
            {
                for (int g = 0; g < getList[i].POSSIBLE_DOTS.Count; g++)
                {
                    GameObject prefab2 = Resources.Load("DotInMenu") as GameObject;
                    GameObject tile2 = Instantiate(prefab2, new Vector3(-65 + getList[i].POSSIBLE_DOTS[g].x*4, -45 + getList[i].POSSIBLE_DOTS[g].y*4, 0), Quaternion.identity) as GameObject;
                    tile2.GetComponent<RectTransform>().localPosition = new Vector3(-65 + getList[i].POSSIBLE_DOTS[g].x * 4, -45 + getList[i].POSSIBLE_DOTS[g].y * 4, 0);
                    //tile2.transform.position = new Vector3(-65 + getList[i].POSSIBLE_DOTS[g].x, -48 + getList[i].POSSIBLE_DOTS[g].y, 0);
                    tile2.transform.SetParent(tile.transform,false);
                }
                //gameManager.playerItem[playerItem]
            }

            int tempint = i;//АБАЛДЕТЬ 

            Button tilebtn = tile.GetComponent<Button>();
            tilebtn.onClick.AddListener(() =>  btnMenuBuyBtn(getList[tempint]));
        }

        ScrollViewLens.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 104 * tmpLens);
        ScrollViewLens.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        ScrollViewCameras.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 104 * tmpCamera);
        ScrollViewCameras.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        ScrollViewStaff.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 104 * tmpOther);
        ScrollViewStaff.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }
    private void updateIngameItems()
    {
        List<itemStore> getList = gameManager.instance.getPlayerItems();

        //GameObject tile2 = ingamemenu.transform.Find("m135").gameObject;

        for (int i = 0; i < getList.Count; i++)
        {
            GameObject tile = null;

            switch (getList[i].TYPE)
            {
                case "lens": tile = ScrollViewLens.Find(getList[i].name).gameObject; break;
                case "camera": tile = ScrollViewCameras.Find(getList[i].name).gameObject; break;
                case "flash": tile = ScrollViewStaff.Find(getList[i].name).gameObject; break;
                case "Film": tile = ScrollViewStaff.Find(getList[i].name).gameObject; break;
                case "Money": tile = ScrollViewStaff.Find(getList[i].name).gameObject; break;
            }


            if (getList[i].playerHave > 0 && getList[i].name != "Film" && getList[i].name != "600SL") // ЕСЛИ ЕСТЬ У ИГРОКА НО НЕ ВСПЫШКА И НЕ ПЛЕНКА
            {
                tile.gameObject.GetComponent<Image>().color = new Color32(0, 154, 77, 255);
            }
            else if (getList[i].name == "Film" || getList[i].name == "600SL" || getList[i].playerHave == 0) // ЕСЛИ НЕТ У ИГРОКА ИЛИ ВСПЫШКА ИЛИ ПЛЕНКА
            {
                tile.gameObject.GetComponent<Image>().color = new Color32(140, 165, 159, 255);
            }

            tile.gameObject.GetComponent<Outline>().effectDistance = new Vector2(0, 0);

            if (getList[i] == gameManager.instance.currentLens || getList[i] == gameManager.instance.currentCamera) // ЕСЛИ ЭТО ТЕКЩАЯ КАМЕРА ИЛИ ОБЪЕКТИВ ТО ПОДСВЕТИТЬ
            {
                tile.gameObject.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
            }

            if (gameManager.instance.currentCamera.name=="phoneF" && getList[i].TYPE == "lens")
            {
                tile.transform.Find("hidepanel").gameObject.SetActive(true);
            }
            else
            {
                tile.transform.Find("hidepanel").gameObject.SetActive(false);
            }
            /*
           if (getList[i].playerUse == 1)
           {
               tile.gameObject.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
           }*/


        }
    }

    public void devInfoUpdate(string toshow)//string[,] tileObjects
    {
        devText.text = toshow;
        //devText.text=PlayerPrefs.
        //devText.text = "";
        // devText.text = "days: "+ gameManager.STATA["days"]+"\n"+"shoots: "+ gameManager.STATA["shoots"];
        //for
        // tileObjects
    }

    private void btnMenuBuyBtn(itemStore btnNumInList) //ПЕРЕДАЛИ КЛАСС ПРЕДМЕТА
    { 
        print(btnNumInList.name +  " was pressed");
        gameManager.instance.buySomething(btnNumInList);
        updateIngameItems();
    }

    public void hideopenRestartBtn(bool todo)
     {
         restart.gameObject.SetActive(todo);
     }
     private void restartFun()
     {
        //показать рекламу после проигрыша
        admob.instance.pauseGameButton();
        gameManager.instance.Restart();
     }
     private void btnCloseMenu()
     {
        ingamemenu.SetActive(false);
        objectsClass tmpPlayer = GameObject.Find("Player").GetComponent<objectsClass>();
        tmpPlayer.CURRENT_STATE = objectsClass.curent_state_enum.free;
     }
    private void btnCloseGllery()
    {
        ingamegallery.SetActive(false);
        objectsClass tmpPlayer = GameObject.Find("Player").GetComponent<objectsClass>();
        tmpPlayer.CURRENT_STATE = objectsClass.curent_state_enum.free;
        updateLevelsGalleryPos();


/*
            devInfoUpdate("banner load");

            devInfoUpdate("banner not load");
*/

    }

    private void toggleCredits(bool what)
    {
        credits.SetActive(what);
    }
    private void toggleHowtoplay(bool what)
    {
        howtoplay.SetActive(what);
    }

    public void openInGameMenu(bool toDo)
     {
         ingamemenu.SetActive(toDo);
     }
    public void openInGameGallery(bool toDo)
    {
        ingamegallery.SetActive(toDo);
    }
     public void showScorePlus(Vector3 place, string sum)
     {
         GameObject newGO = new GameObject("myTextGO"+Random.value*100);
         newGO.transform.SetParent(this.transform);
         newGO.transform.position=new Vector3(place.x, place.y,10);
         Text myText = newGO.AddComponent<Text>();
         myText.text = sum+"$";
         myText.font = Resources.Load<Font>("Fonts/SHPinscher-Regular");
         myText.fontSize = 32;
         myText.color = Color.yellow;
         myText.fontStyle = FontStyle.Bold;
         myText.alignment = TextAnchor.MiddleCenter;

         RectTransform rt = newGO.GetComponent<RectTransform>();
         rt.sizeDelta = new Vector2(400, 400);
         rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);//This works

        Shadow myShadow = newGO.AddComponent<Shadow>();
         myShadow.effectDistance = new Vector2( 2.5f,-2.5f);

        //myText.CrossFadeAlpha(0, 1F, true);  // ВЫЗЫВАЕТ ГЛЮК
        StartCoroutine(SmoothDesapert(myText));

         Destroy(newGO, 1f);

     }

    public void showText(Vector3 place, string text)
    {
       // print(place);

        GameObject newGO = new GameObject("myTextGO" + Random.value * 100);
        newGO.transform.SetParent(this.transform);
        //newGO.transform.position = new Vector3(place.x, place.y, 10);
        newGO.transform.localPosition = new Vector3(place.x, place.y, 10);
        Text myText = newGO.AddComponent<Text>();
        myText.text = text;
        myText.font = Resources.Load<Font>("Fonts/SHPinscher-Regular");
        myText.fontSize = 84;
        myText.color = Color.yellow;
        myText.fontStyle = FontStyle.Bold;
        myText.alignment = TextAnchor.MiddleCenter;

        RectTransform rt = newGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 400);
        rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);//This works

        Shadow myShadow = newGO.AddComponent<Shadow>();
        myShadow.effectDistance = new Vector2(2.5f, -2.5f);

        //myText.CrossFadeAlpha(0, 1F, true);  // ВЫЗЫВАЕТ ГЛЮК

        //print(place);

        StartCoroutine(SmoothDesapert(myText));
        Destroy(newGO, 1f);

    }
    protected IEnumerator SmoothDesapert(Text DispText)
     {
        float fontSize = 32;
        while (fontSize < 50)
        {
            fontSize += 1;
            DispText.fontSize = (int)fontSize;
            yield return null;
        }
     }

         /*
         void Awake()
         {
             //Check if instance already exists
             if (instance == null)

                 //if not, set instance to this
                 instance = this;

             //If instance already exists and it's not this:
             else if (instance != this)

                 //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                 Destroy(gameObject);
             DontDestroyOnLoad(gameObject);
         }*/
        }
