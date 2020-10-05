using System.Collections.Generic;
using UnityEngine;

public class itemStore  {

    public string name;
    public int COST;
    public string NAME;
    public string IMAGENAME;
    public string DESCRIPTION;
    public int playerUse;
    public int playerHave;
    public string TYPE;
    public float SPECIAL;
    public List<Vector2> POSSIBLE_DOTS;

    public itemStore(string Name1,string Name2, string img, string descr, int plrUse, int plrHave, string typeOf, float spec=0, List<Vector2> possibleVecs=null)
    {
        name = Name1;
        NAME = Name2;
        IMAGENAME = img;
        DESCRIPTION = descr;
        playerUse = plrUse;
        playerHave = plrHave;
        COST = gameManager.COST[Name1];
        TYPE = typeOf;
        SPECIAL = spec;
        POSSIBLE_DOTS = possibleVecs;
    }
}
