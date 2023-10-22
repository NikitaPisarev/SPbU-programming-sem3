namespace Lazy;

/// <summary>
/// Interface to allow lazy initialization.
/// </summary>
/// <typeparam name="T">Result element type.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Getting the result. Calculates the result only on the first call. Then returns it.
    /// </summary>
    public T? Get();
}
