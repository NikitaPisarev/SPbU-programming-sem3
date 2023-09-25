namespace Lazy;

public class MultiLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private bool _isReady = false;
    private T? _result;
    private Exception? _supplierException;
    private readonly object _lockObject = new();


    public MultiLazy(Func<T> supplier)
    {
        if (supplier is null)
        {
            throw new ArgumentNullException("Supplier cannot be null.");
        }

        _supplier = supplier;
    }

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
                if (!_isReady)
                {
                    try
                    {
                        _result = _supplier!();
                    }
                    catch (Exception e)
                    {
                        _supplierException = e;
                        throw e;
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
