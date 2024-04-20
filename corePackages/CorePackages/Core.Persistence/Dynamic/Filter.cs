namespace Core.Persistence.Dynamic;

public class Filter
{
    public string Field { get; set; }
    public string? Value { get; set; }
    public string Operation { get; set; }
    public string? Logic { get; set; }
    public IEnumerable<Filter>? Filters { get; set; }

    public Filter()
    {
        Field = string.Empty;
        Operation = string.Empty;
    }

    public Filter(string filed, string @operation)
    {
        Field = filed;
        Operation = @operation;
    }
}
