using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

class PersonSearchEngine
{
    private const LuceneVersion version = LuceneVersion.LUCENE_48;

    private readonly StandardAnalyzer _analyzer;
    private readonly RAMDirectory _directory;
    private readonly IndexWriter _writer;

    public PersonSearchEngine()
    {
        _analyzer = new StandardAnalyzer(version);
        _directory = new RAMDirectory();
        var config = new IndexWriterConfig(version, _analyzer);
        _writer = new IndexWriter(_directory, config);
    }

    public void AddPersonsToIndex(IEnumerable<Person> persons)
    {
        foreach (var person in persons)
        {
            var document = new Document();
            document.Add(new StringField("Guid", person.Guid.ToString(), Field.Store.YES));
            document.Add(new TextField("FirstName", person.FirstName, Field.Store.YES));
            document.Add(new TextField("LastName", person.LastName, Field.Store.YES));
            document.Add(new TextField("Company", person.Company, Field.Store.YES));
            document.Add(new TextField("Description", person.Description, Field.Store.YES));
            _writer.AddDocument(document);
        }
        _writer.Commit();
    }

    public IEnumerable<Person> Search(string searchTerm)
    {
        var directoryReader = DirectoryReader.Open(_directory);
        var indexSearcher = new IndexSearcher(directoryReader);

        string [] fields = {"FirstName", "LastName", "Company", "Description"};
        var queryParser = new MultiFieldQueryParser(version, fields, _analyzer);
        var query = queryParser.Parse(searchTerm);

        var hits = indexSearcher.Search(query, 10).ScoreDocs;
        var persons = new List<Person>();
        foreach (var hit in hits)
        {
            var document = indexSearcher.Doc(hit.Doc);
            persons.Add(new Person(){
                Guid = new Guid(document.Get("Guid")),
                FirstName = document.Get("FirstName"),
                LastName = document.Get("LastName"),
                Company = document.Get("Company"),
                Description = document.Get("Description")
            });
        }
        return persons;
    }
}