using System;
using UnityEngine;

[Serializable]
public abstract class GameVar<T> : ScriptableObject
{
    [SerializeField] private T _value;

    public Action<T> OnValueChanged;

    public virtual T Value
    {
        get { return _value; }
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

#if UNITY_EDITOR
    // used to update the value in the editor
    private void OnValidate()
    {
        Value = _value;
    }
#endif

}
