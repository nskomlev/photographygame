using System.Collections.Generic;
using UnityEngine;

public class levelTypes
{
    public int id;
    public string name;
    public int amount;
    public curWeatherEnum weather;
    public levelTypes(int aid, string aname, int aamount, curWeatherEnum aweather) { id = aid; name = aname; amount = aamount; weather = aweather; }
}

