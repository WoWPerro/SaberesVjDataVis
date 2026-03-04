using UnityEngine;
using UnityEngine.UI;

public class TemporalManager : MonoBehaviour
{
    [Tooltip("Referencia al UI Slider que controla el tiempo.")]
    public Slider timeSlider;

    // Arreglo que almacena todos los NodeControllers encontrados en la escena.
    private NodeController[] allNodes;

    void Start()
    {
        // Encontrar todos los NodeController activos en la escena.
        // Se usa FindObjectsSortMode.None para mayor eficiencia ya que no importa el orden.
        allNodes = FindObjectsByType<NodeController>(FindObjectsSortMode.None);

        if (timeSlider != null)
        {
            // Suscribir el método OnTimeScrub al evento onValueChanged del slider
            timeSlider.onValueChanged.AddListener(OnTimeScrub);

            // Inicializamos la visualización llamando al método con 0f (o el valor inicial del slider)
            OnTimeScrub(0f);
        }
        else
        {
            Debug.LogWarning("TemporalManager: No se ha asignado una referencia al timeSlider en el inspector.");
        }
    }

    /// <summary>
    /// Método llamado cada vez que el valor del Slider cambia.
    /// Actualiza la escala de todos los nodos que no sean Macro-Nodos.
    /// </summary>
    /// <param name="timeT">El nuevo valor de tiempo enviado por el Slider.</param>
    public void OnTimeScrub(float timeT)
    {
        // Validar que existan nodos en el arreglo para evitar errores
        if (allNodes == null || allNodes.Length == 0)
            return;

        // Iteramos sobre todos los nodos encontrados
        for (int i = 0; i < allNodes.Length; i++)
        {
            NodeController node = allNodes[i];

            // Verificamos que referencie a un nodo válido y que su data esté asignada
            if (node != null && node.data != null)
            {
                // Solo afecta la escala si NO es un Macro-Nodo
                if (!node.data.isMacroNode)
                {
                    // Evaluar el valor de la curva en el punto timeT
                    float scaleValue = node.data.scaleOverTime.Evaluate(timeT);

                    // Aplicar la escala uniforme calculada al transform del nodo
                    node.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                }
            }
        }
    }

    void OnDestroy()
    {
        // Buena práctica: desuscribir el evento al ser destruido el componente
        if (timeSlider != null)
        {
            timeSlider.onValueChanged.RemoveListener(OnTimeScrub);
        }
    }
}