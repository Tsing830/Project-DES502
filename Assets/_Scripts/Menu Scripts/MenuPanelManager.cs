using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas[] canvases;

    [Header("Navigation")]
    public Canvas backArrow;

    [Header("Default Canvas")]
    public Canvas mainMenuCanvas;

    private void Start()
    {
        HideAllCanvases();
        if (mainMenuCanvas != null) mainMenuCanvas.enabled = true;
    }

    public void ShowCanvas(Canvas canvas)
    {
        HideAllCanvases();
        canvas.enabled = true;
        if (backArrow != null) backArrow.enabled = true;
    }

    public void HideAllCanvases()
    {
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }
        if (backArrow != null) backArrow.enabled = false;
    }
}