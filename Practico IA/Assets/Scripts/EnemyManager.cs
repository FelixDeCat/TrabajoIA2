using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    SpatialGrid mySpatialGrid;

    List<GridEntity> AllEntities;

    List<Obstacle> obstaculos = new List<Obstacle>();
    List<EnemySuicider> enemigos = new List<EnemySuicider>();

    //IA2-P1 (OfType , Select , ToList)
    private void Start()
    {
        mySpatialGrid = GetComponent<SpatialGrid>();
        AllEntities = mySpatialGrid.GetEntities().ToList();

        obstaculos = AllEntities.OfType<Obstacle>().Select(x => x.GetComponent<Obstacle>()).ToList();
        enemigos = AllEntities.OfType<EnemySuicider>().Select(x => x.GetComponent<EnemySuicider>()).ToList();
    }
}
