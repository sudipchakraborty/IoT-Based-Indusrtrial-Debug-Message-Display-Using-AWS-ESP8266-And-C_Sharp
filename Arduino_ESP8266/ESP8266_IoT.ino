// https://www.youtube.com/watch?v=x9GfxgkEpXg&t=16s
// https://how2electronics.com/connecting-esp8266-to-amazon-aws-iot-core-using-mqtt/ 

#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <time.h>
#include "secrets.h"

float h ;
float t;
unsigned long lastMillis = 0;
unsigned long previousMillis = 0;
const long interval = 5000;
 
#define AWS_IOT_PUBLISH_TOPIC   "$aws/things/MyThing/shadow/name/MyShadow/update"
#define AWS_IOT_SUBSCRIBE_TOPIC "$aws/things/MyThing/shadow/name/MyShadow/get/accepted"
 
WiFiClientSecure net;
 
BearSSL::X509List cert(cacert);
BearSSL::X509List client_crt(client_cert);
BearSSL::PrivateKey key(privkey);
 
PubSubClient client(net);
 
time_t now;
time_t nowish = 1510592825;
//______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void setup()
{  
  Serial.begin(9600);
  connectAWS();
}
//______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void loop()
{
   now = time(nullptr);
 
  if (!client.connected())
  {
    connectAWS();
  }
  else
  {
    client.loop();

    if (Serial.available())
    {
      String teststr = Serial.readString();  //read until timeout
      teststr.trim();  
   //   Serial.println(teststr);

      StaticJsonDocument<200> doc;
      doc["message"] = teststr; 
      char jsonBuffer[512];
      serializeJson(doc, jsonBuffer);  
      client.publish(AWS_IOT_PUBLISH_TOPIC, jsonBuffer);
    } 




    // if (millis() - lastMillis > 5000)
    // {
    //   lastMillis = millis();
    //   publishMessage();
    //   Serial.print("Message Published.\r\n.");
    // }
  }
}
//______________________________________________________________________________________________________________________________________________________________________________________________________________________________

 


 //______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void NTPConnect(void)
{
  Serial.print("Setting time using SNTP");
  configTime(TIME_ZONE * 3600, 0 * 3600, "pool.ntp.org", "time.nist.gov");
  now = time(nullptr);
  while (now < nowish)
  {
    delay(500);
    Serial.print(".");
    now = time(nullptr);
  }
  Serial.println("done!");
  struct tm timeinfo;
  gmtime_r(&now, &timeinfo);
  Serial.print("Current time: ");
  Serial.print(asctime(&timeinfo));
}
 //______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void messageReceived(char *topic, byte *payload, unsigned int length)
{
  Serial.print("Received [");
  Serial.print(topic);
  Serial.print("]: ");
  for (int i = 0; i < length; i++)
  {
    Serial.print((char)payload[i]);
  }
  Serial.println();
}
//______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void connectAWS()
{
  delay(3000);
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
 
  Serial.println(String("Attempting to connect to SSID: ") + String(WIFI_SSID));
 
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print(".");
    delay(1000);
  }
 
  NTPConnect();
 
  net.setTrustAnchors(&cert);
  net.setClientRSACert(&client_crt, &key);
 
  client.setServer(MQTT_HOST, 8883);
  client.setCallback(messageReceived);
 
 
  Serial.println("Connecting to AWS IOT");
 
  while (!client.connect(THINGNAME))
  {
    Serial.print(".");
    delay(1000);
  }
 
  if (!client.connected()) {
    Serial.println("AWS IoT Timeout!");
    return;
  }
  // Subscribe to a topic
  client.subscribe(AWS_IOT_SUBSCRIBE_TOPIC);
 
  Serial.println("AWS IoT Connected!");
}
 //______________________________________________________________________________________________________________________________________________________________________________________________________________________________
void publishMessage()
{
  StaticJsonDocument<200> doc;
  doc["time"] = millis();
  doc["humidity"] = h;
  doc["temperature"] = t;
  char jsonBuffer[512];
  serializeJson(doc, jsonBuffer); // print to client
 
  client.publish(AWS_IOT_PUBLISH_TOPIC, jsonBuffer);
}
//______________________________________________________________________________________________________________________________________________________________________________________________________________________________
