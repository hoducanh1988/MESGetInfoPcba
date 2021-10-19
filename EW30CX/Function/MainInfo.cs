using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW30CX.Function {
    public class MainInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public MainInfo() {
            appInfo = "Version EW30CXVN0U0001 - Build date 28/09/2021 09:20 - Copyright of VNPT Technology 2021";
        }

        string _app_info;
        public string appInfo {
            get { return _app_info; }
            set {
                _app_info = value;
                OnPropertyChanged(nameof(appInfo));
            }
        }
    }
}
