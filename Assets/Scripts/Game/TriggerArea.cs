using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public UnityEvent OnEnterTrigger;
    public UnityEvent OnExitTrigger;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
    }
}
