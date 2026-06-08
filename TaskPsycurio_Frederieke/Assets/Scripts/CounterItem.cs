using UnityEngine;

public class CounterItem : MonoBehaviour, IClickable
{
    private Counter counter;
    private int index;

    public void Setup(Counter counter, int index)
    {
        this.counter = counter;
        this.index = index;
    }

    public void OnClick()
    {
        if (counter != null)
        {
            counter.RemoveItem(index);
        }

        Destroy(gameObject);
    }
}