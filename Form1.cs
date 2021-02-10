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
            var ips= GetIpAddressToCheck();
            listOfResponse.Clear();
            foreach(var ip in ips)
            {
                listOfResponse=PingDevice(ip,listOfResponse);
            }
            dataGridView1.DataSource = listOfResponse;
            dataGridView1.Refresh();
        }

        public List<PingModel> PingDevice(string pingAddress, List<PingModel> list)
        {
            //Method base on
            //https://docs.microsoft.com/pl-pl/dotnet/api/system.net.networkinformation.ping.send?view=net-5.0

            Ping pingSender = new Ping();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 100;
            PingOptions options = new PingOptions(64, true);

            // Send the request.
            PingReply reply = pingSender.Send(pingAddress, timeout, buffer, options);

            if (reply.Status == IPStatus.Success)
            {
                list.Add(new PingModel { address = reply.Address.ToString(), roundtripTime = reply.RoundtripTime, ttl = reply.Options.Ttl, dontFragment = reply.Options.DontFragment, bufferLength = reply.Buffer.Length });
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
    }
}
