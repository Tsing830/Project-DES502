using UnityEngine;
using TMPro;
using System.Collections;

public class Datapad : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    [Header("Button Indicator")]
    public Canvas placeholderButtonIndicator; 

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;

        if (placeholderButtonIndicator != null)
            placeholderButtonIndicator.enabled = false; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Red Green Yellow");
        StartDialogue();

        if (placeholderButtonIndicator != null)
        {
            StopCoroutine(nameof(ShowIndicator)); // Cancel any existing timer
            StartCoroutine(nameof(ShowIndicator));
        }
    }

    IEnumerator ShowIndicator()
    {
        placeholderButtonIndicator.enabled = true;
        yield return new WaitForSeconds(6f);
        placeholderButtonIndicator.enabled = false;
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}