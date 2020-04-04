using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfAppPage.Annotations;

namespace WpfAppPage.ViewModels
{

    public abstract class BaseViewModels: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(sender: this , e: new PropertyChangedEventArgs(propertyName: propertyName));
        }
    }

}
