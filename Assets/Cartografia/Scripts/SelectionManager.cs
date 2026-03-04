using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public Camera mainCamera;
    public CameraController cameraController;
    // public UIManager uiManager; // Descomentar cuando lo crees

    private NodeController hoveredNode = null;
    private NodeController selectedNode = null;

    void Update()
    {
        HandleRaycast();
    }

    void HandleRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NodeController node = hit.collider.GetComponent<NodeController>();

            if (node != null)
            {
                // Manejo del Hover
                if (hoveredNode != node)
                {
                    if (hoveredNode != null && hoveredNode != selectedNode) 
                        hoveredNode.OnHoverExit();
                    
                    hoveredNode = node;
                    if (hoveredNode != selectedNode) 
                        hoveredNode.OnHoverEnter();
                        
                    // uiManager.ShowTooltip(hoveredNode.transform.position, hoveredNode.data.title);
                }

                // Manejo del Click Izquierdo
                if (Input.GetMouseButtonDown(0))
                {
                    SelectNode(node);
                }
            }
        }
        else
        {
            // Si el mouse sale del nodo
            if (hoveredNode != null)
            {
                if (hoveredNode != selectedNode) 
                    hoveredNode.OnHoverExit();
                hoveredNode = null;
                // uiManager.HideTooltip();
            }
        }
    }

    void SelectNode(NodeController node)
    {
        if (selectedNode != null) selectedNode.OnDeselect(); // Apaga aristas secundarias anteriores
        
        selectedNode = node;
        selectedNode.OnSelect(); // Enciende sus aristas secundarias
        
        cameraController.FocusOnNode(selectedNode.transform);
        // uiManager.OpenDataPanel(selectedNode.data);
    }
}