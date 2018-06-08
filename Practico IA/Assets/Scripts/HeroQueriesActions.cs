using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//IA2-P2
//IA2-P1
public class HeroQueriesActions : MonoBehaviour {

    Queries myQueries;

    private void Awake()
    {
        myQueries = GetComponent<Queries>();
    }


    //IA2-P1 (Where, Select, ToList, Foreach)
    public void Button_Kill_The_Weak_Entities()
    {
        var entities = myQueries.Query();

        entities
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList()
            .Where(x => x.Life < 25)
            .ToList()
            .ForEach(x => x.Death());


    }

    public void Button_JumpKill()
    {
        var entities = myQueries.Query();

        entities
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList()
            .ToList()
            .ForEach(x => x.Death());

    }

    public void Button_Paint_The_Most_Weak()
    {
        var entities = myQueries.Query();

        entities
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList()
            .Where(x => x.Life < 25)
            .ToList()
            .ForEach(x => x.Color = Color.green);

    }

    public void Button_Scare_Painted()
    {
        var entities = FindObjectOfType<SpatialGrid>().GetEntities();

        entities
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList()
            .Where(x => x.IsGreen)
            .ToList()
            .ForEach(x => x.Scare());
    }

    public void Button_Scare_Weaker()
    {
        var entities = myQueries.Query();

        entities
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList()
            .Where(x => x.Life < 25)
            .ToList()
            .ForEach(x => x.Scare());
    }

    // IA2-P1 (Concat, OrderBy, TakeWhile)
    public void EjectWeakAndScared()
    {
        var entities = myQueries.Query();

        entities.Where(x => x.gameObject.GetComponent<Enemy>() != null)
                    .Select(x => x.GetComponent<Enemy>())
                    .Where(x => x.Life < 25)
                    .Concat(entities
                    .Where(x => x.gameObject.GetComponent<Enemy>() != null)
                    .Select(x => x.GetComponent<Enemy>())
                    .Where(x => x.myRender.material.color == Color.black))
                    .OrderBy(x => x.Life)
                    .TakeWhile(x => x.Life < 10)
                    .ToList()
                    .ForEach(x => x.Eject());
    }
}
