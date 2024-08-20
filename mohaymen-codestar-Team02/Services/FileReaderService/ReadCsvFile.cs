namespace mohaymen_codestar_Team02.Services.FileReaderService;

public class ReadCsvFile : IFileReader
{
    public string Read(IFormFile? file)
    {
        if (file == null || file.Length == 0) return "";

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            return reader.ReadToEnd();
        }
    }
}