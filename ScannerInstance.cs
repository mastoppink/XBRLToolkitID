namespace XBRLToolkitID
{
    public class ScannerInstance
    {
        public static IEnumerable<string> Scan(string folderToScan)
        {
            var zipFiles = Directory.EnumerateFiles(folderToScan, "*instance.zip", SearchOption.AllDirectories);
            return zipFiles;
        }
    }
}