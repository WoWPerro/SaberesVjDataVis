using UnityEngine;

public class MicroNodeOrbit : MonoBehaviour
{
    [Header("Configuración de Órbita")]
    public Transform targetMacroNode; // El núcleo alrededor del cual orbitaremos
    public float baseOrbitSpeed = 15f;
    
    // Variables internas para el ruido orgánico
    private float currentSpeed;
    private Vector3 orbitAxis;

    void Start()
    {
        // 1. Variación de velocidad: Algunos saberes orbitan más rápido que otros
        currentSpeed = baseOrbitSpeed + Random.Range(-5f, 5f);

        // 2. Inclinación del Eje: 
        // Principalmente orbitan en Z (1f), pero les damos un ligero tilt en X y Y 
        // para crear una constelación tridimensional en lugar de un disco plano.
        float tiltX = Random.Range(-0.3f, 0.3f);
        float tiltY = Random.Range(-0.3f, 0.3f);
        orbitAxis = new Vector3(tiltX, tiltY, 1f).normalized;
    }

    void Update()
    {
        // Si no hay target, no hacemos nada para evitar errores
        if (targetMacroNode == null) return;

        // La magia de Unity: Rota este objeto alrededor de un punto, en un eje, a cierta velocidad
        transform.RotateAround(targetMacroNode.position, orbitAxis, currentSpeed * Time.deltaTime);
    }
}