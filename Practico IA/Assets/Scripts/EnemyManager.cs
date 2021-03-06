﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    SpatialGrid mySpatialGrid;

    List<GridEntity> AllEntities;

    List<Obstacle> obstaculos = new List<Obstacle>();
    public List<Enemy> enemigos = new List<Enemy>();


    private void Awake()
    {
        mySpatialGrid = GetComponent<SpatialGrid>();

        AllEntities = mySpatialGrid.GetEntities().ToList();

        obstaculos = AllEntities
            .Where(x => x.GetComponent<Obstacle>() != null)
            .Select(x => x.GetComponent<Obstacle>()).ToList();

        enemigos = AllEntities
            .Where(x => x.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList();
    }

    private void Start()
    {

        //IA2 - P1(Select ToList)


        // Primero Spawneo todos los Obstaculos
        obstaculos
            .Select(x => x.GetComponent<GridEntity>())
            .RandomizePositions(mySpatialGrid.width, mySpatialGrid.height, mySpatialGrid.cellWidth, mySpatialGrid.cellHeight);

        //Les doy una forma
        obstaculos.ForEach(x =>
        {
            x.IsCroucheable = Random.Range(-1, 1) == 0;
            x.IsJumpeable = Random.Range(-1, 1) == 0;
            x.WidthHeigth = new Vector2(Random.Range(1,10), Random.Range(1, 10));
        }
        );

        //Obtengo los transforms
        var transfObstacles = obstaculos.Select(x => x.transform);

        //con esos transform como referencia spawneo los enemies, el "RandomizePositionsExcept" toma un parametro mas, que para que los enemies no spawaneen ahi
        enemigos
            .Select(x => x.GetComponent<GridEntity>())
            .RandomizePositionsExcept(mySpatialGrid.width, mySpatialGrid.height, mySpatialGrid.cellWidth, mySpatialGrid.cellHeight, transfObstacles);

    }


}


public static class someextensions
{
    //IA2-P1 (ToList, Foreach)
    public static IEnumerable<GridEntity> RandomizePositions(this IEnumerable<GridEntity> col, float width, float heigth, float cellWidth, float cellheigth)
    {
        col
            .ToList()
            .ForEach(x => x.transform.position = new Vector3(UnityEngine.Random.Range(0, width * cellWidth), 0, UnityEngine.Random.Range(0, heigth * cellheigth)));

        return col;
    }

    public static IEnumerable<GridEntity> RandomizePositionsExcept(this IEnumerable<GridEntity> col, float width, float heigth, float cellWidth, float cellheigth, IEnumerable<Transform> transf)
    {
        col
            .ToList()
            .ForEach(x =>
           {

               bool UnaCoincidencia = true;
               float posX = -1;
               float posZ = -1;

               while (UnaCoincidencia)
               {
                   UnaCoincidencia = false;

                   posX = UnityEngine.Random.Range(0, width * cellWidth);
                   posZ = UnityEngine.Random.Range(0, heigth * cellheigth);

                   transf.ToList().ForEach(t =>
                   {
                       if (
                           (posX >= t.position.x - t.localScale.x / 2 && posX <= t.position.x + t.localScale.x / 2) ||
                           (posZ >= t.position.z + t.localScale.z / 2 && posZ <= t.position.z - t.localScale.z / 2))
                       { UnaCoincidencia = true; }
                   });
               }

               x.transform.position = new Vector3(posX, 0, posZ);

           });

        return col;
    }
}
