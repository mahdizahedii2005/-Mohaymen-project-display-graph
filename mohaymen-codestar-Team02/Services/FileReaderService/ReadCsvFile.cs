namespace mohaymen_codestar_Team02.Services.FileReaderService;

public class ReadCsvFile : IFileReader
{
    public string Read(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return "";
        }

        var supportedTypes = new[] { "csv" };
        var fileExt = Path.GetExtension(file.FileName).Substring(1);
        if (!supportedTypes.Contains(fileExt.ToLower()))
        {
            return "";
        }

        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine("uploads", fileName);
        return File.ReadAllText(filePath);
    }
}