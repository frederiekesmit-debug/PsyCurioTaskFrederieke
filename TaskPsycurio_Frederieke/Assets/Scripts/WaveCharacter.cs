using UnityEngine;

public class WaveCharacter : MonoBehaviour, IClickable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string waveTrigger = "Wave";

    public void OnClick()
    {
        animator.SetTrigger(waveTrigger);
    }
}