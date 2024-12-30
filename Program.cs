using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static MalzemeStokYoneticisi malzemeStoku = new(); // Malzeme stok yöneticisi
    static List<Tarif> tarifListesi = new() // Tarif listesi
    {
        new Tarif { Isim = "Omlet", Malzemeler = new() { { "Yumurta", 2 }, { "Tuz", 1 } }, Kalori = 150, Kategori = "Kahvalti" },
        new Tarif { Isim = "Sebze Corbasi", Malzemeler = new() { { "Havuc", 2 }, { "Patates", 1 }, { "Sogan", 1 } }, Kalori = 250, Kategori = "Oglen Yemeği" }
        
    };

    static void Main()
    {
        while (true)
        {
            EkranTemizleVeRenkliYaz("YEMEK PLANLAYICI ve TARIF ÖNERICI", ConsoleColor.Yellow);
            Console.WriteLine("1. Tarifleri Listele\n2. Yeni Tarif Ekle\n3. Malzeme Yonetimi\n4. Yapilabilir Yemekleri Listele\n5. Cikis\n");
            Console.Write("Seciminizi yapin: ");

            switch (Console.ReadLine())
            {
                case "1": TarifleriListele(); break;
                case "2": YeniTarifEkle(); break;
                case "3": MalzemeYoneticisi(); break;
                case "4": YapilabilirYemekleriListele(); break;
                case "5": CikisYap(); return;
                default: RenkliYaz("Gecersiz secim!", ConsoleColor.Red); break;
            }
        }
    }

    // Tarif sinifi
    class Tarif
    {
        public string Isim { get; set; }
        public Dictionary<string, int> Malzemeler { get; set; } = new();
        public int Kalori { get; set; }
        public string Kategori { get; set; }
    }

    // Malzeme sinifi
    class Malzeme
    {
        public string Isim { get; set; }
        public int Miktar { get; set; }
    }

    // Malzeme stok yoneticisi
    class MalzemeStokYoneticisi
    {
        private Dictionary<string, int> malzemeStogu = new();

        public void MalzemeEkle(string isim, int miktar)
        {
            string normalizedIsim = isim.ToUpper();
            malzemeStogu[normalizedIsim] = malzemeStogu.GetValueOrDefault(normalizedIsim) + miktar;
        }

        public bool MalzemeVarMi(string isim) => malzemeStogu.ContainsKey(isim.ToUpper());

        public int GetMiktar(string isim) => malzemeStogu.GetValueOrDefault(isim.ToUpper(), 0);

        public void MalzemeGoruntule()
        {
            if (!malzemeStogu.Any())
            {
                RenkliYaz("Hic malzeme bulunamadi.", ConsoleColor.Red);
            }
            else
            {
                foreach (var m in malzemeStogu)
                {
                    Console.WriteLine($"{m.Key}: {m.Value} adet");
                }
            }
        }

        public void GuncelleStok(Dictionary<string, int> malzemeler)
        {
            foreach (var malzeme in malzemeler)
            {
                malzemeStogu[malzeme.Key] -= malzeme.Value;
            }
        }
    }

    // Tarifleri listele
    static void TarifleriListele()
    {
        EkranTemizleVeRenkliYaz("MEVCUT TARIFLER", ConsoleColor.Cyan);

        if (!tarifListesi.Any())
        {
            RenkliYaz("Henüz tarif eklenmedi.", ConsoleColor.Red);
        }
        else
        {
            foreach (var tarif in tarifListesi)
            {
                Console.WriteLine($"Kategori: {tarif.Kategori}");
                RenkliYaz($"- {tarif.Isim} (Kalori: {tarif.Kalori})", ConsoleColor.Green);
                Console.WriteLine($"  Malzemeler: {string.Join(", ", tarif.Malzemeler.Select(m => $"{m.Key} ({m.Value} adet)"))}");
            }
        }
        Bekle();
    }

    // Yeni tarif ekle
    static void YeniTarifEkle()
    {
        EkranTemizleVeRenkliYaz("YENI TARIF EKLE", ConsoleColor.Green);

        Console.Write("Tarif Ismi: ");
        string isim = Console.ReadLine();

        Console.Write("Kalori (kcal): ");
        int kalori;
        while (!int.TryParse(Console.ReadLine(), out kalori))
        {
            RenkliYaz("Gecersiz kalori girisi! Lutfen bir sayi giriniz.", ConsoleColor.Red);
            Console.Write("Kalori (kcal): ");
        }

        Console.WriteLine("\nKategori Secin:\n1. Kahvalti\n2. Ara Ogun\n3. Oglen Yemegi\n4. Aksam Yemegi\n");
        Console.Write("Seciminiz: ");
        string kategori = KategoriSec(Console.ReadLine());

        if (kategori == null)
        {
            RenkliYaz("Gecersiz kategori secimi!", ConsoleColor.Red);
            return;
        }

        var malzemeler = MalzemeEkle();

        tarifListesi.Add(new Tarif { Isim = isim, Kalori = kalori, Kategori = kategori, Malzemeler = malzemeler });
        RenkliYaz("Tarif basariyla eklendi!", ConsoleColor.Green);
    }

    // Malzeme yonetimi
    static void MalzemeYoneticisi()
    {
        EkranTemizleVeRenkliYaz("MALZEME YONETIMI", ConsoleColor.Blue);

        while (true)
        {
            Console.WriteLine("1. Malzeme Ekle\n2. Mevcut Malzemeleri Goruntule\n3. Cikis\n");
            Console.Write("Seciminiz: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Malzeme Ismi: ");
                    string isim = Console.ReadLine();
                    if (!IsGecerliMalzemeIsmi(isim))
                    {
                        RenkliYaz("Gecerli bir malzeme ismi giriniz!", ConsoleColor.Red);
                        break;
                    }

                    Console.Write("Miktar: ");
                    int miktar;
                    while (!int.TryParse(Console.ReadLine(), out miktar))
                    {
                        RenkliYaz("Gecersiz miktar! Lutfen gecerli bir sayi giriniz.", ConsoleColor.Red);
                        Console.Write("Miktar: ");
                    }

                    // Malzeme ekleme islemi
                    malzemeStoku.MalzemeEkle(isim, miktar);
                    RenkliYaz("Malzeme basariyla guncellendi!", ConsoleColor.Green);
                    break;

                case "2":
                    malzemeStoku.MalzemeGoruntule();
                    break;

                case "3":
                    return;

                default:
                    RenkliYaz("Gecersiz secim!", ConsoleColor.Red);
                    break;
            }
        }
    }

    // Yapilabilir yemekleri listele
    static void YapilabilirYemekleriListele()
    {
        EkranTemizleVeRenkliYaz("YAPILABILIR YEMEKLER", ConsoleColor.Magenta);

        var yapilabilirTarifler = tarifListesi.Where(t =>
            t.Malzemeler.All(m => malzemeStoku.GetMiktar(m.Key) >= m.Value)).ToList();

        if (!yapilabilirTarifler.Any())
        {
            RenkliYaz("Mevcut malzemelerle yapilabilecek yemek yok.", ConsoleColor.Red);
        }
        else
        {
            int index = 1;
            foreach (var tarif in yapilabilirTarifler)
            {
                int maxAdet = tarif.Malzemeler.Min(m => malzemeStoku.GetMiktar(m.Key) / m.Value);
                Console.WriteLine($"{index}. {tarif.Isim} (Yapilabilir: {maxAdet} adet)");
                index++;
            }

            Console.Write("Bir yemek secin (cikmak icin bos birakın): ");
            string secim = Console.ReadLine();
            if (int.TryParse(secim, out int secilenIndex) && secilenIndex > 0 && secilenIndex <= yapilabilirTarifler.Count)
            {
                YapilabilirYemekSecVeYap(yapilabilirTarifler[secilenIndex - 1]);
            }
        }
        Bekle();
    }

    // Yapilabilir yemeği sec ve yap
    static void YapilabilirYemekSecVeYap(Tarif secilenTarif)
    {
        malzemeStoku.GuncelleStok(secilenTarif.Malzemeler);
        RenkliYaz($"{secilenTarif.Isim} yapildi! Malzeme stoğu guncellendi.", ConsoleColor.Green);
    }

    // Cikis yap
    static void CikisYap()
    {
        EkranTemizleVeRenkliYaz("Cikis yapiliyor...", ConsoleColor.Red);
    }

    // Kategori secimi
    static string KategoriSec(string secim)
    {
        return secim switch
        {
            "1" => "Kahvalti",
            "2" => "Ara Ogun",
            "3" => "Oglen Yemegi",
            "4" => "Aksam Yemegi",
            _ => null
        };
    }

    // Malzeme ekle
    static Dictionary<string, int> MalzemeEkle()
    {
        var malzemeler = new Dictionary<string, int>();

        while (true)
        {
            Console.Write("Malzeme ismi (bitirmek icin bos birakın): ");
            string malzemeIsim = Console.ReadLine();
            if (string.IsNullOrEmpty(malzemeIsim)) break;

            if (!IsGecerliMalzemeIsmi(malzemeIsim))
            {
                RenkliYaz("Gecerli bir malzeme ismi giriniz!", ConsoleColor.Red);
                continue;
            }

            Console.Write("Miktar: ");
            int miktar;
            while (!int.TryParse(Console.ReadLine(), out miktar))
            {
                RenkliYaz("Gecersiz miktar! Lutfen gecerli bir sayi giriniz.", ConsoleColor.Red);
                Console.Write("Miktar: ");
            }
            malzemeler[malzemeIsim.ToUpper()] = miktar; // Malzeme ismini büyük harfe donusturerek ekle
        }

        return malzemeler;
    }

    // Gecerli malzeme ismi kontrol
    static bool IsGecerliMalzemeIsmi(string isim)
    {
        return !string.IsNullOrWhiteSpace(isim) && isim.All(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)); // Yalnizca harf ve bosluk izin verilir
    }

    // Renkli yazı
    static void RenkliYaz(string mesaj, ConsoleColor renk)
    {
        Console.ForegroundColor = renk;
        Console.WriteLine(mesaj);
        Console.ResetColor();
    }

    // Ekran temizleme ve başlık yazma
    static void EkranTemizleVeRenkliYaz(string baslik, ConsoleColor renk)
    {
        Console.Clear();
        RenkliYaz(baslik, renk);
        Console.WriteLine(new string('-', 30));
    }

    // Bekle
    static void Bekle()
    {
        Console.Write("\nDevam etmek için bir tuşa basın...");
        Console.ReadKey();
    }
}
