using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SafeUIController : MonoBehaviour
{
    public GameObject safeUIPanel; // The UI panel for the safe
    public TextMeshProUGUI displayText; // Text to display the entered code
    public GameObject hudOverlay;



    private string enteredCode = ""; // The code entered by the player
    private SafeController safeController; // Reference to the SafeController

    private void Start()
    {
        safeUIPanel.SetActive(false); // Hide the UI panel at the start
    }

    public void OpenSafeUI(SafeController Safe)
    {
       safeController = Safe;
       enteredCode = ""; // Reset entered code
       displayText.text = ""; // Clear display text
       safeUIPanel.SetActive(true); // Show the UI panel
       hudOverlay.SetActive(false); // Hide the HUD overlay

        LockPlayer(true); // Lock player movement
        PauseGame(true); // Pause the game
    }

    public void CloseSafeUI()
    {
        safeUIPanel.SetActive(false); // Hide the UI panel
        hudOverlay.SetActive(true); // Show the HUD overlay
        LockPlayer(false); // Unlock player movement
        PauseGame(false); // Resume the game
    }

    public void PressDigit(string digit)
    {
        Debug.Log("Pressed: " + digit);


        if (enteredCode.Length < 4) // Limit code to 4 digits
        {
            enteredCode += digit; // Append the pressed digit
            displayText.text = enteredCode; // Update display text
        }
        Debug.Log(enteredCode);
    }

    public void ClearCode()
    {
        enteredCode = ""; // Clear the entered code
        displayText.text = ""; // Clear display text
    }

    public void pressEnter() {
        if (safeController.CheckCode(enteredCode)) { 
           displayText.text = "Safe Opened!";
        } else {
           displayText.text = "Incorrect!";
        }; // Check the entered code with the SafeController
    }

    public void pressEscape()
    {
        CloseSafeUI();
    }




    private void LockPlayer(bool locked)
    {
        var playerController = FindFirstObjectByType<SimplePlayerController>();
        if (playerController != null)
            playerController.enabled = !locked; // Enable or disable player movement

        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked; // Unlock or lock the cursor
        Cursor.visible = locked; // Show or hide the cursor
    }

    private void PauseGame(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f; // Pause or resume the game
    }
}
