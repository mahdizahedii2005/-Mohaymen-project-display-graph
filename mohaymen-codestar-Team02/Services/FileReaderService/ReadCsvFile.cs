namespace mohaymen_codestar_Team02.Services.FileReaderService;

public class ReadCsvFile : IFileReader
{
    public string Read(IFormFile? file)
    {
        if (file == null || file.Length == 0) throw new FormatException();
        var supportedTypes = new[] { "csv" };
        var fileExt = Path.GetExtension(file.FileName).Substring(1);
        if (!supportedTypes.Contains(fileExt.ToLower()))
        {
            throw new FormatException();
        }

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            return reader.ReadToEnd();
        }
    }
}