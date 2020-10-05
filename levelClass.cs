using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelClass
{
    //allLevel.Add(new Level(1,"Flower",3,curWeatherEnum.sun,"Building",2,curWeatherEnum.sun,"Boat",1,curWeatherEnum.sun));

    public curWeatherEnum curWeather;
    public int num;
    public string Class1Name;
    public int Class1Count;
    public int Class1Total;
    public curWeatherEnum Class1Weather;
    public string Class2Name;
    public int Class2Count;
    public int Class2Total;
    public curWeatherEnum Class2Weather;
    public string Class3Name;
    public int Class3Count;
    public int Class3Total;
    public curWeatherEnum Class3Weather;
    public int plusScore;

    public levelClass(int levelnum,string Class1N,int Class1C, curWeatherEnum Class1W, int Class1T, string Class2N, int Class2C, curWeatherEnum Class2W, int Class2T, string Class3N, int Class3C, curWeatherEnum Class3W, int Class3T, int winMoney=0)
    {
        num= levelnum; 
        Class1Name = Class1N;
        Class1Count= Class1C;
        Class1Weather= Class1W;
        Class1Total = Class1T;

        Class2Name = Class2N;
        Class2Count= Class2C;
        Class2Weather= Class2W;
        Class2Total = Class2T;

        Class3Name = Class3N;
        Class3Count= Class3C;
        Class3Weather= Class3W;
        Class3Total = Class3T;

        plusScore = winMoney;
    }
}
