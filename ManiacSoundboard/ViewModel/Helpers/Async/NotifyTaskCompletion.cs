using System;
using System.Threading.Tasks;

/// <summary>
/// Code from https://msdn.microsoft.com/en-us/magazine/dn605875.aspx?f=255&MSPPError=-2147217396.
/// </summary>
public class NotifyTaskCompletion<TResult> : ObservedClass
{

    public NotifyTaskCompletion(Task<TResult> task)
    {
        Task = task;
        if (!task.IsCompleted)
        {
            var _task = WatchTaskAsync(task);
        }
    }

    public NotifyTaskCompletion(Task<TResult> task, Action finished)
    {
        _finished = finished;
        Task = task;
        if (!task.IsCompleted)
        {
            var _task = WatchTaskAsync(task);
        }
    }

    private Action _finished;

    public Task<TResult> Task { get; private set; }

    public TResult Result
    {
        get
        {
            return (Task.Status == TaskStatus.RanToCompletion) ?
                Task.Result : default(TResult);
        }
    }

    public TaskStatus Status { get { return Task.Status; } }

    public bool IsCompleted { get { return Task.IsCompleted; } }

    public bool IsNotCompleted { get { return !Task.IsCompleted; } }

    public bool IsSuccessfullyCompleted
    {
        get
        {
            return Task.Status == TaskStatus.RanToCompletion;
        }
    }

    public bool IsCanceled { get { return Task.IsCanceled; } }

    public bool IsFaulted { get { return Task.IsFaulted; } }

    public AggregateException Exception { get { return Task.Exception; } }

    public Exception InnerException
    {
        get
        {
            return (Exception == null) ?
                null : Exception.InnerException;
        }
    }

    public string ErrorMessage
    {
        get
        {
            return (InnerException == null) ?
                null : InnerException.Message;
        }
    }

    private async Task WatchTaskAsync(Task task)
    {

        try
        {
            await task.ConfigureAwait(false);
        }
        catch
        {
        }

        OnPropertyChanged("Status", "IsCompleted", "IsNotCompleted");

        if (task.IsCanceled)
        {
            OnPropertyChanged("IsCanceled");
        }
        else if (task.IsFaulted)
        {
            OnPropertyChanged("IsFaulted", "Exeption", "InnerException", "ErrorMessage");
        }
        else
        {
            OnPropertyChanged("IsSuccessfullyCompleted", "Result");
        }

        _finished?.Invoke();
    }

}
