using System.ComponentModel;

/// <summary>
/// Defines a class that is being observed.
/// </summary>
public abstract class ObservedClass : INotifyPropertyChanged
{

    /// <summary>
    /// Rises when the specified property has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> event by sending names of the properties that have changed.
    /// </summary>
    /// <param name="properties">Array of the names of the properties.</param>
    public void OnPropertyChanged(params string[] properties)
    {
        foreach (string property in properties)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}