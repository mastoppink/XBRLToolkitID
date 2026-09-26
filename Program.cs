
namespace XBRLToolkitID
{

  class Program
  {
    static void Main(string[] args)
    {
      TaxonomyElement.Instance.Load("./taxonomy/cor-element2020.xsd"); // loading taxonomy concept

      // var files = ScannerInstance.Scan("./IDX_Reports");
      // XbrlExtractor.Extract(files.First());

      var AssetConcept = TaxonomyElement.Instance.GetByName("Liabilities");
      Console.WriteLine($"Assets: {AssetConcept?.ToString()}");
    }
  }
}