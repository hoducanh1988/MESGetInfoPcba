using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW30CX.Function {
    public class TestingInfo : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public TestingInfo() {
            initParams();
        }

        public bool passedParams() {
            totalResult = "Passed";
            return true;
        }

        public bool failedParams() {
            totalResult = "Failed";
            return true;
        }

        public void waitParams(string mac) {
            totalResult = "Waiting...";
            macStamp = mac;
            errorMessage = "";
        }

        public void initParams() {
            sshLog = "";
            fwVersion = "";
            fwBuildTime = "";
            hwVersion = "";
            modelNumber = "";
            serialNumber = "";
            macEthernet = "";
            macWifi2G = "";
            macWifi5G = "";
            totalResult = "--";
            macStamp = "";
            errorMessage = "";
        }

        string _fwversion;
        public string fwVersion {
            get { return _fwversion; }
            set {
                _fwversion = value;
                OnPropertyChanged(nameof(fwVersion));
            }
        }
        string _fwbuildtime;
        public string fwBuildTime {
            get { return _fwbuildtime; }
            set {
                _fwbuildtime = value;
                OnPropertyChanged(nameof(fwBuildTime));
            }
        }
        string _hwversion;
        public string hwVersion {
            get { return _hwversion; }
            set {
                _hwversion = value;
                OnPropertyChanged(nameof(hwVersion));
            }
        }
        string _modelnumber;
        public string modelNumber {
            get { return _modelnumber; }
            set {
                _modelnumber = value;
                OnPropertyChanged(nameof(modelNumber));
            }
        }
        string _serialnumber;
        public string serialNumber {
            get { return _serialnumber; }
            set {
                _serialnumber = value;
                OnPropertyChanged(nameof(serialNumber));
            }
        }
        string _macethernet;
        public string macEthernet {
            get { return _macethernet; }
            set {
                _macethernet = value;
                OnPropertyChanged(nameof(macEthernet));
            }
        }
        string _macwifi2g;
        public string macWifi2G {
            get { return _macwifi2g; }
            set {
                _macwifi2g = value;
                OnPropertyChanged(nameof(macWifi2G));
            }
        }
        string _macwifi5g;
        public string macWifi5G {
            get { return _macwifi5g; }
            set {
                _macwifi5g = value;
                OnPropertyChanged(nameof(macWifi5G));
            }
        }
        string _sshlog;
        public string sshLog {
            get { return _sshlog; }
            set {
                _sshlog = value;
                OnPropertyChanged(nameof(sshLog));
            }
        }
        string _totalresult;
        public string totalResult {
            get { return _totalresult; }
            set {
                _totalresult = value;
                OnPropertyChanged(nameof(totalResult));
            }
        }
        string _macstamp;
        public string macStamp {
            get { return _macstamp; }
            set {
                _macstamp = value;
                OnPropertyChanged(nameof(macStamp));
            }
        }
        string _errormessage;
        public string errorMessage {
            get { return _errormessage; }
            set {
                _errormessage = value;
                OnPropertyChanged(nameof(errorMessage));
            }
        }
    }
}
