using System;
using System.ComponentModel;
using System.Windows;

namespace HotMess
{
    public class BaseNotifying : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                Action d = () =>
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                };
                Application.Current.BeginInvoke(d);
            }
        }
    }
}