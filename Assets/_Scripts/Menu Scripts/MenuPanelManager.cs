using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject[] panels;

    void Start()
    {
        HideAllPanels();
    }

    public void ShowPanel(GameObject panel)
    {
        HideAllPanels();
        panel.SetActive(true);
    }

    public void HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
}