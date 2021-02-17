using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace zBind.Mvvm.HomePage
{
    public class HomePageVm : INotifyPropertyChanged
    {
        public HomePageVm()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(10*1000), MyTimerCallback);

            TestVm = new TestVm();
        }

        private bool MyTimerCallback()
        {
            TestVm = new TestVm();
            Count++;
            return true;
        }

        private long _count;

        public long Count { get => _count; 
            set
            {
                if(_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }

        private TestVm _testVm;

        public TestVm TestVm
        {
            get => _testVm;
            set
            {
                if (_testVm != value)
                {
                    _testVm = value;
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
