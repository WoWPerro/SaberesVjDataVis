using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public CanvasGroup dataPanelGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI categoryText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI quoteText;

    [Header("Referencias de Control")]
    public CameraController cameraController;

    private void Start()
    {
        // Asegurarnos de que el panel esté oculto e inactivo al inicio
        if (dataPanelGroup != null)
        {
            dataPanelGroup.alpha = 0f;
            dataPanelGroup.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Abre el panel de datos, rellena la información desde el ScriptableObject y hace Fade In.
    /// </summary>
    public void OpenDataPanel(NodeData data)
    {
        if (data == null) return;

        // Llenado de textos usando la información procedimental del nodo
        if (titleText != null) titleText.text = data.nodeTitle;
        if (categoryText != null) categoryText.text = data.ejeY_Habermas.ToString();
        if (descriptionText != null) descriptionText.text = data.descripcionCartografica;
        if (quoteText != null) quoteText.text = $"\"{data.citaExperto}\"";

        // Animación de entrada con DOTween
        if (dataPanelGroup != null)
        {
            dataPanelGroup.DOKill(); // Detenemos cualquier fade anterior
            dataPanelGroup.blocksRaycasts = true; // Activar el panel para capturar eventos de UI (como un botón de cerrar)
            dataPanelGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    /// <summary>
    /// Cierra el panel de datos con un Fade Out y libera la cámara de su enfoque actual.
    /// Puede ser llamado desde un Botón (UI Button) en el Canvas.
    /// </summary>
    public void CloseDataPanel()
    {
        // Animación de salida con DOTween
        if (dataPanelGroup != null)
        {
            dataPanelGroup.DOKill();
            dataPanelGroup.blocksRaycasts = false; // Desactivar la interacción inmediatamente
            dataPanelGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad);
        }

        // Liberar el anclaje de la cámara
        if (cameraController != null)
        {
            cameraController.BreakFocus();
        }
    }

    /// <summary>
    /// Método placeholder llamado por el SelectionManager al hacer Hover sobre un nodo.
    /// Muestra un tooltip flotante junto al cursor.
    /// </summary>
    public void ShowTooltip(Vector3 pos, string title)
    {
        // TODO: Implementar lógica de Tooltip flotante si se requiere
    }

    /// <summary>
    /// Método placeholder llamado por el SelectionManager al salir del Hover.
    /// Oculta el tooltip flotante.
    /// </summary>
    public void HideTooltip()
    {
        // TODO: Ocultar el Tooltip flotante
    }
}
