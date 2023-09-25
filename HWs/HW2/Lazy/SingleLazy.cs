namespace Lazy;

public class SingleLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private bool _isReady = false;
    private T? _result;
    private Exception? _supplierException;

    public SingleLazy(Func<T> supplier)
    {
        _supplier = supplier;
    }

    public T? Get()
    {
        if (_supplier is null)
        {
            throw new ArgumentNullException("Supplier cannot be null.");
        }

        if (_supplierException != null)
        {
            throw _supplierException;
        }

        if (!_isReady)
        {
            try
            {
                _result = _supplier();
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

        return _result;
    }
}
