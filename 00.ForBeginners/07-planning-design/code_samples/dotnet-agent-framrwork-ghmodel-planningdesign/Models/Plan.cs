using System.Text.Json.Serialization;
public class Plan
{
	[JsonPropertyName("assigned_agent")]
	public string? Assigned_agent { get; set; }

	[JsonPropertyName("task_details")]
	public string? Task_details { get; set; }
}