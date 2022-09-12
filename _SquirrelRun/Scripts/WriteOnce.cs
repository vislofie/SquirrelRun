﻿using System;

public sealed class WriteOnce<T>
{
    private T _value;
    private bool _hasValue;
    public override string ToString()
    {
        return _hasValue ? Convert.ToString(_value) : string.Empty;
    }
    public T Value
    {
        get
        {
            if (!_hasValue) throw new InvalidOperationException("Value not set");
            return _value;
        }
        set
        {
            if (_hasValue) throw new InvalidOperationException("Value already set");
            this._value = value;
            this._hasValue = true;
        }
    }
    public static implicit operator T(WriteOnce<T> value) { return value.Value; }
}

