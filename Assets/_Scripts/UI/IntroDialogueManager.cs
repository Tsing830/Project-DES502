using UnityEngine;

public class IntroDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject blackScreenBg;
    public Transform event1Container;

    private int currentSentenceIndex = 0;
    private bool isIntroPlaying = false;

    void Start()
    {
        StartIntro();
    }

    void StartIntro()
    {
        isIntroPlaying = true;

        blackScreenBg.SetActive(true);

        // Pause game time
        Time.timeScale = 0f;

        // Hide all sentence nodes initially
        for (int i = 0; i < event1Container.childCount; i++)
        {
            event1Container.GetChild(i).gameObject.SetActive(false);
        }

        // Show the first sentence if available
        if (event1Container.childCount > 0)
        {
            currentSentenceIndex = 0;
            event1Container.GetChild(currentSentenceIndex).gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No sentences found under Event1 container.");
            EndIntro();
        }
    }

    void Update()
    {
        if (!isIntroPlaying) return;

        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextSentence();
        }
    }

    void ShowNextSentence()
    {
        // Hide current sentence
        event1Container.GetChild(currentSentenceIndex).gameObject.SetActive(false);

        currentSentenceIndex++;

        // Show next sentence or end intro
        if (currentSentenceIndex < event1Container.childCount)
        {
            event1Container.GetChild(currentSentenceIndex).gameObject.SetActive(true);
        }
        else
        {
            EndIntro();
        }
    }

    void EndIntro()
    {
        isIntroPlaying = false;

        blackScreenBg.SetActive(false);
        event1Container.gameObject.SetActive(false);

        // Resume game time
        Time.timeScale = 1f;

        Debug.Log("Intro finished. Game started.");
    }
}