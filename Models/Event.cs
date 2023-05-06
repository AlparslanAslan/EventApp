namespace EventApp.Models;

public class Event
{
    public int Id {get;set;}
    public string? Baslik { get; set; }
    public string? Aciklama { get; set; }
    public DateTime Tarih { get; set; }
    public int Kontenjan { get; set; }
    public string? Kategori { get; set; }
    public string? Sehir { get; set; }
    public string? Olusturan { get; set; }
    public int OlusturanNumeric { get; set; }
    public int Onay { get; set; }
    public int Aktif { get; set; }
    public string? Adres { get; set; }
    
}


