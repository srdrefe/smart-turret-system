#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <Servo.h>

const int trigPin = 6;
const int echoPin = 7;
const int laserPin = 8;
const int panPin = 9;
const int tiltPin = 10;
const int buzzerPin = 11;

Servo panServo;
Servo tiltServo;
LiquidCrystal_I2C lcd(0x27, 16, 2);

long duration;
int distance;

// Devriye Ayarları
unsigned long lastSerialTime = 0;       
const unsigned long idleTimeout = 2000; 

int currentPanAngle = 90;               
int sweepStep = 1;                      
unsigned long lastSweepTime = 0;        
const int sweepInterval = 30;           

void setup() {
  Serial.begin(115200); 
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  pinMode(laserPin, OUTPUT);
  pinMode(buzzerPin, OUTPUT);

  panServo.attach(panPin);
  tiltServo.attach(tiltPin);
  panServo.write(90);
  tiltServo.write(90);

  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("SISTEM AKTIF");
  delay(1500);
  lcd.clear();
}

void loop() {
  // 1. Gelişmiş Seri Port Okuma (Donmaları Engeller)
  if (Serial.available() > 0) {
    String data = Serial.readStringUntil('\n'); // 'Enter' gelene kadar oku
    data.trim(); // Varsa sağdaki soldaki boşlukları sil
    
    if (data.length() > 0) {
      parseData(data);
    }
  }

  // 2. Devriye Modu Kontrolü
  if (millis() - lastSerialTime > idleTimeout) {
    sweepArea();
  }

  // 3. Mesafe Ölçümü
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  duration = pulseIn(echoPin, HIGH, 30000); 
  
  if (duration == 0) {
    distance = 999; 
  } else {
    distance = duration * 0.034 / 2;
  }

  // 4. Ekranı Güncelle
  updateDisplay();

  // 5. Ateşleme
  if (distance > 0 && distance < 30) {
    fireSequence();
    lastSerialTime = millis(); 
  }
}

// Koordinatları Güvenli Şekilde Ayırma
void parseData(String data) {
  data.toUpperCase(); // x90y45 gelse bile X90Y45 yapar (Hayat kurtarır)
  
  int xIndex = data.indexOf('X');
  int yIndex = data.indexOf('Y');
  
  // Sadece gerçek bir koordinat geldiyse motorları çevir ve sayacı sıfırla
  if (xIndex != -1 && yIndex != -1) {
    int xVal = data.substring(xIndex + 1, yIndex).toInt();
    int yVal = data.substring(yIndex + 1).toInt();
    
    // Değerleri servo sınırlarına (0-180) kilitle ki motorlar zorlanmasın
    xVal = constrain(xVal, 0, 180);
    yVal = constrain(yVal, 0, 180);
    
    currentPanAngle = xVal; 
    
    panServo.write(xVal);
    tiltServo.write(yVal);
    
    // Geçerli konum geldiği için devriye sayacını ŞİMDİ sıfırla
    lastSerialTime = millis(); 
  }
}

// Devriye (Tarama) Fonksiyonu
void sweepArea() {
  if (millis() - lastSweepTime > sweepInterval) {
    lastSweepTime = millis();
    
    currentPanAngle += sweepStep;
    
    if (currentPanAngle >= 150) {
      currentPanAngle = 150;
      sweepStep = -1; 
    } else if (currentPanAngle <= 30) {
      currentPanAngle = 30;
      sweepStep = 1;  
    }
    
    panServo.write(currentPanAngle);
    tiltServo.write(90); 
  }
}

// Ekran Yazıları
void updateDisplay() {
  static unsigned long lastLcdTime = 0;
  if (millis() - lastLcdTime > 200) {
      lastLcdTime = millis();
      
      lcd.setCursor(0, 0);
      if (millis() - lastSerialTime <= idleTimeout) {
        lcd.print("HEDEF IZLENIYOR ");
      } else {
        lcd.print("ALAN TARANIYOR  ");
      }
      
      lcd.setCursor(0, 1);
      lcd.print("Mesafe: ");
      if (distance < 400) {
        lcd.print(distance);
        lcd.print(" cm   ");
      } else {
        lcd.print("Yok   ");
      }
  }
}

void fireSequence() {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("!!! TEHDIT !!!");
  lcd.setCursor(0, 1);
  lcd.print("ATESLENIYOR...");

  digitalWrite(laserPin, HIGH);

  unsigned long startTime = millis();
  while (millis() - startTime < 2000) {
    tone(buzzerPin, 1200); delay(60);
    tone(buzzerPin, 400);  delay(40);
    noTone(buzzerPin);     delay(100);
  }

  digitalWrite(laserPin, LOW);
  lcd.clear();
}