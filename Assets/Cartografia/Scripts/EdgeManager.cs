using UnityEngine;
using System.Collections.Generic;

public class EdgeManager : MonoBehaviour
{
    public GameObject linePrefab;
    public float baseLineWidth = 0.5f;

    public void DrawEdges(Dictionary<NodeData, NodeController> nodosInstanciados)
    {
        // Limpiar líneas anteriores si las hay
        foreach (Transform child in transform) { Destroy(child.gameObject); }

        foreach (var kvp in nodosInstanciados)
        {
            NodeData originData = kvp.Key;
            NodeController originNode = kvp.Value;

            foreach (var connection in originData.connections)
            {
                if (connection.targetNode == null || !nodosInstanciados.ContainsKey(connection.targetNode)) continue;

                NodeController targetNode = nodosInstanciados[connection.targetNode];

                GameObject lineObj = Instantiate(linePrefab, transform);
                LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                
                float calculatedWidth = connection.connectionWeight * baseLineWidth;
                lr.startWidth = calculatedWidth;
                lr.endWidth = calculatedWidth;

                DynamicEdge dynamicEdge = lineObj.AddComponent<DynamicEdge>();
                dynamicEdge.startNode = originNode.transform;
                dynamicEdge.endNode = targetNode.transform;

                if (connection.isSecondary)
                {
                    lineObj.SetActive(false);
                    originNode.secondaryEdges.Add(lr);
                }
            }
        }
    }
}