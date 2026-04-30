using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitUI : MonoBehaviour
{
    [Header("Quit Menu")]
    public Canvas quitCanvas;

    private bool isOpen = false;

    void Start()
    {
        if (quitCanvas != null)
            quitCanvas.enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseQuitMenu();
            else
                OpenQuitMenu();
        }
    }

    void OpenQuitMenu()
    {
        isOpen = true;
        quitCanvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Frees the cursor to move
    }

    public void CloseQuitMenu()
    {
        isOpen = false;
        quitCanvas.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Locks cursor back to centre
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}