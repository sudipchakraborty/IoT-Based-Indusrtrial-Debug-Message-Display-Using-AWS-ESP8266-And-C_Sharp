using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace IoT_Debugger
{
    public partial class frm_main : Form
    {
        AWS_MQTT_Client client;
        MSG msg;

        public frm_main()
        {
            InitializeComponent();
            client = new AWS_MQTT_Client();
            msg= new MSG(lst_msg);
        }

        private void frm_main_Load(object sender, EventArgs e)
        {
            client.connect();
            if(client.connected)
            {
                msg.push("Client Connected");
            }
            else
            {
                msg.push("Unable to connect with AWS cloud!!!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(client.received)
            {
              string s=  client.Get_Data("message");
              msg.push(s);
              client.received = false;
            }
        }
    }
}
