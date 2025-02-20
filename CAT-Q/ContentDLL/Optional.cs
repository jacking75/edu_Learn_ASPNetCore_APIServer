namespace ContentDLL;

public struct Optional<T>
{
    private readonly T _value;
    private readonly bool _hasValue;

    public bool HasValue => _hasValue;

    public T Value
    {
        get
        {
            if (!_hasValue)
                throw new InvalidOperationException("Optional has no value");
            return _value;
        }
    }

    private Optional(T value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public static Optional<T> Some(T value) => new Optional<T>(value, true);
    public static Optional<T> None() => new Optional<T>(default, false);

    public T ValueOr(T defaultValue) => _hasValue ? _value : defaultValue;

    public Optional<TResult> Map<TResult>(Func<T, TResult> mapper)
        => _hasValue ? Optional<TResult>.Some(mapper(_value)) : Optional<TResult>.None();

    public T Match(Func<T, T> some, Func<T> none)
        => _hasValue ? some(_value) : none();

    public void Match(Action<T> some, Action none)
    {
        if (_hasValue)
            some(_value);
        else
            none();
    }

    public bool TryGetValue(out T value)
    {
        value = _value;
        return _hasValue;
    }

    public override string ToString()
        => _hasValue ? $"Some({_value})" : "None";

    public static implicit operator Optional<T>(T value)
        => new Optional<T>(value, true);
}
