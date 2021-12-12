namespace Bender.Core.Nodes;

using System.Collections;
using System.Collections.Generic;
using Layouts;
using Rendering;

/// <summary>
///     A one dimensional collection of <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">Array type</typeparam>
public class BArrray<T> : BaseNode, IEnumerable<T> where T : IRenderable
{
    private readonly T[] _data;

    /// <summary>
    ///     Create a new BArray
    /// </summary>
    /// <param name="el">Parent</param>
    /// <param name="data">Array elements</param>
    public BArrray(Element el, T[] data) : base(el)
    {
        Ensure.IsNotNull(nameof(data), data);
        _data = data;
    }

    /// <summary>
    ///     Count of elements
    /// </summary>
    public int Length => _data.Length;

    /// <summary>
    ///     Get item at index
    /// </summary>
    /// <param name="index">Array index</param>
    public T this[int index] => _data[index];

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return new List<T>(_data).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"array({Length})";
    }
}