using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Configuración")]
    public Transform camTransform; // La Main Camera hija
    
    [Header("Seguimiento de Nodo")]
    public Transform focusTarget; 
    public float followSpeed = 5f; // Velocidad con la que persigue al nodo

    [Header("Orbit & Pan")]
    public float orbitSpeed = 3f;
    public float panSpeed = 0.5f;
    private float yaw, pitch;

    [Header("Zoom")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 150f;
    private float currentZoom = 20f;

    [Header("Movimiento Libre (WASD)")]
    public float moveSpeed = 25f; 
    public float shiftMultiplier = 3f;

    // --- El secreto para unificar el movimiento ---
    private Vector3 targetPosition; 

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        targetPosition = transform.position; // Inicializamos donde esté
        
        if (camTransform != null)
        {
            camTransform.localPosition = new Vector3(0, 0, -currentZoom);
        }
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraTransform();
    }

    void HandleInput()
    {
        // 1. ORBIT (Clic derecho)
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * orbitSpeed;
            pitch -= Input.GetAxis("Mouse Y") * orbitSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }

        // 2. PAN (Clic central)
        if (Input.GetMouseButton(2))
        {
            BreakFocus(); // Soltamos el nodo
            float currentPanSpeed = panSpeed * 4f; 
            Vector3 panDelta = -camTransform.right * Input.GetAxis("Mouse X") * currentPanSpeed 
                               - camTransform.up * Input.GetAxis("Mouse Y") * currentPanSpeed;
            targetPosition += panDelta;
        }

        // 3. WASD (Vuelo Libre)
        float hor = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) hor += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) hor -= 1f;

        float ver = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ver += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ver -= 1f;

        float upDown = 0f;
        if (Input.GetKey(KeyCode.E)) upDown = 1f;
        if (Input.GetKey(KeyCode.Q)) upDown = -1f;

        if (hor != 0f || ver != 0f || upDown != 0f)
        {
            BreakFocus(); // Soltamos el nodo al movernos manualmente
            float currentSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeed *= shiftMultiplier;
            }
            
            // Calculamos dirección basada en hacia dónde mira la cámara
            Vector3 moveDir = (camTransform.right * hor) + (camTransform.forward * ver) + (Vector3.up * upDown);
            targetPosition += moveDir.normalized * currentSpeed * Time.deltaTime;
        }

        // 4. ZOOM
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoom -= scroll * zoomSpeed * 10f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void UpdateCameraTransform()
    {
        // A. Si estamos siguiendo un nodo, actualizamos la meta cada frame
        if (focusTarget != null)
        {
            targetPosition = focusTarget.position;
        }

        // B. Nos movemos suavemente hacia la meta (Persigue nodos en movimiento o deslizamiento WASD)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // C. Rotación
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        // D. Zoom suave
        if (camTransform != null)
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, new Vector3(0, 0, -currentZoom), Time.deltaTime * 10f);
        }
    }

    public void BreakFocus()
    {
        focusTarget = null;
    }

    public void FocusOnNode(Transform newTarget, float offsetDistance = 8f)
    {
        focusTarget = newTarget;
        
        // Animamos solo el Zoom con DOTween. El movimiento posicional lo maneja el Lerp de UpdateCameraTransform
        DOTween.To(() => currentZoom, x => currentZoom = x, offsetDistance, 1f).SetEase(Ease.InOutCubic).SetId(this);
    }
}