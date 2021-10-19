using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EW12S.Function {

    public class SSH {

        private ShellStream shellStreamSSH;
        private SshClient sshClient;

        public bool Login(string IPAddress, string Username, string Pass) {
            try {
                this.sshClient = new SshClient(IPAddress, 22, Username, Pass);

                //Thực hiện kết nối
                this.sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                this.sshClient.Connect();

                // tạo shell stream để điều khiển command ssh
                this.shellStreamSSH = this.sshClient.CreateShellStream("vt100", 80, 60, 800, 600, 65535);

                return true;
            }
            catch {
                return false;
            }
        }


        public void Write(string cmd) {
            this.shellStreamSSH.Write(cmd);
            this.shellStreamSSH.Flush();
        }

        public void WriteLine(string cmd) {
            this.Write(cmd + "\r\n");
        }

        public string Read() {
            string value = "NULL";
            //Thread.Sleep(500);
            try {
                //Thread.Sleep(500);
                value = shellStreamSSH.Read();
                //Thread.Sleep(500);
                return value;
            }
            catch {
                return value;
            };
        }

        [Obsolete("No use this method for read data from SSH Shell Stream. Replace by 'Read()' method.", true)]
        public string recvSSHData() {
            while (true) {
                try {
                    if (this.shellStreamSSH != null && this.shellStreamSSH.DataAvailable) {
                        string strData = this.shellStreamSSH.Read();
                        Thread.Sleep(500);
                        return strData;
                    }
                }
                catch {
                    System.Windows.MessageBox.Show("Không đọc được dữ liệu từ thiết bị");
                    return "ERROR";
                }
            }
        }

        public void Disconnect() {
            if (this.sshClient != null) this.sshClient.Disconnect();
        }

        public void Close() {
            if (this.sshClient != null) this.sshClient.Dispose();
        }

        public void CloseShellStream() {
            if (this.shellStreamSSH != null) this.shellStreamSSH.Close();
        }

    }

}
