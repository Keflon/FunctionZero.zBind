using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace zBind.Mvvm.HomePage
{
    public class TestVm : INotifyPropertyChanged
    {
        public TestVm()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(22), MyTimerCallback);
        }

        private bool MyTimerCallback()
        {
            Count++;
            return true;
        }

        private long _count;

        public long Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
