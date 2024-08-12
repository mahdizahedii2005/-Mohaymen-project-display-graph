using mohaymen_codestar_Team02.initialProgram;
using mohaymen_codestar_Team02.Models;

public class Program()
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        InitialApp.Init(builder);
        
    }
}