namespace MyThreadPool;

using System.Collections.Concurrent;
using System.Threading;

public class MyThreadPool
{
    private readonly Thread[] _threads;
    private readonly ConcurrentQueue<Action> _taskQueue;
    private readonly CancellationTokenSource _cts;
    private readonly object _lockObject;


    public MyThreadPool(int threadNumber)
    {
        _threads = new Thread[threadNumber];
        _taskQueue = new();
        _cts = new();
        _lockObject = new();

        for (int i = 0; i < threadNumber; i++)
        {
            _threads[i] = new Thread(Work);
            _threads[i].Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (_cts.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException("Thread pool was shut down.");
        }

        lock (_lockObject)
        {
            var myTask = new MyTask<TResult>(function, this);
            _taskQueue.Enqueue(myTask.Execute);
            Monitor.Pulse(_lockObject);

            return myTask;
        }
    }

    private void Work()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            Action? task = null;
            lock (_lockObject)
            {
                while (!_taskQueue.TryDequeue(out task) && !_cts.Token.IsCancellationRequested)
                {
                    Monitor.Wait(_lockObject);
                }
            }
            task?.Invoke();
        }
    }

    public void Shutdown()
    {
        lock (_lockObject)
        {
            _cts.Cancel();
            Monitor.PulseAll(_lockObject);
        }

        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> _function;
        private MyThreadPool _threadPool;
        private TResult? _result;
        private volatile bool _isCompleted = false;
        private Exception? _exception;
        private object _syncObject = new();

        public MyTask(Func<TResult> function, MyThreadPool threadPool)
        {
            _function = function;
            _threadPool = threadPool;
        }

        public bool IsCompleted => _isCompleted;

        public TResult? Result
        {
            get
            {
                lock (_syncObject)
                {
                    while (!_isCompleted)
                    {
                        Monitor.Wait(_syncObject);
                    }

                    if (_exception != null)
                    {
                        throw new AggregateException(_exception);
                    }

                    return _result;
                }
            }
        }

        internal void Execute()
        {
            try
            {
                _result = _function();
            }
            catch (Exception e)
            {
                _exception = e;
            }
            finally
            {
                lock (_syncObject)
                {
                    _isCompleted = true;
                    Monitor.Pulse(_syncObject);
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuationFunction)
        {
            return _threadPool.Submit(() => continuationFunction(Result));
        }
    }
}