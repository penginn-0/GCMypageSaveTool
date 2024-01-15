using Microsoft.Extensions.Configuration;
using System.Text.Json;
namespace GCMypageSaveTool
{
    public class Program
    {
        public static void Main()
        {
            Credential credential = new Credential();
            var Api = new GCAPI();
            if(Api.LoginAndCheck(credential))
            {
                Api.GetPlayedMusics();
            }
        }
    }
}
public class Credential
{
     public Credential()
    {
        var Cfg = new ConfigurationBuilder()
        .AddIniFile(".\\config.ini")
        .Build()
        .GetSection("Credential");

        CardID = Cfg["CardID"];
        Password = Cfg["Password"];

    }
    public string CardID { get; set; }
    public string Password { get; set; }
}