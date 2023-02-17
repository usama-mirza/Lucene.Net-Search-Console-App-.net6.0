
using System.Text.Json;

string path = AppContext.BaseDirectory;
var personsFileText = File.ReadAllText(path.Replace(@"\bin\Debug\net6.0\", @"\persons.json"));
var persons = JsonSerializer.Deserialize<List<Person>>(personsFileText, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

var engine = new PersonSearchEngine();
if(persons == null)
    return;

engine.AddPersonsToIndex(persons);
while (true)
{
    Console.Clear();
    Console.WriteLine("Enter a search query: ");
    var query = Console.ReadLine();
    if(string.IsNullOrEmpty(query))
        continue;

    var results = engine.Search(query);
    if (!results.Any())
    {
        Console.WriteLine("No results found.");
        continue;
    }

    Console.WriteLine("Results");
    foreach (var result in results)
    {
        Console.WriteLine($"{result.Company} - {result.FirstName} ({result.LastName})");
    }

    Console.WriteLine("Punch your keyboard to continue...");
    Console.ReadKey();
}