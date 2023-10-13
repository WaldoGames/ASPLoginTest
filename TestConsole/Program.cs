// See https://aka.ms/new-console-template for more information
using Dal.Classes;


Console.Write("Voer u nieuwe wachtwoord in aub: ");
string pass = Console.ReadLine();

bool logedin = false;
Encryption encryption = new Encryption();
string Epass = encryption.EncryptNewString(pass);
Console.WriteLine("Encrypted password: "+Epass);
while (!logedin)
{
    Console.Write("Voer u huidige wachtwoord in aub: ");
    string LoginPass = Console.ReadLine();

    if(encryption.CompareEncryptedString(LoginPass, Epass))
    {
        logedin = true;
    }
    else
    {
        Console.WriteLine("Verkeerde wachtwoord");
    }
}


//string Epass2 = encryption.EncryptNewString(pass);

Console.WriteLine("U bent ingelogd");