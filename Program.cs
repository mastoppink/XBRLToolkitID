
namespace XBRLToolkitID
{

  class Program
  {
    static void Main(string[] args)
    {
      var files = ScannerInstance.Scan("./IDX_Reports");
      XbrlExtractor.Extract(files.First());
    }
  }
}