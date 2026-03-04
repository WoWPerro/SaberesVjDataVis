using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewNodeData", menuName = "Cartografia/Node Data")]
public class NodeData : ScriptableObject
{
    [BoxGroup("Identidad del Nodo")]
    public string nodeTitle;
    
    [BoxGroup("Identidad del Nodo")]
    [TextArea(3, 5)]
    public string descripcionCartografica;

    [BoxGroup("Identidad del Nodo")]
    [TextArea(2, 4)]
    public string citaExperto;
    
    
    public bool isMacroNode = false; // Checkbox para saber si es padre o hijo
    
    [BoxGroup("Identidad del Nodo")]
    [HideIf("isMacroNode")] // Odin esconde esto si isMacroNode es true
    [InfoBox("Arrastra aquí el Macro-Nodo al que pertenece para que lo orbite.")]
    public NodeData parentMacroNode;

    [ShowIf("isMacroNode")]
    [BoxGroup("Matriz Espacial (Posicionamiento)")]
    [EnumToggleButtons]
    public HabermasInterest ejeY_Habermas; // Y: 0, 5, 10
    
    [ShowIf("isMacroNode")]
    [BoxGroup("Matriz Espacial (Posicionamiento)")]
    [EnumToggleButtons]
    public SchonReflection ejeX_Schon; // X: -5, 0, 5

    [BoxGroup("Evolución Temporal (ENA)")]
    [InfoBox("Curva de tamaño: Eje X = Tiempo (0 a 1), Eje Y = Escala del Micro-Nodo.")]
    public AnimationCurve scaleOverTime = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1f);

    [BoxGroup("Red Epistémica (Conexiones)")]
    [TableList]
    public List<NodeConnection> connections = new List<NodeConnection>();
}

[System.Serializable]
public struct NodeConnection
{
    public NodeData targetNode; // Ahora referenciamos el ScriptableObject en lugar del componente de escena
    [Range(0.1f, 1.0f)]
    public float connectionWeight; // Define el grosor de la arista
    public bool isSecondary; // Para saber si lo ocultamos hasta hacer clic
}

public enum HabermasInterest { Tecnico, Practico, Emancipatorio }
public enum SchonReflection { ConocimientoEnAccion, ReflexionEnAccion, ReflexionSobreAccion }