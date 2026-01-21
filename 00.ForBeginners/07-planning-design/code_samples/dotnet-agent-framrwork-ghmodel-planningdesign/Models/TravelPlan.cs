using System;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
public class TravelPlan
{
	[JsonPropertyName("main_task")]
	public string? Main_task { get; set; }

	[JsonPropertyName("subtasks")]
	public IList<Plan> Subtasks { get; set; }
}