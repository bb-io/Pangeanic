namespace Apps.Pangeanic.Constants;

public static class ApiEndpoints
{
    private const string TextProcessing = "/NexRelay/v1";
    private const string Corp = TextProcessing + "/corp";
    
    public const string Engines = Corp + "/engines";
    public const string Translate = TextProcessing + "/translate";

    private const string FileProcessing = "/PGFile/v1";
    
    public const string SendFile = FileProcessing + "/sendfile";
    public const string DownloadFile = FileProcessing + "/retrievefile";
}