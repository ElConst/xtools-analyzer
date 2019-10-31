using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Parent for VMs that has implementation of the INotifyPropertyChanged interface.</summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Raises the PropertyChanged event.</summary>
        /// <param name="propertyName">Name of the property being changed (can be set automatically)</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Sets property and raises PropertyChanged event if new value differs from old.</summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="storage">Backing field of the property.</param>
        /// <param name="value">New value that's needed to be set.</param>
        /// <param name="propertyName">Name of the property being changed (is set automatically).</param>
        /// <returns>Raised PropertyChanged or not.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) { return false; }

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
