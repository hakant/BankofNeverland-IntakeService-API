public class CosmosDbConfig
{
    public CosmosDbConfig()
    { }

    public string DatabaseId { get; set; }
    public string ServiceEndPoint { get; set; }
    public string PrimaryKey { get; set; }

    public Collections Collections { get; } = new Collections();
}

public class Collections
{
    public string IntakesCollection = "IntakesCollection";
}