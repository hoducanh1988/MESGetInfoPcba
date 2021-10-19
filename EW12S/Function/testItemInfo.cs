using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12S.Function
{
    public class testItemInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        string _itemname;
        public string itemName {
            get { return _itemname; }
            set {
                _itemname = value;
                OnPropertyChanged(nameof(itemName));
            }
        }
        string _stdvalue;
        public string standardValue {
            get { return _stdvalue; }
            set {
                _stdvalue = value;
                OnPropertyChanged(nameof(standardValue));
            }
        }
        string _actvalue;
        public string actualValue {
            get { return _actvalue; }
            set {
                _actvalue = value;
                OnPropertyChanged(nameof(actualValue));
            }
        }
        string _itemresult;
        public string itemResult {
            get { return _itemresult; }
            set {
                _itemresult = value;
                OnPropertyChanged(nameof(itemResult));
            }
        }


    }
}
