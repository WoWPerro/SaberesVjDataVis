using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class GraphBuilder : MonoBehaviour
{
    [Title("Base de Datos de Saberes")]
    [InfoBox("Arrastra aquí todos los ScriptableObjects de tus nodos.")]
    public List<NodeData> allNodesData;

    [Title("Prefabs")]
    public GameObject macroNodePrefab;
    public GameObject microNodePrefab;

    [Title("Configuración Espacial")]
    public float zOffsetForMicroNodes = 3f; // Distancia hacia atrás para los micro-nodos

    public EdgeManager edgeManager;

    [Button("Generar y Posicionar Cartografía", ButtonSizes.Large)]
    public void BuildAndOrganizeGraph()
    {
        // 1. Limpiar nodos anteriores
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Diccionario para conectar fácilmente a los hijos con sus padres
        Dictionary<NodeData, NodeController> nodosInstanciados = new Dictionary<NodeData, NodeController>();

        // --- FASE 1: INSTANCIAR TODOS ---
        foreach (NodeData data in allNodesData)
        {
            if (data == null) continue;

            // Decidir qué prefab usar basándonos en tu checkbox
            GameObject prefabAVariar = data.isMacroNode ? macroNodePrefab : microNodePrefab;
            GameObject newNodeObject = Instantiate(prefabAVariar, transform);
            
            NodeController controller = newNodeObject.GetComponent<NodeController>();
            controller.Initialize(data);
            nodosInstanciados.Add(data, controller);

            // Si es Macro-Nodo, lo posicionamos en la cuadrícula Habermas/Schön
            if (data.isMacroNode)
            {
                float posX = ObtenerCoordenadaX(data.ejeX_Schon);
                float posY = ObtenerCoordenadaY(data.ejeY_Habermas);
                newNodeObject.transform.position = new Vector3(posX, posY, 0f);
            }
        }

        // --- FASE 2: EMPARENTAR Y ORBITAR MICRO-NODOS ---
        foreach (NodeData data in allNodesData)
        {
            // Si es un Micro-Nodo y tiene un padre asignado
            if (!data.isMacroNode && data.parentMacroNode != null)
            {
                // Verificamos que el padre también haya sido arrastrado a la lista
                if (nodosInstanciados.ContainsKey(data.parentMacroNode))
                {
                    NodeController micro = nodosInstanciados[data];
                    NodeController macro = nodosInstanciados[data.parentMacroNode];

                    // Lo hacemos hijo en la jerarquía de Unity
                    micro.transform.SetParent(macro.transform);

                    // Le asignamos su órbita aleatoria
                    MicroNodeOrbit orbitScript = micro.gameObject.GetComponent<MicroNodeOrbit>();
                    if (orbitScript == null) orbitScript = micro.gameObject.AddComponent<MicroNodeOrbit>();
                    orbitScript.targetMacroNode = macro.transform;

                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    float orbitRadius = Random.Range(1.2f, 1.8f);
                    float offsetX = Mathf.Cos(angle) * orbitRadius;
                    float offsetY = Mathf.Sin(angle) * orbitRadius;
                    float zVariation = zOffsetForMicroNodes + Random.Range(-1f, 1f);
                    
                    micro.transform.localPosition = new Vector3(offsetX, offsetY, zVariation);
                }
                else
                {
                    Debug.LogWarning($"El nodo {data.nodeTitle} busca a su padre, pero el padre no está en la lista AllNodesData.");
                }
            }
        }

        if (edgeManager != null) edgeManager.DrawEdges(nodosInstanciados);
        Debug.Log("Cartografía Generada con éxito.");
    }

    // --- LÓGICA ESPACIAL (Marco Teórico) ---

    private float ObtenerCoordenadaY(HabermasInterest interes)
    {
        switch (interes)
        {
            case HabermasInterest.Tecnico: return 0f;      // Base
            case HabermasInterest.Practico: return 5f;     // Medio
            case HabermasInterest.Emancipatorio: return 10f; // Cima
            default: return 0f;
        }
    }

    private float ObtenerCoordenadaX(SchonReflection reflexion)
    {
        switch (reflexion)
        {
            case SchonReflection.ConocimientoEnAccion: return -5f; // Izquierda
            case SchonReflection.ReflexionEnAccion: return 0f;    // Centro
            case SchonReflection.ReflexionSobreAccion: return 5f;  // Derecha
            default: return 0f;
        }
    }

    // --- LÓGICA DE ÓRBITAS ---

    private void OrganizarMicroNodos(Transform macroNode)
    {
        int childCount = macroNode.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            Transform microNode = macroNode.GetChild(i);
            
            // Distribución circular inicial
            float angle = i * Mathf.PI * 2f / childCount;
            float orbitRadius = Random.Range(2.5f, 4f); 
            
            float offsetX = Mathf.Cos(angle) * orbitRadius;
            float offsetY = Mathf.Sin(angle) * orbitRadius;
            
            float zVariation = zOffsetForMicroNodes + Random.Range(-1f, 1f);
            microNode.localPosition = new Vector3(offsetX, offsetY, zVariation); 

            // Configurar el script de Órbita
            MicroNodeOrbit orbitScript = microNode.GetComponent<MicroNodeOrbit>();
            if (orbitScript == null) 
            {
                orbitScript = microNode.gameObject.AddComponent<MicroNodeOrbit>();
            }
            
            orbitScript.targetMacroNode = macroNode;
        }
    }
}