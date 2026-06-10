using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private TMP_Text text;

    private System.Collections.IEnumerator activeRoutine;

    public void ShowMessage(string message)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = Display(message);
        StartCoroutine(activeRoutine);
    }

    public void Hide()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        bubbleRoot.SetActive(false);
    }

    public bool IsVisible()
    {
        return bubbleRoot.activeSelf;
    }

    private System.Collections.IEnumerator Display(string msg)
    {
        bubbleRoot.SetActive(true);
        text.text = msg;

        // stays open until manually closed
        yield return null;
    }
}
