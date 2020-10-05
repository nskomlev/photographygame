using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggers : MonoBehaviour
{
    public Vector2 curPos;
   // GameObject checkMove;

    void Start()
    {
        //checkMove = GameObject.Find("GameLogic");
    }

    // Update is called once per frame
    void Update()
    {
        //checkForObjects();
    }
    public void setCurPos(Vector2 myPos)
    {
        curPos = myPos;
    }
    private void checkForObjects()
    {

       /* if (buildMap.allObjectsCanBeDhoot.Contains(checkMove.GetComponent<buildMap>().sendCollisonWithObjects(curPos)))
        {
            //TODO: найти объект и проверить можно ли его фоткать
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            //GetComponent<SpriteRenderer>().color = Color.white;
        }*/
    }
    void OnTriggerEnter2D (Collider2D collision)
    {

        if (buildMap.allObjectsCanBeDhoot.Contains(collision.gameObject.name) || buildMap.allObjectsCanBeDhootNotAlways.Contains(collision.gameObject.name) || buildMap.allObjectsEnemies.Contains(collision.gameObject.name)) //Stop moving and fight!
        {
            objectsClass tmpobj = collision.gameObject.GetComponent<objectsClass>();

            if (tmpobj.CURRENT_STATE == objectsClass.curent_state_enum.canbeshot) {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        else
        {
            //GetComponent<SpriteRenderer>().color = Color.white;
           // curEnemy = null;
            
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {

        if (buildMap.allObjectsCanBeDhoot.Contains(collision.gameObject.name) || buildMap.allObjectsCanBeDhootNotAlways.Contains(collision.gameObject.name)) //Stop moving and fight!
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 237, 0, 103);
        }
    }
    /*
    void OnMouseUp()
    {
       // print(curPos);
        
        if (curEnemy != null)
        {
            curEnemy.SendMessage("wasHit");
        }
      //  buildMap.playerIsShooting = true;
      //  print(buildMap.playerIsShooting);
    }*/


}