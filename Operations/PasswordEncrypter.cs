using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordEncrypter
{
    public static string EncryptPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }
    }
     public static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        string enteredPasswordHashed = EncryptPassword(enteredPassword);
        return enteredPasswordHashed.Equals(storedPassword);
    }
}