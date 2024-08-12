using mohaymen_codestar_Team02.initialProgram;

namespace mohaymen_codestar_Team02;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        InitialApp.Init(builder);
        
    }
}