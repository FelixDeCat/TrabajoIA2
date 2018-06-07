using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    SpatialGrid mySpatialGrid;

    List<GridEntity> AllEntities;

    private void Start()
    {
        mySpatialGrid = GetComponent<SpatialGrid>();
        AllEntities = mySpatialGrid.GetEntities().ToList();
    }
}
