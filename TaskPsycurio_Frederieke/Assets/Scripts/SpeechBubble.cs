using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private TMP_Text text;

    public void ShowMessage(string message, float duration = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(Display(message, duration));
    }

    private System.Collections.IEnumerator Display(string msg, float duration)
    {
        bubbleRoot.SetActive(true);
        text.text = msg;

        yield return new WaitForSeconds(duration);

        bubbleRoot.SetActive(false);
    }
}
