using UnityEngine;
using TMPro;
using System.Collections;

public class Datapad : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    [Header("Datapad UI")]
    public GameObject datapadUIRoot;

    [Header("Button Indicator")]
    public Canvas placeholderButtonIndicator;

    private int index;
    private CanvasGroup datapadUIGroup;
    private Datapad dialogueSource;
    private Coroutine typeLineCoroutine;
    private bool isDatapadOpen;
    private float previousTimeScale = 1f;

    void Start()
    {
        EnsureDatapadUIRoot();
        EnsureDatapadUIGroup();

        if (textComponent != null)
            textComponent.text = string.Empty;

        if (ControlsExternalUI())
            HideDatapadUI();
    }

    void Update()
    {
        if (IsUIOnlyComponent()) return;

        if (isDatapadOpen && Input.GetMouseButtonDown(0))
        {
            CloseDatapad();
        }
    }

    public void Interact()
    {
        Debug.Log("Red Green Yellow");
        ShowDatapadUI();
        StartDialogue();
    }

    void ShowDatapadUI()
    {
        EnsureDatapadUIRoot();
        EnsureDatapadUIGroup();

        if (!ControlsExternalUI()) return;

        if (!isDatapadOpen)
            previousTimeScale = Time.timeScale;

        if (datapadUIGroup != null)
        {
            datapadUIGroup.alpha = 1f;
            datapadUIGroup.interactable = true;
            datapadUIGroup.blocksRaycasts = true;
        }

        if (placeholderButtonIndicator != null)
        {
            placeholderButtonIndicator.gameObject.SetActive(true);
            placeholderButtonIndicator.enabled = true;
        }

        isDatapadOpen = true;
        Time.timeScale = 0f;
    }

    public void CloseIndicator()
    {
        CloseDatapad();
    }

    void CloseDatapad()
    {
        if (!ControlsExternalUI()) return;

        HideDatapadUI();
        Time.timeScale = previousTimeScale;
        isDatapadOpen = false;
    }

    void HideDatapadUI()
    {
        StopTypeLine();

        if (placeholderButtonIndicator != null)
        {
            placeholderButtonIndicator.enabled = false;
            placeholderButtonIndicator.gameObject.SetActive(false);
        }

        if (datapadUIGroup != null)
        {
            datapadUIGroup.alpha = 0f;
            datapadUIGroup.interactable = false;
            datapadUIGroup.blocksRaycasts = false;
        }
    }

    void StartDialogue()
    {
        ResolveDialogueSource();

        if (!CanShowText()) return;

        index = 0;
        textComponent.text = string.Empty;
        StopTypeLine();
        typeLineCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in CurrentLines()[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(CurrentTextSpeed());
        }

        typeLineCoroutine = null;
    }

    void StopTypeLine()
    {
        if (typeLineCoroutine == null) return;

        StopCoroutine(typeLineCoroutine);
        typeLineCoroutine = null;
    }

    bool CanShowText()
    {
        return textComponent != null && HasDialogue(this);
    }

    void OnDisable()
    {
        StopTypeLine();

        if (ControlsExternalUI() && isDatapadOpen)
        {
            Time.timeScale = previousTimeScale;
            isDatapadOpen = false;
        }
    }

    bool ControlsExternalUI()
    {
        return placeholderButtonIndicator != null || (datapadUIRoot != null && datapadUIRoot != gameObject);
    }

    bool IsUIOnlyComponent()
    {
        return placeholderButtonIndicator == null && datapadUIRoot == gameObject;
    }

    void EnsureDatapadUIRoot()
    {
        if (datapadUIRoot == null && textComponent != null)
            datapadUIRoot = textComponent.transform.parent.gameObject;
    }

    void EnsureDatapadUIGroup()
    {
        if (datapadUIRoot == null || datapadUIRoot == gameObject) return;

        if (datapadUIGroup == null)
            datapadUIGroup = datapadUIRoot.GetComponent<CanvasGroup>();

        if (datapadUIGroup == null)
            datapadUIGroup = datapadUIRoot.AddComponent<CanvasGroup>();
    }

    void ResolveDialogueSource()
    {
        dialogueSource = null;

        if (HasOwnDialogue(this))
        {
            dialogueSource = this;
            return;
        }

        if (datapadUIRoot == null) return;

        foreach (Datapad datapad in datapadUIRoot.GetComponents<Datapad>())
        {
            if (datapad == this || !HasOwnDialogue(datapad)) continue;

            dialogueSource = datapad;
            if (textComponent == null)
                textComponent = datapad.textComponent;
            return;
        }
    }

    bool HasDialogue(Datapad datapad)
    {
        if (datapad == null) return false;

        if (HasOwnDialogue(datapad)) return true;

        if (dialogueSource == null)
            ResolveDialogueSource();

        return HasOwnDialogue(dialogueSource);
    }

    bool HasOwnDialogue(Datapad datapad)
    {
        if (datapad == null || datapad.lines == null) return false;

        foreach (string line in datapad.lines)
        {
            if (!string.IsNullOrEmpty(line))
                return true;
        }

        return false;
    }

    string[] CurrentLines()
    {
        return dialogueSource != null && HasOwnDialogue(dialogueSource) ? dialogueSource.lines : lines;
    }

    float CurrentTextSpeed()
    {
        if (dialogueSource != null && dialogueSource.textSpeed > 0f)
            return dialogueSource.textSpeed;

        return textSpeed;
    }
}
