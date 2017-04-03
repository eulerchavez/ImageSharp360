namespace ImageSharp360Viewer.ViewModel {

    // Se importa todo lo necesario

    using System.ComponentModel;

    /// <summary>
    /// Base class for ViewModels
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName) {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }

}
