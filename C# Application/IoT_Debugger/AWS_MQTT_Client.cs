//using System.ComponentModel.Design;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//using Tools;

//internal class Program
//{
//    static AWS_MQTT_Client device;

//    private static void Main(string[] args)
//    {
//        device=new AWS_MQTT_Client();
//        device.connect();
//        while (device.connected)
//        {
//            Console.Write(DateTime.Now.ToString()+ ">");
//            string input = Console.ReadLine();
//            device.process_command(input);
//        }
//    }
//}// class
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Security.Cryptography.X509Certificates;
using System.Text;
using M2Mqtt;
using M2Mqtt.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.IotData;
using Amazon.IotData.Model;
using System.IO;
using System.Windows;
using System;
//////////////////////////////////////////////////////////////////////////////////
//From Nuget package manager add the below packages to run the MQTT module
//    a.	Newtonsoft.Json                     (by James Newton-King) 
//    b.	M2MqttClientDotnetcore              (by M2MqttClientDotnetCore1.0.1)
//////////////////////////////////////////////////////////////////////////////////
namespace Tools
{
    public class AWS_MQTT_Client
    {
        public string iotEndpoint= "aj5290-ats.iot.us-east-1.amazonaws.com";
        public string topic = "$aws/things/MyThing/shadow/name/MyShadow/update";    // "$aws/things/MyThing/shadow/name/MyShadow/get/accepted";
        public string password = "MyDevice@123";

        int brokerPort = 8883;
        public string Received_Message = "";
        public bool connected = false;
        public bool subscribed=false;
        public bool received=false;
        MqttClient client;
        public string value="";
        string jsonState;
        public int test_val = 0;

        static int count = 0;
        static string place = "bed_room";

        public AWS_MQTT_Client()
        {

            //if (!device.connected)
            //{
            //    device.connect();
            //    Console.WriteLine("Device connected with AWS ok...");
            //}
        }

        public AWS_MQTT_Client(string end_point,string topic,string password)
        {
            iotEndpoint=end_point;
            this.topic=topic;
            this.password=password;
        }

        public void connect()
        {
            try
            {
                string path = Path.Combine(System.IO.Directory.GetCurrent‌​Directory(), "AmazonRootCA1.pem"); 
                var caCert = X509Certificate.CreateFromCertFile(path);
                path = Path.Combine(System.IO.Directory.GetCurrent‌​Directory(), "device_certificate.cert.pfx");
                var clientCert = new X509Certificate2(path, password);

                client = new MqttClient(iotEndpoint, brokerPort, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);

                client.MqttMsgSubscribed += IotClient_MqttMsgSubscribed;
                client.MqttMsgPublishReceived += IotClient_MqttMsgPublishReceived;

                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                connected= true;
            }
            catch(Exception ex)
            {
                connected = false;
                subscribed= false;
            }
        }

        private  void IotClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Received_Message=Encoding.UTF8.GetString(e.Message);
            received=true;
        }

