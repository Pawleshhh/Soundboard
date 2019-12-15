using System;
/// <summary>
/// Provides functions to handle errors that occurr on this application.
/// </summary>
public class ErrorHandler : ObservedClass
{

    #region Constructors

    #endregion

    #region Private fields

    #endregion

    #region Properties

    /// <summary>
    /// Gets the exception that caused the error.
    /// </summary>
    public Exception CurrentException { get; private set; }

    /// <summary>
    /// Gets the message for the user about the current error.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Gets whether there is any error or not.
    /// </summary>
    public bool IsErrorEmpty => CurrentException == null;

    #endregion

    #region Events

    /// <summary>
    /// Rises when an error occurrs.
    /// </summary>
    public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

    private void OnErrorOccurred(Exception ex, string message)
    {
        ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(ex, message));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets an error.
    /// </summary>
    /// <param name="ex">Exception that caused the error.</param>
    public void SetError(Exception ex)
    {
        SetError(ex, ex.Message);
    }

    /// <summary>
    /// Sets an error.
    /// </summary>
    /// <param name="ex">Exception that caused the error.</param>
    /// <param name="message">Message for the user about the error.</param>
    public void SetError(Exception ex, string message)
    {
        if (ex == null) return;
        CurrentException = ex;
        Message = message;

        OnErrorOccurred(ex, message);
    }

    /// <summary>
    /// Clears current error.
    /// </summary>
    public void HandleError()
    {
        CurrentException = null;
        Message = string.Empty;
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod(Action method)
    {
        return TryMethod(method, string.Empty);
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="message">The message for the user about the error if occurrs.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod(Action method, string message)
    {
        if (method == null)
            throw new ArgumentNullException("method", "Action parameter cannot be null.");

        //Try the method, if the method throws an exception then set the error.
        try
        {
            method.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(message))
                SetError(ex, message);
            else
                SetError(ex);
        }

        return false;
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="defaultMessage">Default message for the user if any of the given types is not a type of the exception that has been thrown.</param>
    /// <param name="messages">The tuples with two items, Item1 is the type of an exception, Item2 is the message for the type. 
    /// If one of the type from the tuples is the type of the thrown exception then the message of this type will be shown.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod(Action method, string defaultMessage, params Tuple<Type, string>[] messages)
    {
        if (method == null)
            throw new ArgumentNullException("method", "Action parameter cannot be null.");

        //Try the method, if the method throws an exception then set the error.
        try
        {
            method.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            //Check if the given array of tuples contains type of the thrown exception
            if (_CheckMessageTuple(ex, messages)) return false;

            SetError(ex, defaultMessage);
        }

        return false;
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="result">The result of the method.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod<TResult>(Func<TResult> method, ref TResult result)
    {
        return TryMethod(method, string.Empty, ref result);
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="message">The message for the user about the error if occurrs.</param>
    /// <param name="result">The result of the method.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod<TResult>(Func<TResult> method, string message, ref TResult result)
    {
        if (method == null)
            throw new ArgumentNullException("method", "Action parameter cannot be null.");

        //Try the method, if the method throws an exception then set the error.
        try
        {
            result = method.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(message))
                SetError(ex, message);
            else
                SetError(ex);
        }

        return false;
    }

    /// <summary>
    /// Invokes the given method and handles an exception if occurrs.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="defaultMessage">Default message for the user if any of the given types is not a type of the exception that has been thrown.</param>
    /// <param name="result">The result of the method.</param>
    /// <param name="messages">The tuples with two items, Item1 is the type of an exception, Item2 is the message for the type. 
    /// If one of the type from the tuples is the type of the thrown exception then the message of this type will be shown.</param>
    /// <returns>Returns true if any exception has not occurred. Otherwise; false.</returns>
    public bool TryMethod<TResult>(Func<TResult> method, string defaultMessage, ref TResult result, params Tuple<Type, string>[] messages)
    {
        if (method == null)
            throw new ArgumentNullException("method", "Action parameter cannot be null.");

        //Try the method, if the method throws an exception then set the error.
        try
        {
            result = method.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            //Check if the given array of tuples contains type of the thrown exception
            if (_CheckMessageTuple(ex, messages)) return false;

            SetError(ex, defaultMessage);
        }

        return false;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Checks if the tuple array contains the type of an exception.
    /// </summary>
    /// <param name="ex">The exception that its type might be stored in the tuple array.</param>
    /// <param name="messages">The tuple array to check if contains the type of the given exception.</param>
    /// <returns>Returns true if the tuple contains the type of the exception. Otherwise; false.</returns>
    private bool _CheckMessageTuple(Exception ex, Tuple<Type, string>[] messages)
    {
        Type exType = ex.GetType();
        for (int i = 0; i < messages.Length; i++)
        {
            if (messages[i].Item1.Equals(exType))
            {
                SetError(ex, messages[i].Item2);
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Static properties

    private static ErrorHandler _handler;

    /// <summary>
    /// Gets the <see cref="ErrorHandler"/> for this application's view model.
    /// </summary>
    public static ErrorHandler Handler
    {
        get
        {
            if (_handler == null)
                _handler = new ErrorHandler();

            return _handler;
        }
    }

    #endregion

}

/// <summary>
/// Class to deliver data about an error that occurred through the event.
/// </summary>
public class ErrorOccurredEventArgs : EventArgs
{

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the message of the exception.
    /// </summary>
    public string Message { get; }

    public ErrorOccurredEventArgs(Exception ex, string message)
    {
        Exception = ex;
        Message = message;
    }
}