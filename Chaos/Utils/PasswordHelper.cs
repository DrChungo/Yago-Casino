
//Clase para encriptar y verificar contraseñas utilizando BCrypt
public static class PasswordHelper
{
    // Encriptar contraseña
    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }   

    // Verificar contraseña
    public static bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}