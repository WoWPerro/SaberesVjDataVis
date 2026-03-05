using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MicroNodeOrbit : MonoBehaviour
{
    [Header("Configuración de Órbita")]
    public Transform targetMacroNode; 
    public float baseOrbitSpeed = 15f;
    
    private float currentSpeed;
    private Vector3 orbitAxis;
    private LineRenderer gravityLine;

    void Start()
    {
        currentSpeed = baseOrbitSpeed + Random.Range(-5f, 5f);
        float tiltX = Random.Range(-0.3f, 0.3f);
        float tiltY = Random.Range(-0.3f, 0.3f);
        orbitAxis = new Vector3(tiltX, tiltY, 1f).normalized;

        // Configuramos la línea de gravedad
        gravityLine = GetComponent<LineRenderer>();
        gravityLine.positionCount = 2;
        gravityLine.startWidth = 0.05f; // Línea muy delgadita para no competir con las aristas ENA
        gravityLine.endWidth = 0.02f;
    }

    void Update()
    {
        if (targetMacroNode == null) return;
        transform.RotateAround(targetMacroNode.position, orbitAxis, currentSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        // Actualizamos la línea cada frame para que siempre conecte al hijo con su padre
        if (targetMacroNode != null && gravityLine != null)
        {
            gravityLine.SetPosition(0, transform.position);
            gravityLine.SetPosition(1, targetMacroNode.position);
        }
    }
}