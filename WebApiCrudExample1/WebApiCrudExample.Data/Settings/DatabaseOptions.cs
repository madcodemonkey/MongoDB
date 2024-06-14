namespace WebApiCrudExample.Data;

/// <summary>Settings used by the data library.</summary>
public class DatabaseOptions
{
    public static string SectionName = "DatabaseOptions";

    public string MongoDbConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;

}