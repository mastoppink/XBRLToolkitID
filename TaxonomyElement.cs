using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace XBRLToolkitID
{
  public record XBRLConceptElement
  {
    public string Id { get; init; } = string.Empty;                 // "idx-cor_MudharabahTemporarySyirkahFunds"
    public string Name { get; init; } = string.Empty;               // "MudharabahTemporarySyirkahFunds"
    public string Type { get; init; } = string.Empty;               // "xbrli:monetaryItemType"
    public string SubstitutionGroup { get; init; } = string.Empty;  // "xbrli:item"
    public bool Nillable { get; init; }                            // true
    public string? Balance { get; init; }                           // "credit" / "debit" / null
    public string PeriodType { get; init; } = string.Empty;         // "instant" / "duration"
  }

  public sealed class TaxonomyElement
  {
    private static readonly Lazy<TaxonomyElement> _instance = new Lazy<TaxonomyElement>(() => new TaxonomyElement());

    public static TaxonomyElement Instance => _instance.Value;

    private Dictionary<string, XBRLConceptElement> _elementsByName = new Dictionary<string, XBRLConceptElement>(StringComparer.OrdinalIgnoreCase);


    private TaxonomyElement() { }

    public void Load(string filePath)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException("File XSD tidak ditemukan", filePath);

      var doc = XDocument.Load(filePath);

      var rawElements = doc.Descendants()
          .Where(e => e.Name.LocalName == "element" && e.Attribute("name") != null)
          .Select(e => new XBRLConceptElement
          {
            Id = e.Attribute("id")?.Value ?? string.Empty,
            Name = e.Attribute("name")!.Value,
            Type = e.Attribute("type")?.Value ?? string.Empty,
            SubstitutionGroup = e.Attribute("substitutionGroup")?.Value ?? string.Empty,
            Nillable = bool.TryParse(e.Attribute("nillable")?.Value, out var n) && n,
            Balance = e.Attributes().FirstOrDefault(a => a.Name.LocalName == "balance")?.Value,
            PeriodType = e.Attributes().FirstOrDefault(a => a.Name.LocalName == "periodType")?.Value ?? string.Empty
          }).ToList();

      _elementsByName = new Dictionary<string, XBRLConceptElement>(rawElements.Count, StringComparer.OrdinalIgnoreCase);

      foreach (var elem in rawElements)
      {
        _elementsByName.TryAdd(elem.Name, elem);
      }
    }

    public XBRLConceptElement? GetByName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return null;

      return _elementsByName.TryGetValue(name, out var element) ? element : null;
    }

  }
}