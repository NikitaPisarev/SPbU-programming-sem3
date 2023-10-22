namespace Lazy;

/// <summary>
/// Implementation of the ILazy interface for single-threaded use.
/// </summary>
public class SingleLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private bool _isComputed = false;
    private T? _result;
    private Exception? _supplierException;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Lazy function.</param>
    /// <exception cref="ArgumentNullException">Supplier cannot be null.</exception>
    public SingleLazy(Func<T> supplier)
    {
        if (supplier is null)
        {
            throw new ArgumentNullException("Supplier cannot be null.");
        }

        _supplier = supplier;
    }

    /// <inheritdoc cref="ILazy{T}.Get"/>
    public T? Get()
    {
        if (_supplierException != null)
        {
            throw _supplierException;
        }

        if (!_isComputed)
        {
            try
            {
                _result = _supplier!();
            }
            catch (Exception e)
            {
                _supplierException = e;
                throw;
            }
            finally
            {
                _isComputed = true;
                _supplier = null;
            }
        }

        return _result;
    }
}
