using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroQueriesActions : MonoBehaviour {

    public Hero myHero;
    public Queries myQueries;


    public void Button_Kill_The_Weak_Entities()
    {
        var entities = myQueries.Query();
    }

    public void Button_Paint_The_Most_Weak()
    {
        var entities = myQueries.Query();

    }

    public void Button_Scare_Weaker()
    {
        var entities = myQueries.Query();
    }
}
