#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <TM1637Display.h>

#define STASSID "Your_WIFI_ID"
#define STAPSK  "Your_WIFI_PW"

#define cpuCLK D5
#define cpuDIO D6
#define memCLK D7
#define memDIO D8
#define gpuCLK D2
#define gpuDIO D14

#define JP D0
#define KR D10
#define EN D15

TM1637Display cpudisplay(cpuCLK, cpuDIO);
TM1637Display memdisplay(memCLK, memDIO);
TM1637Display gpudisplay(gpuCLK, gpuDIO);

unsigned long lastPacketTime = 0;
unsigned int localPort = 8888;      // local port to listen on

// buffers for receiving and sending data
char packetBuffer[UDP_TX_PACKET_MAX_SIZE + 1]; //buffer to hold incoming packet,

WiFiUDP Udp;

void setup() {
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(STASSID, STAPSK);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print('.');
    delay(500);
  }
  Serial.print("Connected! IP address: ");
  Serial.println(WiFi.localIP());
  Serial.printf("UDP server on port %d\n", localPort);
  Udp.begin(localPort);

  cpudisplay.setBrightness(7);
  cpudisplay.showNumberDec(0,false,4);
  memdisplay.setBrightness(7);
  memdisplay.showNumberDec(0,false,4);
  gpudisplay.setBrightness(7);
  gpudisplay.showNumberDec(0,false,4);

  lastPacketTime = millis();

  pinMode(EN, OUTPUT);
  pinMode(KR, OUTPUT);
  pinMode(JP, OUTPUT);
}

void loop() {
  // if there's data available, read a packet
  unsigned long timeSinceLastPacket = millis() - lastPacketTime;
  
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    Serial.printf("Received packet of size %d from %s:%d\n    (to %s:%d, free heap = %d B)\n",
                  packetSize,
                  Udp.remoteIP().toString().c_str(), Udp.remotePort(),
                  Udp.destinationIP().toString().c_str(), Udp.localPort(),
                  ESP.getFreeHeap());

    // read the packet into packetBufffer
    int n = Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
    packetBuffer[n] = 0;
    Serial.println("Contents:");
    Serial.println(packetBuffer);

    cpudisplay.showNumberDec(((packetBuffer[2]-48)*1000) + ((packetBuffer[3]-48)*100) + ((packetBuffer[7]-48)*10) + packetBuffer[8]-48, 4);
    memdisplay.showNumberDec(((packetBuffer[12]-48)*1000) + ((packetBuffer[13]-48)*100) + ((packetBuffer[17]-48)*10) + packetBuffer[18]-48, 4);
    gpudisplay.showNumberDec(((packetBuffer[27]-48)*1000) + ((packetBuffer[28]-48)*100) + ((packetBuffer[22]-48)*10) + packetBuffer[23]-48, 4);
    
    digitalWrite(EN, LOW);
    digitalWrite(KR, LOW);
    digitalWrite(JP, LOW);

    char lastChar = packetBuffer[30];
    Serial.println(lastChar); //input lang mode

    switch(lastChar) {
      case 'E':
        Serial.println("Received 'E'");
        digitalWrite(EN, HIGH);
        break;
      case 'K':
        Serial.println("Received 'K'");
        digitalWrite(KR, HIGH);
        break;
      case 'J':
        Serial.println("Received 'J'");
        digitalWrite(JP, HIGH);
        break;
      default:
        Serial.println("Received unknown value");
        break;
    }
    lastPacketTime = millis();
  } 
    if (timeSinceLastPacket > 10000) {
    cpudisplay.clear();
    memdisplay.clear();
    gpudisplay.clear();

    digitalWrite(EN, LOW);
    digitalWrite(KR, LOW);
    digitalWrite(JP, LOW);
  } else {
    cpudisplay.setBrightness(7);  // Set display brightness to 7 (maximum)
    memdisplay.setBrightness(7);  // Set display brightness to 7 (maximum)
    gpudisplay.setBrightness(7);  // Set display brightness to 7 (maximum)
  }
}
//0000,0000,0000
/*
  test (shell/netcat):
  --------------------
    nc -u 192.168.esp.address 8888
*/