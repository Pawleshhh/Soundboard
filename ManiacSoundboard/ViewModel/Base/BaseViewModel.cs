using System.Windows;
using System.Xml.Serialization;

/// <summary>
/// Abstract class that defines the base view model of this application.
/// </summary>
public abstract class BaseViewModel : ObservedClass, IViewModel
{

    protected BaseViewModel()
    {
        ErrorHandler = ErrorHandler.Handler;

        ErrorHandler.ErrorOccurred += ErrorHandler_ErrorOccurred;
    }

    protected BaseViewModel(IViewModel owner) : this()
    {
        Owner = owner;
    }

    /// <summary>
    /// Gets the error handler.
    /// </summary>
    [XmlIgnore] public ErrorHandler ErrorHandler { get; protected set; }

    /// <summary>
    /// Gets the owner of this view model.
    /// </summary>
    [XmlIgnore]public IViewModel Owner { get; protected set; }

    /// <summary>
    /// Gets whether the view model has the owner or not.
    /// </summary>
    [XmlIgnore] public bool HasOwner => Owner != null;

    public virtual void Dispose()
    {

    }

    public virtual void WhenClosing()
    {
        Dispose();
    }

    /// <summary>
    /// Rises when the <see cref="ErrorHandler.ErrorOccurred"/> event rises.
    /// </summary>
    protected virtual void OnErrorOccurred()
    {

    }

    /// <summary>
    /// Subscribed method to the <see cref="ErrorHandler.ErrorOccurred"/> event.
    /// </summary>
    private void ErrorHandler_ErrorOccurred(object sender, ErrorOccurredEventArgs e)
    {
        OnErrorOccurred();
    }

}