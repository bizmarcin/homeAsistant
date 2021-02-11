using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homeAsistant
{
    public partial class Form1 : Form
    {
        List<PingModel> listOfResponse = new List<PingModel>();
        public Form1()
        {
            InitializeComponent();
        }

        private void findDevicesInNetwork_Click(object sender, EventArgs e)
        {           
            backgroundWorker1.RunWorkerAsync();
        }

        public List<PingModel> PingDevice(string pingAddress, List<PingModel> list)
        {
            //Method base on
            //https://docs.microsoft.com/pl-pl/dotnet/api/system.net.networkinformation.ping.send?view=net-5.0

            Ping pingSender = new Ping();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 200;
            PingOptions options = new PingOptions(64, true);

            // Send the request.
            PingReply reply = pingSender.Send(pingAddress, timeout, buffer, options);

            if (reply.Status == IPStatus.Success)
            {
                list.Add(new PingModel { address = reply.Address.ToString(), hostName=GetHostName(reply.Address.ToString()), roundtripTime = reply.RoundtripTime, ttl = reply.Options.Ttl, dontFragment = reply.Options.DontFragment, bufferLength = reply.Buffer.Length });
            }
            else
            {
                //do nothing
            }

            return list;
        }

        public string GetLocalIPAddress()
        {
            //Methd from
            //https://stackoverflow.com/questions/6803073/get-local-ip-address
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public List<string> GetIpAddressToCheck()
        {
            var list = new List<string>();
            var networkIpAddress = GetLocalIPAddress();
            var splitedNetworkIpAddress = networkIpAddress.Split('.');
            for(int i=1; i <= 255; i++)
            {
                list.Add($"{splitedNetworkIpAddress[0]}.{splitedNetworkIpAddress[1]}.{splitedNetworkIpAddress[2]}.{i}");
            }
            return list;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            toolStripStatusLabel1.Text = "Proggress:";
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel2.Text = "0 %";

            var ips = GetIpAddressToCheck();
            float proggressInPercent = 0;
            listOfResponse.Clear();
            float stepsNo = (float)100/(float)ips.Count();
            foreach (var ip in ips)
            {
                proggressInPercent += stepsNo;
                listOfResponse = PingDevice(ip, listOfResponse);
                backgroundWorker1.ReportProgress((int)proggressInPercent);
            }                        
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Visible = false;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel2.Visible = false;

            dataGridView1.DataSource = listOfResponse;
            dataGridView1.Refresh();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Visible = true;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel2.Visible = true;

            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = $"{e.ProgressPercentage}%";
        }

        public string GetHostName(string ipAddress)
        {
            //From
            //https://stackoverflow.com/questions/11123639/how-to-resolve-hostname-from-local-ip-in-c-net
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                //unknown host or
                //not every IP has a name
                //log exception (manage it)
            }

            return null;
        }
    }
}
