using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DynamicEdge : MonoBehaviour
{
    public Transform startNode;
    public Transform endNode;
    private LineRenderer lr;

    void Start() 
    { 
        lr = GetComponent<LineRenderer>(); 
    }

    void LateUpdate()
    {
        // Actualizamos la posición de la línea cada frame para que siga a los nodos sin importar si orbitan
        if(startNode != null && endNode != null)
        {
            lr.SetPosition(0, startNode.position);
            lr.SetPosition(1, endNode.position);
        }
    }
}
