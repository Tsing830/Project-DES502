using UnityEngine;
using System.Collections;

public class TriggerDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The container (e.g., Event2) holding sentence GameObjects")]
    public Transform dialogueContainer;

    [Header("Settings")]
    [Tooltip("Time each sentence stays on screen")]
    public float displayDuration = 3.0f;

    [Tooltip("The tag of the object that triggers the dialogue")]
    public string playerTag = "Player";

    private bool hasTriggered = false;

    private void Start()
    {
        // Ensure the container and all its children are hidden at start
        if (dialogueContainer != null)
        {
            dialogueContainer.gameObject.SetActive(false);
            foreach (Transform child in dialogueContainer)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger only once when the player enters
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            StartCoroutine(PlayDialogueSequence());
        }
    }

    private IEnumerator PlayDialogueSequence()
    {
        if (dialogueContainer == null) yield break;

        dialogueContainer.gameObject.SetActive(true);

        // Iterate through each sentence in order
        foreach (Transform sentence in dialogueContainer)
        {
            // Show current sentence
            sentence.gameObject.SetActive(true);

            // Wait for the specified duration
            yield return new WaitForSeconds(displayDuration);

            // Hide current sentence
            sentence.gameObject.SetActive(false);
        }

        // Hide the entire container after finished
        dialogueContainer.gameObject.SetActive(false);

        Debug.Log("Trigger dialogue sequence finished.");
    }
}