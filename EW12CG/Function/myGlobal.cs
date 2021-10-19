using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12CG.Function {
    public class myGlobal {

        public static TestingInfo myTesting = new TestingInfo();
        public static SettingInfo mySetting = new SettingInfo();
        public static string dir_Path = AppDomain.CurrentDomain.BaseDirectory.Replace("EW12CG\\", "");
        public static string helpFileFullName = string.Format("{0}guide.xps", AppDomain.CurrentDomain.BaseDirectory);

        public static ObservableCollection<testItemInfo> datagridlogResult = new ObservableCollection<testItemInfo>() {
            new testItemInfo(){ itemName = "Firmware version", standardValue = mySetting.fwVersion, actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Firmware build time", standardValue = mySetting.fwBuildTime, actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Hardware version", standardValue = mySetting.hwVersion, actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Model number", standardValue = mySetting.modelNumber, actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Mac ethernet", standardValue = "", actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Mac wlan 2G", standardValue = "", actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Mac wlan 5G", standardValue = "", actualValue = "", itemResult = "" },
            new testItemInfo(){ itemName = "Serial number", standardValue = "Không phán định", actualValue = "", itemResult = "None" }
        };
    }
}
