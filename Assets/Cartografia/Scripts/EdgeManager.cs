using UnityEngine;
using System.Collections.Generic;

public class EdgeManager : MonoBehaviour
{
    public GameObject linePrefab; // Un prefab con un LineRenderer y un material aditivo/transparente
    public float baseLineWidth = 0.5f; // Multiplicador general de grosor

    private List<LineRenderer> allSecondaryEdges = new List<LineRenderer>();

    public void DrawEdgesForNode(NodeController originNode)
    {
        foreach (var connection in originNode.data.connections)
        {
            if (connection.targetNode == null) continue;

            GameObject lineObj = Instantiate(linePrefab, transform);
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            
            lr.SetPosition(0, originNode.transform.position);
            lr.SetPosition(1, connection.targetNode.transform.position);

            // El peso de la conexión dicta el grosor de la arista (ENA)
            float calculatedWidth = connection.connectionWeight * baseLineWidth;
            lr.startWidth = calculatedWidth;
            lr.endWidth = calculatedWidth;

            if (connection.isSecondary)
            {
                lr.gameObject.SetActive(false); // Oculto por defecto
                allSecondaryEdges.Add(lr);
                // Aquí guardaríamos una referencia en el originNode para encenderla al hacer clic
                originNode.secondaryEdges.Add(lr); 
            }
        }
    }
}