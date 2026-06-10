using UnityEngine;

public class CounterItem : MonoBehaviour, IClickable
{
    private Counter counter;
    private int index;

    public int Index => index;

    public void Setup(Counter counter, int index)
    {
        this.counter = counter;
        this.index = index;
    }

    public void OnClick()
    {
        Shopkeeper shopkeeper = FindFirstObjectByType<Shopkeeper>();

        if (shopkeeper == null)
            return;

        if (shopkeeper.IsBusy())
            return;

        shopkeeper.RemoveCounterItem(this);
    }
}