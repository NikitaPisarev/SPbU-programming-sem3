namespace Lazy;

/// <summary>
/// Implementation of the ILazy interface for multi-threaded use.
/// </summary>
public class MultiLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private volatile bool _isReady = false;
    private T? _result;
    private Exception? _supplierException;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Lazy function.</param>
    /// <exception cref="ArgumentNullException">Supplier cannot be null.</exception>
    public MultiLazy(Func<T> supplier)
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

        if (!_isReady)
        {
            lock (_lockObject)
            {
                if (_supplierException != null)
                {
                    throw _supplierException;
                }

                if (!_isReady)
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
                        _isReady = true;
                        _supplier = null;
                    }
                }
            }
        }

        return _result;
    }
}
