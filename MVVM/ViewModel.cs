using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace xammacmvvm;

public class ViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
   protected void OnPropertyChanging([CallerMemberName] string? callerName = null) =>
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(callerName));

   protected void OnPropertyChanged([CallerMemberName] string? callerName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Using a ref parameter here is intentional")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Using a default value is required for CallerMemberName")]
    protected bool Set<T>(
          ref T target,
          T newValue,
          IEqualityComparer<T> equalityComparer = null,
          [CallerMemberName] string propertyName = "")
    {
        return SetInternal(ref target, newValue, equalityComparer, propertyName);
    }

    private bool SetInternal<T>(
            ref T target,
            T newValue,
            IEqualityComparer<T> equalityComparer,
            string propertyName)
    {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;
        if (comparer.Equals(target, newValue))
        {
            // Always update the value anyway, as the equality may not be absolute, but objects may be
            // otherwise considered equivalent for display purposes
            target = newValue;
            return false;
        }

        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        target = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;
}

