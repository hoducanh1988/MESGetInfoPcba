using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace EW30SX.Function {
    public class LogFile {

        string dir_log_total = string.Format("{0}Logtotal", myGlobal.dir_Path);
        string dir_log_detail = string.Format("{0}Logdetail\\{1}", myGlobal.dir_Path, DateTime.Now.ToString("yyyy-MM-dd"));

        public LogFile() {
            if (Directory.Exists(dir_log_total) == false) Directory.CreateDirectory(dir_log_total);
            if (Directory.Exists(dir_log_detail) == false) Directory.CreateDirectory(dir_log_detail);
        }

        private void getSettingInfo(ref string str) {
            str += "SETTING INFO\n";
            PropertyInfo[] propers = myGlobal.mySetting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in propers) {
                string data = $"{p.Name}={p.GetValue(myGlobal.mySetting, null)}";
                str += $"{data}\n";
            }
            str += "+++++++++++++++++++++++++++++++++++++++++\n";
        }

        private void getAppInfo(ref string str) {
            str += "APP INFO\n";
            str += $"{myGlobal.myAppInfo.appInfo}\n";
            str += "+++++++++++++++++++++++++++++++++++++++++\n";
        }

        public bool saveLogDetail() {
            try {
                string log_data = "";
                getAppInfo(ref log_data);
                getSettingInfo(ref log_data);
                string f = string.Format("{0}\\EW30SX_{1}_{2}_{3}.txt", dir_log_detail, myGlobal.myTesting.macStamp, DateTime.Now.ToString("hhmmss"), myGlobal.myTesting.totalResult);
                using (var sw = new StreamWriter(f, true, Encoding.Unicode)) {
                    sw.WriteLine("Product: EW30SX");
                    sw.WriteLine(log_data);
                    sw.WriteLine(myGlobal.myTesting.sshLog);
                }
                return true;
            } catch { return false; }
        }

        public bool saveLogTotal() {
            try {
                string f = string.Format("{0}\\EW30SX_{1}.csv", dir_log_total, DateTime.Now.ToString("yyyyMMdd"));
                string f_title = "\"DateTimeCreated\",\"MacStamp\",\"fwVersion\",\"fwBuildTime\",\"hwVersion\",\"modelNumber\",\"macEthernet\",\"macWlan2G\",\"macWlan5G\",\"serialNumber\",\"Result\"";
                string f_content = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"", 
                                                 DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                                 myGlobal.myTesting.macStamp,
                                                 myGlobal.myTesting.fwVersion.Replace("__", "_"),
                                                 myGlobal.myTesting.fwBuildTime,
                                                 myGlobal.myTesting.hwVersion,
                                                 myGlobal.myTesting.modelNumber,
                                                 myGlobal.myTesting.macEthernet,
                                                 myGlobal.myTesting.macWifi2G,
                                                 myGlobal.myTesting.macWifi5G,
                                                 myGlobal.myTesting.serialNumber,
                                                 myGlobal.myTesting.totalResult);

                if (File.Exists(f) == false) {
                    using (var sw = new StreamWriter(f, true, Encoding.Unicode)) {
                        sw.WriteLine(f_title);
                        sw.WriteLine(f_content);
                    }
                }
                else {
                    using (var sw = new StreamWriter(f, true, Encoding.Unicode)) {
                        sw.WriteLine(f_content);
                    }
                }

                return true;
            } catch { return false; }
        }


    }
}
