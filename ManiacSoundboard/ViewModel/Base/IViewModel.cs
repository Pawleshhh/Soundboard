using System;
using System.Windows;

/// <summary>
/// Interface to represent every view model of this application.
/// </summary>
public interface IViewModel : IDisposable
{

    /// <summary>
    /// Gets the owner of this view model.
    /// </summary>
    IViewModel Owner { get; }

    /// <summary>
    /// Gets the error handler of this view model.
    /// </summary>
    ErrorHandler ErrorHandler { get; }

    void WhenClosing();

}