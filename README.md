# smart-turret-system
Bu proje, Bilgisayar Teknolojisi alanındaki donanım ve yazılım entegrasyonu yetkinliklerini sergilemek amacıyla geliştirilmiş gerçek zamanlı ve otonom bir pan-tilt savunma taretidir.
#  Otonom Sentry Gun (Hedef Takip ve Savunma Tareti)

Bu proje, kameradan gördüğü hedefi otonom olarak takip eden ve hedefin belirli bir mesafeye (30 cm) girmesiyle uyarı/ateşleme sistemini devreye sokan robotik bir savunma sistemidir.

##  Sistem Ne Yapıyor?

* **Gözlem ve Takip:** ESP32-CAM modülü Wi-Fi üzerinden canlı görüntü aktarır. C# ile yazılmış program bu görüntüyü işleyerek hedefin koordinatlarını bulur ve tareti o yöne çevirir.
* **Devriye Modu (Sweep):** Eğer 2 saniye boyunca etrafta bir hedef görülmezse, taret eylemsiz kalmaz; bulunduğu alanı sağa ve sola dönerek otomatik taramaya (devriye atmaya) başlar.
* **Menzil ve Ateşleme:** Taret hedefe döndüğünde ultrasonik sensör ile aradaki mesafeyi ölçer. Hedef 30 cm'den daha yakına gelirse lazer ve sesli uyarı sistemi (ateşleme) anında devreye girer.
* **Hızlı ve Akıcı İletişim:** Bilgisayar ve Arduino arasındaki haberleşme yüksek hızda (115200 baud) yapıldığı için sistem takılmadan, anlık tepkiler verir.

##  Kullanılan Donanım ve Yazılımlar

**Donanımlar:**
* Arduino Uno R3 (Ana Kontrolcü)
* ESP32-CAM (Kablosuz IP Kamera)
* 2 Adet Servo Motor (Pan-Tilt hareket mekanizması için)
* HC-SR04 Ultrasonik Mesafe Sensörü
* 16x2 I2C LCD Ekran (Anlık durum izleme)
* Lazer Diyot ve Buzzer (Ateşleme simülasyonu)

**Yazılımlar:**
* **C# (Windows Forms):** Hedefi tespit eden ve koordinatları hesaplayan bilgisayar arayüzü.
* **C++ (Arduino IDE):** Motorları, sensörü ve devriye algoritmasını yöneten gömülü yazılım.

##  Nasıl Çalışır?

1. **Görüntü Aktarımı:** ESP32-CAM, canlı görüntüyü Wi-Fi ağı üzerinden bilgisayara iletir.
2. **Hesaplama:** C# yazılımı bu görüntüyü analiz eder, hedefi bulur ve X-Y yönlendirme komutlarını USB (Seri Port) üzerinden Arduino'ya gönderir.
3. **Fiziksel Tepki:** Arduino motorları o yöne çevirir. Mesafe sensörü 30 cm altında bir yakınlaşma tespit ederse sistemi savunma moduna sokar. Hedef kaybolursa sistem kendi kendine devriyeye çıkar.

---
**Geliştirici:** Serdar Efe Şentürk  
**Eğitim:** Bilgisayar Teknolojisi
