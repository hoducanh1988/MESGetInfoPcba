using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using EW30CX.Function;
using System.Windows.Xps.Packaging;

namespace EW30CX {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }

        SSH imap = null;
        List<history> listHist = new List<history>();

        public MainWindow() {
            InitializeComponent();
            this.tab_runall.DataContext = myGlobal.myTesting;
            this.tab_setting.DataContext = myGlobal.mySetting;
            this.datagridResult.ItemsSource = myGlobal.datagridlogResult;
            this.DataContext = myGlobal.myAppInfo;

            //
            this.cbb_logtype.ItemsSource = new List<string>() { "log total", "log detail" };
            //
            listHist.Add(new history() {
                ID = "1",
                VERSION = "EW30CXVN0U0001",
                CONTENT = "- Xây dựng phần mềm đọc thông tin sản phẩm mesh EW30CX.",
                DATE = "28/09/2021",
                CHANGETYPE = "Tạo mới",
                PERSON = "Hồ Đức Anh"
            });

            this.GridAbout.ItemsSource = listHist;

            //help
            if (System.IO.File.Exists(myGlobal.helpFileFullName)) {
                XpsDocument xpsDocument = new XpsDocument(myGlobal.helpFileFullName, System.IO.FileAccess.Read);
                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                _docViewer.Document = fds;
            }

        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            TextBox tb_mac = sender as TextBox;

            if (e.Key == Key.Enter) {
                string mac = tb_mac.Text;

                Thread t = new Thread(new ThreadStart(() => {
                    Stopwatch st = new Stopwatch();
                    st.Start();
                    Dispatcher.Invoke(new Action(() => { tb_mac.IsEnabled = false; }));
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                    myGlobal.myTesting.initParams();
                    myGlobal.myTesting.waitParams(mac);
                    bool ___ = runAll(mac) ? myGlobal.myTesting.passedParams() : myGlobal.myTesting.failedParams();

                    st.Stop();
                    myGlobal.myTesting.sshLog += string.Format("+++++++++++++++++++++++++\r\n");
                    myGlobal.myTesting.sshLog += string.Format("Total time: {0} ms\r\n", st.ElapsedMilliseconds);
                    //save log
                    var log = new LogFile();
                    log.saveLogDetail();
                    log.saveLogTotal();
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                    Dispatcher.Invoke(new Action(() => { tb_mac.Clear(); tb_mac.IsEnabled = true; tb_mac.Focus(); }));
                }));
                t.IsBackground = true;
                t.Start();
            }
        }


        private bool runAll(string mac_stamp) {
            try {
                bool r = false;
                bool total_result = true;

                resetDataGrid();
                updateSettingToDataGrid();

                //check mac stamp
                r = UtilityPack.Validation.Parse.IsMacAddress(mac_stamp);
                if (!r) {
                    myGlobal.myTesting.errorMessage += string.Format("Địa chỉ mac \"{0}\" sai định dạng\n", mac_stamp);
                    total_result = false;
                    goto END;
                }

                //ping network to imap
                r = _pingIMAP(myGlobal.mySetting.ipAddress, 10);
                if (!r) {
                    myGlobal.myTesting.errorMessage += string.Format("Không ping được mạng tới IMAP {0}\n", myGlobal.mySetting.ipAddress);
                    total_result = false;
                    goto END;
                }

                //login ssh to imap
                r = _loginSSH(myGlobal.mySetting.ipAddress, myGlobal.mySetting.sshUser, myGlobal.mySetting.sshPassword, 3);
                if (!r) {
                    myGlobal.myTesting.errorMessage += string.Format("Không login SSH được vào IMAP\n");
                    total_result = false;
                    goto END;
                }

                //check firmware verion
                r = _get_firmwareversion(3);
                if (!r) total_result = false;

                //check firmware build time
                r = _get_firmwarebuildtime(3);
                if (!r) total_result = false;

                //check model number
                r = _get_hversion_modelnumber_macEthernet(3);
                if (!r) total_result = false;

                //check mac wifi 2G
                r = _get_mac_wifi_2G(3);
                if (!r) total_result = false;

                //check mac wifi 5G
                r = _get_mac_wifi_5G(3);
                if (!r) total_result = false;

                //check serial number
                _get_serialnumber(3);

            END:
                return total_result;
            }
            catch { return false; }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string b_content = (string)b.Content;

            switch (b_content.ToLower()) {
                case "go": {
                        try {
                            switch (cbb_logtype.SelectedItem.ToString()) {
                                case "log total": {
                                        try {
                                            Process.Start(string.Format("{0}Logtotal", myGlobal.dir_Path));
                                        }
                                        catch {
                                            Process.Start(string.Format("{0}", myGlobal.dir_Path));
                                        }
                                        break;
                                    }
                                case "log detail": {
                                        try {
                                            Process.Start(string.Format("{0}Logdetail\\{1}", myGlobal.dir_Path, DateTime.Now.ToString("yyyy-MM-dd")));
                                        }
                                        catch {
                                            Process.Start(string.Format("{0}", myGlobal.dir_Path));
                                        }
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex) {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                default: break;
            }


        }

        #region sub-function

        private void resetDataGrid() {
            foreach (var item in myGlobal.datagridlogResult) {
                item.itemResult = "";
                item.actualValue = "";
                item.standardValue = "";
            }
        }

        private void updateSettingToDataGrid() {
            myGlobal.datagridlogResult[0].standardValue = myGlobal.mySetting.fwVersion;
            myGlobal.datagridlogResult[1].standardValue = myGlobal.mySetting.fwBuildTime;
            myGlobal.datagridlogResult[2].standardValue = myGlobal.mySetting.hwVersion;
            myGlobal.datagridlogResult[3].standardValue = myGlobal.mySetting.modelNumber;
            myGlobal.datagridlogResult[7].standardValue = "Không phán định";
            myGlobal.datagridlogResult[7].itemResult = "None";
        }

        private string GetMAC2G(string mac) {
            string hexMAC = "FAIL";
            try {
                int num = Int32.Parse(mac, System.Globalization.NumberStyles.HexNumber);
                num = num + 1;
                hexMAC = num.ToString("X").Trim();
                int hexMAC_len = hexMAC.Length;
                if (hexMAC_len < 6) {
                    for (int i = 0; i < 6 - hexMAC_len; i++) {
                        hexMAC = "0" + hexMAC;
                    }
                }
                else
                    if (hexMAC_len == 7) {
                    hexMAC = hexMAC.Substring(0, 6);
                }
            }
            catch { }

            return hexMAC;
        }

        private string GetMAC5G(string mac) {
            string hexMAC = "FAIL";
            try {
                int num = Int32.Parse(mac, System.Globalization.NumberStyles.HexNumber);
                num = num + 2;
                hexMAC = num.ToString("X").Trim();
                int hexMAC_len = hexMAC.Length;
                if (hexMAC_len < 6) {
                    for (int i = 0; i < 6 - hexMAC_len; i++) {
                        hexMAC = "0" + hexMAC;
                    }
                }
                else
                    if (hexMAC_len == 7) {
                    hexMAC = hexMAC.Substring(0, 6);
                }
            }
            catch { }

            return hexMAC;
        }

        private bool pingNetwork(string ip) {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            // Use the default Ttl value which is 128, 
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000;

            try {
                PingReply reply = pingSender.Send(ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        private bool _pingIMAP(string ip, int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format(">>> Ping to {0}\r\n", ip);
            myGlobal.myTesting.sshLog += string.Format("Waiting.");
        RE_PING:
            count++;
            myGlobal.myTesting.sshLog += string.Format("..{0}", count);
            r = pingNetwork(ip);
            if (!r) {
                if (count < retry_time) {
                    Thread.Sleep(100);
                    goto RE_PING;
                }
            }
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }

        private bool _loginSSH(string ip, string ssh_user, string ssh_pass, int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>> Login SSH to {0}\r\n", ip);
            myGlobal.myTesting.sshLog += string.Format("Waiting.");
            imap = new SSH();
        RE_LOGIN:
            count++;
            myGlobal.myTesting.sshLog += string.Format("..{0}", count);
            r = imap.Login(ip, ssh_user, ssh_pass);
            if (!r) {
                if (count < retry_time) {
                    Thread.Sleep(100);
                    goto RE_LOGIN;
                }
            }
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }


        private bool _get_firmwareversion(int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get firmware version\r\n");
            imap.WriteLine("");
            Thread.Sleep(500);
            imap.Read();

        RE:
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("cat /etc/fw_info");
            Thread.Sleep(100);
            myGlobal.myTesting.sshLog += string.Format("send command: cat /etc/fw_info\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }
            string[] buffer = data.Split('\n');
            foreach (var x in buffer) {
                if (x.ToLower().Contains("firmware version")) {
                    myGlobal.myTesting.fwVersion = x.Split(':')[1].Replace("\r", "").Trim().Replace("_", "__");
                    r = true;
                    break;
                }
            }

            //check firmware verion
            if (string.IsNullOrEmpty(myGlobal.myTesting.fwVersion) ||
               string.IsNullOrEmpty(myGlobal.mySetting.fwVersion)) {
                //myGlobal.myTesting.errorMessage += string.Format("Firmware version không đúng với setting.\n");
                r = false;
            }
            else {
                r = myGlobal.myTesting.fwVersion.ToLower().Replace("__", "_").Equals(myGlobal.mySetting.fwVersion.ToLower());
                //if (!r) {
                //    myGlobal.myTesting.errorMessage += string.Format("Firmware version không đúng với setting.\n");
                //}
            }

            //update grid
            myGlobal.datagridlogResult[0].actualValue = myGlobal.myTesting.fwVersion.Replace("__", "_");
            myGlobal.datagridlogResult[0].itemResult = r == true ? "Passed" : "Failed";

            //
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }


        private bool _get_firmwarebuildtime(int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get firmware build time\r\n");
        RE:
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("cat /proc/version");
            Thread.Sleep(100);
            myGlobal.myTesting.sshLog += string.Format("send command: cat /proc/version\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }

            string[] buffer = data.Split('\n');
            foreach (var x in buffer) {
                if (x.ToLower().Contains("gcc version")) {
                    myGlobal.myTesting.fwBuildTime = x.Split(new string[] { ") )" }, StringSplitOptions.None)[1].Replace("\r", "").Trim();
                    r = true;
                    break;
                }
            }

            //check firmware build time
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.fwBuildTime) ||
                string.IsNullOrWhiteSpace(myGlobal.mySetting.fwBuildTime)) {
                r = false;
            }
            else {
                r = myGlobal.myTesting.fwBuildTime.ToLower().Equals(myGlobal.mySetting.fwBuildTime.ToLower());
            }

            //update grid
            myGlobal.datagridlogResult[1].actualValue = myGlobal.myTesting.fwBuildTime.Replace("__", "_");
            myGlobal.datagridlogResult[1].itemResult = r == true ? "Passed" : "Failed";

            //
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }


        private bool _get_hversion_modelnumber_macEthernet(int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get Hardware version, model number, mac Ethernet\r\n");
        RE:
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("fw_printenv");
            Thread.Sleep(500);
            myGlobal.myTesting.sshLog += string.Format("send command: fw_printenv\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }
            r = data.Contains("ethaddr=") && data.Contains("hardwareversion=") && data.Contains("modelnumber=");
            if (!r) { if (count < retry_time) goto RE; }
            else {
                string[] buffer = data.Split('\n');
                int max = buffer.Length - 1;
                for (int i = max; i >= 0; i--) {
                    string s = buffer[i];
                    if (s.Contains("modelnumber=")) {
                        myGlobal.myTesting.modelNumber = s.Replace("modelnumber=", "").Replace("\r", "").Replace("\n", "").Trim();
                    }
                    if (s.Contains("hardwareversion=")) {
                        myGlobal.myTesting.hwVersion = s.Replace("hardwareversion=", "").Replace("\r", "").Replace("\n", "").Trim();
                    }
                    if (s.Contains("ethaddr=")) {
                        myGlobal.myTesting.macEthernet = s.Replace("ethaddr=", "").Replace("\r", "").Replace("\n", "").Trim();
                        break;
                    }
                }
            }

            bool r1 = false, r2 = false, r3 = false;

            //check hardware version
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.hwVersion) ||
                string.IsNullOrWhiteSpace(myGlobal.mySetting.hwVersion)) {
                r2 = false;
            }
            else {
                r2 = myGlobal.myTesting.hwVersion.ToLower().Equals(myGlobal.mySetting.hwVersion.ToLower());
            }

            //update grid
            myGlobal.datagridlogResult[2].actualValue = myGlobal.myTesting.hwVersion;
            myGlobal.datagridlogResult[2].itemResult = r2 == true ? "Passed" : "Failed";


            //check model number
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.modelNumber) ||
                string.IsNullOrWhiteSpace(myGlobal.mySetting.modelNumber)) {
                r1 = false;
            }
            else {
                r1 = myGlobal.myTesting.modelNumber.ToLower().Equals(myGlobal.mySetting.modelNumber.ToLower());
            }

            //update grid
            myGlobal.datagridlogResult[3].actualValue = myGlobal.myTesting.modelNumber;
            myGlobal.datagridlogResult[3].itemResult = r1 == true ? "Passed" : "Failed";


            //check mac ethernet
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.macEthernet)) {
                r3 = false;
            }
            else {
                r3 = myGlobal.myTesting.macEthernet.Replace(":", "").ToLower().Equals(myGlobal.myTesting.macStamp.ToLower());
            }

            //update grid
            myGlobal.datagridlogResult[4].standardValue = myGlobal.myTesting.macStamp;
            myGlobal.datagridlogResult[4].actualValue = myGlobal.myTesting.macEthernet.Replace(":", "");
            myGlobal.datagridlogResult[4].itemResult = r3 == true ? "Passed" : "Failed";

            r = r1 && r2 && r3;

            //
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }


        private bool _get_mac_wifi_2G(int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get mac wifi 2G\r\n");
        RE:
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("hexdump /dev/mtd5 | grep 0001000");
            Thread.Sleep(1000);
            myGlobal.myTesting.sshLog += string.Format("send command: hexdump /dev/mtd5 | grep 0001000\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }
            r = data.Contains("hexdump /dev/mtd5 | grep 0001000");
            if (!r) { if (count < retry_time) goto RE; }
            else {
                string[] buffer = data.Split('\n');
                int max = buffer.Length - 1;
                for (int i = 0; i <= max; i++) {
                    string s = buffer[i];
                    if (s.Contains("hexdump /dev/mtd5 | grep 0001000")) {
                        string sss = buffer[i + 1].Replace("\r", "").Replace("\n", "").Trim().ToUpper().Substring(13, 14).Replace(" ", "");
                        myGlobal.myTesting.macWifi2G = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", sss.Substring(0, 2), sss.Substring(2, 2), sss.Substring(4, 2), sss.Substring(6, 2), sss.Substring(8, 2), sss.Substring(10, 2));
                        break;
                    }
                }
            }

            string mac_2g = "";
            string mac_ethernet = "";
            //check mac wifi 2G
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.macWifi2G)) {
                //myGlobal.myTesting.errorMessage += string.Format("Mac wifi 2G sai định dạng.\n");
                r = false;
            }
            else {
                if (string.IsNullOrWhiteSpace(myGlobal.myTesting.macEthernet) == false) {
                    mac_ethernet = myGlobal.myTesting.macStamp; //myGlobal.myTesting.macEthernet.Replace(":", "").Replace("-", "");
                    mac_2g = mac_ethernet.Substring(0, 6) + GetMAC2G(mac_ethernet.Substring(6, 6));
                    r = mac_2g.ToLower().Equals(myGlobal.myTesting.macWifi2G.Replace(":", "").Replace("-", "").ToLower());
                    //if (!r) {
                    //   myGlobal.myTesting.errorMessage += string.Format("Mac wifi 2G không đúng.\n");
                    //}
                }
            }

            //update grid
            myGlobal.datagridlogResult[5].standardValue = mac_2g;
            myGlobal.datagridlogResult[5].actualValue = myGlobal.myTesting.macWifi2G.Replace(":", "").Replace("-", "");
            myGlobal.datagridlogResult[5].itemResult = r == true ? "Passed" : "Failed";

            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }

        private bool _get_mac_wifi_5G(int retry_time) {
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get mac wifi 5G\r\n");
        RE:
            //imap.WriteLine("hexdump /dev/mtd5 | grep 5000");
            //Thread.Sleep(1000);
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("hexdump /dev/mtd5 | grep 0005000");
            Thread.Sleep(1000);
            myGlobal.myTesting.sshLog += string.Format("send command: hexdump /dev/mtd5 | grep 0005000\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }
            r = data.Contains("hexdump /dev/mtd5 | grep 0005000");
            if (!r) { if (count < retry_time) goto RE; }
            else {
                string[] buffer = data.Split('\n');
                int max = buffer.Length - 1;
                for (int i = 0; i <= max; i++) {
                    string s = buffer[i];
                    if (s.Contains("hexdump /dev/mtd5 | grep 0005000")) {
                        string sss = buffer[i + 1].Replace("\r", "").Replace("\n", "").Trim().ToUpper().Substring(23, 14).Replace(" ", "");
                        myGlobal.myTesting.macWifi5G = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", sss.Substring(0, 2), sss.Substring(2, 2), sss.Substring(4, 2), sss.Substring(6, 2), sss.Substring(8, 2), sss.Substring(10, 2));
                        break;
                    }
                }
            }

            string mac_ethernet = "";
            string mac_5g = "";

            //check mac wifi 5G
            if (string.IsNullOrWhiteSpace(myGlobal.myTesting.macWifi5G)) {
                //myGlobal.myTesting.errorMessage += string.Format("Mac wifi 5G sai định dạng.\n");
                r = false;
            }
            else {
                if (string.IsNullOrWhiteSpace(myGlobal.myTesting.macEthernet) == false) {
                    mac_ethernet = myGlobal.myTesting.macStamp; //myGlobal.myTesting.macEthernet.Replace(":", "").Replace("-", "");
                    mac_5g = mac_ethernet.Substring(0, 6) + GetMAC5G(mac_ethernet.Substring(6, 6));
                    r = mac_5g.ToLower().Equals(myGlobal.myTesting.macWifi5G.Replace(":", "").Replace("-", "").ToLower());
                    //if (!r) {
                    //    myGlobal.myTesting.errorMessage += string.Format("Mac wifi 5G không đúng.\n");
                    //}
                }
            }

            //update grid
            myGlobal.datagridlogResult[6].standardValue = mac_5g;
            myGlobal.datagridlogResult[6].actualValue = myGlobal.myTesting.macWifi5G.Replace(":", "").Replace("-", "");
            myGlobal.datagridlogResult[6].itemResult = r == true ? "Passed" : "Failed";

            //
            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }

        private bool _get_serialnumber(int retry_time) {
            //## Error: \"serialnumber\" not defined
            int count = 0;
            retry_time = retry_time <= 0 ? 1 : retry_time;
            bool r = false;
            myGlobal.myTesting.sshLog += string.Format("\r\n>>>  Get serial number\r\n");
        RE:
            count++;
            myGlobal.myTesting.sshLog += string.Format("retry {0}\r\n", count);
            imap.WriteLine("fw_printenv serialnumber");
            Thread.Sleep(1000);
            myGlobal.myTesting.sshLog += string.Format("send command: fw_printenv serialnumber\r\n");
            string data = imap.Read();
            myGlobal.myTesting.sshLog += string.Format("feedback: {0}\r\n", data);
            if (string.IsNullOrEmpty(data)) { if (count < retry_time) goto RE; }
            r = !data.Contains("## Error: \"serialnumber\" not defined");
            if (!r) { if (count < retry_time) goto RE; }
            else {
                string[] buffer = data.Split('\n');
                int max = buffer.Length - 1;
                for (int i = 0; i <= max; i++) {
                    string s = buffer[i];
                    if (s.Contains("fw_printenv serialnumber")) {
                        myGlobal.myTesting.serialNumber = buffer[i + 1].Replace("serialnumber=", "").Replace("\r", "").Replace("\n", "").Trim().ToUpper();
                        break;
                    }
                }
            }

            //update grid
            myGlobal.datagridlogResult[7].actualValue = myGlobal.myTesting.serialNumber;

            myGlobal.myTesting.sshLog += string.Format("\r\n");
            myGlobal.myTesting.sshLog += string.Format("Result: {0}\r\n", r == true ? "Passed" : "Failed");

            return r;
        }


        #endregion


    }
}
