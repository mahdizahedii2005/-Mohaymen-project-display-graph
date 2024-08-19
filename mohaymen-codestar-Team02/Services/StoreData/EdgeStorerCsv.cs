using System.Dynamic;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class EdgeStorerCsv(DataContext dataContext) : IEdageStorer
{
    public bool StoreFileData(string dataFile, long dataGroupId)
    {
        var result = new List<ExpandoObject>();
        using (var reader = new StringReader(dataFile))
        {
            string? headerLine = reader.ReadLine();
            if (headerLine == null)
            {
                return false;
            }
            var headers = headerLine.Split(',');
            string? line;
            // خواندن هر خط به جز هدر
            while ((line = reader.ReadLine()) != null)
            {
                // جدا کردن مقادیر بر اساس کاما
                var values = line.Split(',');

                // ایجاد یک شیء ExpandoObject داینامیک
                var expando = new ExpandoObject() as IDictionary<string, object>;

                // پر کردن ExpandoObject با داده‌ها
                for (int i = 0; i < headers.Length; i++)
                {
                    expando[headers[i]] = values[i]; // افزودن هدر به عنوان کلید و مقدار مربوط به آن
                }

                // افزودن شیء داینامیک به لیست نتیجه
                result.Add((ExpandoObject)expando);
            }
        }

        return result; // بازگرداندن لیست شامل رکوردهای CSV به صورت داینامیک
    }
}