        private  void IotClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            subscribed= true;
        }

        public void send()
        {
            jsonState = "{\"state\":{\"desired\":{\"command\":\""+value+"\"}}}";

            if (connected)
            {
                byte[] Payload = Encoding.UTF8.GetBytes(jsonState);
                client.Publish(topic, Payload);
            }
        }

        public void send(string data)
        {
            string topic2 = "$aws/things/MyThing/shadow/name/MyShadow/get/desired";
            //   jsonState = "{\"state\":{\"desired\":{\"command\":\""+data+"\"}}}";

            if (connected)
            {
                byte[] Payload = Encoding.UTF8.GetBytes(data);
                client.Publish(topic2, Payload);
            }
        }

        public void test()
        {
            jsonState = "{\"state\":{\"desired\":{\"command\":\""+test_val.ToString()+"\"}}}";

            if (connected)
            {
                byte[] Payload = Encoding.UTF8.GetBytes(jsonState);
                client.Publish(topic, Payload);
                test_val++;
            }
        }

        public string Get_Data(string param="command")
        {
            dynamic jsonDe = JsonConvert.DeserializeObject(Received_Message);
            string s= jsonDe[param].ToString();
            return s;
        }

        public void process_command(string command)
        {
            //if ((command=="list")||(command=="ls")) { Print_Device_List(); return; }
            //var cmd = command.Split(' ');
            //if ((cmd[0]=="open")||(cmd[0]=="set")) { place= cmd[1]; Console.WriteLine("place set:"+cmd[1]); return; }
            //if ((cmd[0]=="close")||(cmd[0]=="stop")) { place= ""; Console.WriteLine("place reset:"+cmd[1]); return; }
            ///////////////////////////////////////////////////////////////////////////
            //if ((cmd[1]=="on") || (cmd[1]=="off")||(cmd[2]=="on") || (cmd[2]=="off")) power_state_update(cmd);
            //else if ((cmd[1]=="speed")||(cmd[1]=="brightness") ||(cmd[1]=="colour")) Write_Analog(cmd);
            //else if ((cmd[1]=="temp")||(cmd[1]=="humi")||(cmd[2]=="intensity")) Read_Analog(cmd);
            //else if ((cmd[1]=="knob")||(cmd[1]=="door")) Read_Digital(cmd);
            //else
            //{
            //    Console.WriteLine("Request Handler not found for"+ cmd);
            //}
        }

        public void power_state_update(string[] cmd)
        {
            string pin = Get_PIN_number(cmd[0]);

            string state = "1";

            for (int i = 0; i<cmd.Length; i++)
            {
                if (cmd[i]=="on")
                {
                    state = "0";
                }
            }
            ////////////////////
            JObject jsonObject = new JObject
            (
                new JProperty("device_id", Get_Device_Id(place)),
                        new JProperty("pin_name", cmd[0]),
                        new JProperty("pin_number", pin),
                        new JProperty("request_type", "set"),
                        new JProperty("parameter", "power_state"),
                        new JProperty("value", state)
            );
            string jsonString = jsonObject.ToString();
            send(jsonString);
        }

        public void Write_Analog(string[] cmd)
        {
            string pin = Get_PIN_number(cmd[0]);

            string state = "1";

            for (int i = 0; i<cmd.Length; i++)
            {
                if (cmd[i]=="on")
                {
                    state = "0";
                }
            }
            ////////////////////
            JObject jsonObject = new JObject
            (
                new JProperty("device_id", Get_Device_Id(place)),
                        new JProperty("pin_name", cmd[0]),
                        new JProperty("pin_number", pin),
                        new JProperty("request_type", "set"),
                        new JProperty("parameter", cmd[1]),
                        new JProperty("value", cmd[2])
            );
            string jsonString = jsonObject.ToString();
            send(jsonString);
        }

        public void Read_Analog(string[] cmd)
        {
            // device.send(str_tx+" "+state);
        }

        public void Read_Digital(string[] cmd)
        {
            // device.send(str_tx+" "+state);
        }

        public string Get_Device_Id(string room)
        {
            string id = "";

            switch (room)
            {
                case "bed_room":
                    id= "1";
                    break;
                /////////////
                case "kitchen":
                    id="2";
                    break;
                ///////////////
                case "dinning_space":
                    id="3";
                    break;
                /////////////
                case "bath_room":
                    id="4";
                    break;
                ////////////
                default:
                    break;
            }
            return id;
        }

        public string Get_PIN_number(string load_name)
        {
            if (place=="bed_room")
            {
                if (load_name=="tube") return "D23";
                if (load_name=="light") return "D22";
                if (load_name=="night") return "D21";
                if (load_name=="tv") return "D17";
                if (load_name=="ac") return "D16";
                if (load_name=="plug") return "D4";
                if (load_name=="fan") return "D13";
            }
            return "NA";
        }

        public void Print_Device_List()
        {
            //String lst = "The available Places are:\n";
            //lst+="________________________\n";
            //lst+="bed_room>tv,fan,bulb,night(lamp), ac, temp(erature), humi(dity)\n";
            //lst+="kitchen>bulb,micro wave,mixer(grinder),exhaust fan,knob(gas)\n";
            //lst+="dinning_space>tv,light,fan,plug,washing machine,light(get),door(get)\n";
            //lst+="bath_room>Geyser,Exhaust fan,bulb,plug,door(get)\n";
            //lst +="<End of List>\n";
            //Console.WriteLine(lst);
        }

        public void send_to_iot(string state)
        {
            try
            {
                JObject jsonObject = new JObject
            (
                  new JProperty("device_id", "2222"),
                         new JProperty("pin_name", "1"),
                         new JProperty("pin_number", "D1"),
                         new JProperty("request_type", "set"),
                         new JProperty("parameter", "power_state"),
                         new JProperty("value", "1")
             );
                string jsonString = jsonObject.ToString();


                //Access key: AKIAUA2XFJXXQCOGAYRB
                //Secret access key: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

                var iotDataClient = new AmazonIotDataClient("AKIAUA2XFJXXQCOGAYRB", "0qKq5T8VVKSZ7yErH2SLckgVp8D0d8nQs+v2Ah8x", "https://aj523zoiw6h90-ats.iot.us-east-1.amazonaws.com");

                var request = new PublishRequest
                {
                    Topic = "$aws/things/MyThing/shadow/name/MyShadow/get/accepted",
                    Payload = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)),
                    Qos = 0
                };

                var response = iotDataClient.PublishAsync(request).Result;             
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
        }













    }// class
}// Namespace
