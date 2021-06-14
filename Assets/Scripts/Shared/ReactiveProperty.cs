using System;

public class ReactiveProperty<T>
{
    public event Action<T> OnValueChanged;
    
    private T _Value;

    public ReactiveProperty(T val)
    {
        _Value = val;
    }

    public ReactiveProperty()
    {
        _Value = default;
    }

    public T Value
    {
        get => _Value;
        set
        {
            if (value.Equals(_Value))
                return;
            _Value = value;
            OnValueChanged?.Invoke(value);
        }
    }
}
