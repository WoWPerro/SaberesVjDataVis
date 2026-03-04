using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))] // Necesario para que el Raycast lo detecte
public class NodeController : MonoBehaviour
{
    [Header("Data Reference")]
    public NodeData data; 

    [Header("Visuals")]
    public MeshRenderer meshRenderer;
    private Material originalMaterial;
    public Material highlightMaterial; 
    public Material selectedMaterial;

    [Header("Red Epistémica (ENA)")]
    // ¡Aquí está la "mochila" que nos faltaba!
    public List<LineRenderer> secondaryEdges = new List<LineRenderer>();

    void Start()
    {
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }
    }

    public void Initialize(NodeData nodeData)
    {
        data = nodeData;
        gameObject.name = "Node_" + data.nodeTitle; 
    }

    // Llamado por el SelectionManager cuando el mouse pasa por encima
    public void OnHoverEnter()
    {
        if (meshRenderer.sharedMaterial != selectedMaterial)
        {
            meshRenderer.sharedMaterial = highlightMaterial;
            // TODO: Integrar MMFeedbacks (Feel) para un pequeño pop de escala
        }
    }

    public void OnHoverExit()
    {
        if (meshRenderer.sharedMaterial != selectedMaterial)
        {
            meshRenderer.sharedMaterial = originalMaterial;
        }
    }
    
    // Llamado por el SelectionManager al hacer clic
    public void OnSelect()
    {
        meshRenderer.sharedMaterial = selectedMaterial;
        MostrarAristasSecundarias();
    }

    // Llamado por el SelectionManager al hacer clic en OTRO nodo, o al salir
    public void OnDeselect()
    {
        meshRenderer.sharedMaterial = originalMaterial;
        OcultarAristasSecundarias();
    }

        private void MostrarAristasSecundarias()
        {
            foreach (LineRenderer edge in secondaryEdges)
            {
                if (edge != null)
                {
                    edge.gameObject.SetActive(true);
                
                    // TIP DE TECHNICAL ARTIST: 
                    // Ya que tienes DOTween Pro, en el futuro podemos animar el grosor 
                    // para que la línea no aparezca de golpe, sino que "crezca" desde el nodo base.
                }
            }
        }

        private void OcultarAristasSecundarias()
        {
            foreach (LineRenderer edge in secondaryEdges)
            {
                if (edge != null)
                {
                    edge.gameObject.SetActive(false);
                }
            }
        }
}