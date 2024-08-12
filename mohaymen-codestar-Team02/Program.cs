using mohaymen_codestar_Team02.initialProgram;

public class Program()
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        InitialApp.Init(builder);
    }
}