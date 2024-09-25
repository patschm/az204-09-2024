namespace EvtGridWebHook.Models;

public class GridEventModel
{
    public string? Id { get; set; }
    public string? EventType { get; set; }
    public string? Subject { get; set; }
    public DateTime EventTime { get; set; }        
    public string? Topic { get; set; }
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
}
