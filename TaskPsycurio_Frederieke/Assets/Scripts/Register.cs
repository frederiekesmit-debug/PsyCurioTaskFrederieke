using UnityEngine;

public class Register : MonoBehaviour, IClickable
{
    [SerializeField] private Counter counter;
    [SerializeField] private SpeechBubble speechBubble;

    public void OnClick()
    {
        if (counter == null)
        {
            Debug.LogError("Counter is not assigned on Register!");
            return;
        }

        if (speechBubble == null)
        {
            Debug.LogError("SpeechBubble is not assigned on Register!");
            return;
        }

        string message = BuildOrderMessage();
        speechBubble.ShowMessage(message);
    }

    private string BuildOrderMessage()
    {
        GameObject[] items = counter.GetPlacedItems();

        if (items == null || items.Length == 0)
        {
            return "You've selected nothing";
        }

        bool hasItems = false;
        string result = "You are buying: ";

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                hasItems = true;
                result += items[i].name + ", ";
            }
        }

        if (!hasItems)
        {
            return "You've selected nothing";
        }

        // remove last comma
        result = result.TrimEnd(',', ' ');

        return result;
    }
}
