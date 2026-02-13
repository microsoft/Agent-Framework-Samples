using System;
using System.Linq;

var type = Type.GetType("System.ClientModel.ClientResultException, System.ClientModel");
Console.WriteLine(type);
foreach (var prop in type.GetProperties())
{
	Console.WriteLine($"Property: {prop.Name} - {prop.PropertyType}");
}
foreach (var field in type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
{
	Console.WriteLine($"Field: {field.Name} - {field.FieldType}");
}
