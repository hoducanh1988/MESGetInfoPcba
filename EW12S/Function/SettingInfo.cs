using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12S.Function {
    public class SettingInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
                Properties.Settings.Default.Save();
            }
        }


        #region product
        
        public string ipAddress {
            get { return Properties.Settings.Default.IPAddress; }
            set {
                Properties.Settings.Default.IPAddress = value;
                OnPropertyChanged(nameof(ipAddress));
            }
        }
        public string sshUser {
            get { return Properties.Settings.Default.sshUser; }
            set {
                Properties.Settings.Default.sshUser = value;
                OnPropertyChanged(nameof(sshUser));
            }
        }
        public string sshPassword {
            get { return Properties.Settings.Default.sshPass; }
            set {
                Properties.Settings.Default.sshPass = value;
                OnPropertyChanged(nameof(sshPassword));
            }
        }

        #endregion

        #region standard

        public string fwVersion {
            get { return Properties.Settings.Default.fwVersion; }
            set {
                Properties.Settings.Default.fwVersion = value;
                OnPropertyChanged(nameof(fwVersion));
            }
        }
        public string fwBuildTime {
            get { return Properties.Settings.Default.fwBuildTime; }
            set {
                Properties.Settings.Default.fwBuildTime = value;
                OnPropertyChanged(nameof(fwBuildTime));
            }
        }
        public string hwVersion {
            get { return Properties.Settings.Default.hwVersion; }
            set {
                Properties.Settings.Default.hwVersion = value;
                OnPropertyChanged(nameof(hwVersion));
            }
        }
        public string modelNumber {
            get { return Properties.Settings.Default.modelNumber; }
            set {
                Properties.Settings.Default.modelNumber = value;
                OnPropertyChanged(nameof(modelNumber));
            }
        }

        #endregion

    }
}
