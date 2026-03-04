using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Configuración")]
    public Transform camTransform; // La Main Camera aquí (el hijo)
    
    [Header("Seguimiento de Nodo")]
    public Transform focusTarget; // El nodo al que seguimos (si hay uno)
    public float targetFollowSpeed = 10f; // Velocidad de interpolación hacia el objetivo móvil
    private bool isFlyingToTarget = false; // Bloquea WASD mientras DOTween vuela hacia el nodo inicial

    [Header("Orbit & Pan")]
    public float orbitSpeed = 3f;
    public float panSpeed = 0.5f;
    private float yaw, pitch;

    [Header("Zoom (Distancia al Pivote)")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 150f;
    private float currentZoom = 20f;

    [Header("Movimiento Libre (WASD)")]
    public float moveSpeed = 40f; // Aumentado para que se note en distancias grandes
    public float shiftMultiplier = 3f;

    // Quitando virtualTargetPosition que causaba conflictos con el Edit Mode de Unity.

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        
        if (camTransform != null)
        {
            camTransform.localPosition = new Vector3(0, 0, -currentZoom);
            camTransform.localRotation = Quaternion.identity;
        }
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraTransform();
    }

    void HandleInput()
    {
        // ------------- PANEL & ORBIT -------------
        
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * orbitSpeed;
            pitch -= Input.GetAxis("Mouse Y") * orbitSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }

        if (Input.GetMouseButton(2) || (Input.GetKey(KeyCode.Space) && Input.GetMouseButton(0)))
        {
            float currentPanSpeed = panSpeed * 4f; 
            Vector3 panDelta = -camTransform.right * Input.GetAxis("Mouse X") * currentPanSpeed 
                               - camTransform.up * Input.GetAxis("Mouse Y") * currentPanSpeed;
            
            BreakFocus(); 
            // Movimiento local inmediato y sin restricciones
            transform.position += panDelta;
        }

        // ------------- MOVIMIENTO WASD (Vuelo Libre) -------------
        
        if (!isFlyingToTarget)
        {
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
                BreakFocus(); 
                
                float currentSpeed = moveSpeed;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    currentSpeed *= shiftMultiplier;
                }

                // Generamos dirección desde el hijo pa que WASD siempre sea adelante en cámara
                Vector3 moveDir = (camTransform.right * hor) + (camTransform.forward * ver) + (Vector3.up * upDown);
                
                // Aplicamos directo a la posición del padre sin pasar por virtuales
                transform.position += moveDir.normalized * currentSpeed * Time.deltaTime;
            }
        }

        // ------------- ZOOM -------------
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoom -= scroll * zoomSpeed * 10f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void UpdateCameraTransform()
    {
        // 1. Seguimiento de nodo
        // Hacemos el Lerp DIRECTAMENTE al Transform, solo cuando sabemos que hay un target.
        if (focusTarget != null && !isFlyingToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, focusTarget.position, Time.deltaTime * targetFollowSpeed);
        }

        // Si focusTarget es null (porque nos movimos con pan o WASD), NO HACEMOS NADA con la posición aquí.
        // Eso permite que el script no estorbe jamás en el SceneView de Unity ni bloquee los Inputs procesados arriba.

        // 2. Rotación
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        // 3. Zoom
        if (camTransform != null)
        {
            camTransform.localPosition = new Vector3(0, 0, -currentZoom);
        }
    }

    // Rompe el anclaje al nodo objetivo
    public void BreakFocus()
    {
        focusTarget = null;
        transform.DOKill();
        isFlyingToTarget = false;
    }

    // Llamado por el SelectionManager al hacer clic en un nodo
    public void FocusOnNode(Transform newTarget, float offsetDistance = 15f)
    {
        BreakFocus(); 
        focusTarget = newTarget;
        isFlyingToTarget = true; // Bloquea WASD mientras se anima
        
        // Volar al nodo fluidamente. Al terminar, desactivamos la bandera para permitir el seguimiento de nodo y controles manuales
        transform.DOMove(newTarget.position, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            isFlyingToTarget = false;
        });
        
        // Interpolación del zoom/offset
        DOTween.To(() => currentZoom, x => currentZoom = x, offsetDistance, 1f).SetEase(Ease.InOutCubic).SetId(this);
    }
}