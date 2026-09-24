using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO.Compression;


namespace XBRLToolkitID
{

    public enum ReportType
    {
        TAHUNAN,
        TW1,
        TW2,
        TW3
    }

    public enum ContextType
    {
        CurrentYearInstant,
        CurrentYearDuration,
        PriorYearInstan,
        PriorYearDuration
        
    }

    public record FactItem(
        ReportType ReportId,
        string EntityId,
        string Concept, 
        string TextValue, 
        decimal NumberValue, 
        string MoneyUnit,
        int Decimals,
        ContextType Context,
        string StartDate,
        string EndDate
        );

    public class XbrlExtractor
    {
        public static List<FactItem> Extract(string instanceFile)
        {
            List<FactItem> facts = new List<FactItem>();
            using var archive = ZipFile.OpenRead(instanceFile);

            var firstXml = archive
                                .Entries
                                .FirstOrDefault(e => e.FullName.EndsWith("instance.xbrl", StringComparison.OrdinalIgnoreCase));

            if (firstXml == null)
            {
                Console.WriteLine("First XML is Null");
                throw new Exception();
            }

            var stream = firstXml.Open();

            XDocument doc = XDocument.Load(stream);
            XElement? root = doc.Root;

            if (root == null)
            {
                Console.WriteLine("Dokumen Kosong.");
                throw new Exception();
            }

            // XNamespace xbrli = root.GetDefaultNamespace();

            Console.WriteLine("=== Informasi Umum (DEI)===");

            var deiFacts = root.Elements()
                .Where(e => e.Name.NamespaceName.Contains("/dei"))
                .Where(e => ImportantElementList.deiElements.Contains(e.Name.LocalName))
                .Select(e => new
                {
                    Tag = e.Name.LocalName,
                    Value = e.Value.Trim()
                });

            foreach (var item in deiFacts)
            {
                string value = Regex.Replace(item.Value, @"\s+", " ").Trim();
                // facts.Add(new FactItem(item.Tag, value, 0, "currentYearInstant"));
                Console.WriteLine($"{item.Tag,-35}: {value}");
            }

            Console.WriteLine("=== Financial Facts ===");

            var financialFacts = root.Elements()
              .Where(e => e.Name.NamespaceName.Contains("/cor"))
              .Where(e => ImportantElementList.financialElements.Contains(e.Name.LocalName))
              .Where(e => (string?)e.Attribute("contextRef") == "CurrentYearInstant" || (string?)e.Attribute("contextRef") == "CurrentYearDuration")
              .Where(e => !string.IsNullOrWhiteSpace(e.Value))
              .Select(e => new
              {
                  Tag = e.Name.LocalName,
                  Value = e.Value.Trim()
              });

            foreach (var item in financialFacts)
            {
                var value = item.Value;
                Console.WriteLine($"{item.Tag,-35}: {value}");
            }

            return facts;
        }
    }
}