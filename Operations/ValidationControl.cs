using EventApp.Models;
using System.Text.RegularExpressions;
namespace EventApp.Operations;
public class ValidationControl
{
    public string IsUserValid(User user)
    {
        
        if(String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Surname)|| String.IsNullOrEmpty(user.Email)||String.IsNullOrEmpty(user.Password))
            return "Tüm alanlar dolu olmalıdır!";
        
        else if(!user.Email.Contains('@'))
            return "Lütfen geçerli bir Email adresi giriniz!";
        string pattern = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$";
        
        bool isValidPassword = Regex.IsMatch(user.Password, pattern);
        if(!isValidPassword)
            return "Şifreniz en az sekiz karakter, en az bir harf, bir rakam ve bir özel karakter içermelidir";
        else if(user.Name.Length>30 || user.Surname.Length>30)
            return"Kullanıcı adı veya soyadı 30 karakterden fazla olumaz!";
        
        string patternForName = @"[\W\d]";
        bool hasSpecialCharOrNumber = Regex.IsMatch(user.Name, patternForName);
        if(hasSpecialCharOrNumber)
            return"Kulllanıcı adı sayı ya da özel karakter içeremez";

        bool hasSpecialCharOrNumberS = Regex.IsMatch(user.Surname, patternForName);
        if(hasSpecialCharOrNumberS)
            return"Kulllanıcı soyadı sayı ya da özel karakter içeremez";
        
        return "Valid";
    }
    public string EventControl(Event _event)
    {
        if(String.IsNullOrEmpty(_event.Baslik)||String.IsNullOrEmpty(_event.Adres)||_event.Kontenjan == 0)
            return "Zorunlu alanlar doldurulmalıdır.";
        if(_event.Tarih < DateTime.Now)
            return "Geçmiş tarihli bir etkinlik oluşturulamaz.";
        return "Valid";
    }
}