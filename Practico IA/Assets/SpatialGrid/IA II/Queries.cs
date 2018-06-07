using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queries : MonoBehaviour
{
    public bool isBox;
    public float radius = 20f;
    public SpatialGrid targetGrid;
    public float width = 15f;
    public float height = 30f;
    public IEnumerable<GridEntity> selected = new List<GridEntity>();

    public IEnumerable<GridEntity> Query()
    {
        if (isBox)
        {
            var h = height * 0.5f;
            var w = width * 0.5f;
            //posicion inicial --> esquina superior izquierda de la "caja"
            //posición final --> esquina inferior derecha de la "caja"
            //como funcion para filtrar le damos una que siempre devuelve true, para que no filtre nada.
            return targetGrid.Query(
                transform.position + new Vector3(-w, 0, -h),
                transform.position + new Vector3(w, 0, h),
                x => true);
        }
        else
        {
            //creo una "caja" con las dimensiones deseadas, y luego filtro segun distancia para formar el círculo
            return targetGrid.Query(
                transform.position + new Vector3(-radius, 0, -radius),
                transform.position + new Vector3(radius, 0, radius),
                x => { //o sea X seria la posicion del GridEntity que estoy checkeando
                    var position2d = x - transform.position; // obtengo el vector direccion entre el grid entity y mi posicion, o sea seria un vector que apunta desde (0,0) hasta la posicion del GridEntity que estoy checkeando
                    position2d.y = 0; //este eje no lo usamos
                    return position2d.sqrMagnitude < radius * radius; //de mi vector normalizado obtengo la distancia, y si esa distancia es menor al radio entonces cumple.
                });
        }
    }

    void OnDrawGizmos()
    {
        if (targetGrid == null)
            return;

        //Flatten the sphere we're going to draw
        Gizmos.color = Color.cyan;
        if (isBox)
            Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, height));
        else
        {
            Gizmos.matrix *= Matrix4x4.Scale(Vector3.forward + Vector3.right);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        if (Application.isPlaying)
        {
            selected = Query();
        }
    }


    private void OnGUI()
    {
        GUI.Label( new Rect(0,0,20,20), "HOLA");
    }
}
