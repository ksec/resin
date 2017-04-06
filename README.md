# Resin

Resin is a vector space model implementation, a search and analytics framework and a document store. Querying support includes exact,  fuzzy and prefix, soon also range (up-coming feature in RC 4), and comes with customizable tokenizers and scoring schemes.

## Supports any scoring scheme
Out-of-the-box, to support the default tf-idf scoring scheme Resin will store term counts. To support any scoring scheme Resin gives you the ability to store any additional data (up-coming feature in RC4). That data will be delivered to you neatly as a field on the document posting. In your custom IScoringScheme you then base your per-document posting calculations on that data instead of just the term count.

## Fast at indexing and querying
In many scenarios Resin is already faster than the [market leader](https://lucenenet.apache.org/) when it comes down to querying and indexing speed, making it a [in-many-scenarios-fastest](https://github.com/kreeben/resin/wiki/Lucene-vs-Resin-1.0-RC2) information retrieval system on the .net plaform. 

When Resin is not faster than Lucene, most of the times it's because I haven't spent much time yet looking into that particular scenario. If you have a scenario where you feel Resin should do better, let me know please, so that we can get rid of Lucene.Net 3.0.3 once and for all.

## Not based on a java port (but deeply influenced)
Lucene hit version 3 a while ago. Five years ago the .net community's search engine also came in at version 3 (still is).

Who could use a modern and powerful search engine based on sound mathematics that's extensible and built on Core, though?

## Stable API and file format in RC3
Resin's API and file format should be considered unstable until release candidate 3. Coming features are indexing support for IComparable instead of just strings, improved compression of documents by representing them as tries, and updates/merges of documents.

## Supported .net version
Resin is built for 4.6.1 but have no dependancies on any Core-incompatible technology. Resin will be available on both frameworks soon.

## Download
Latest release is [here](https://github.com/kreeben/resin/releases/latest)

## Help out?
Start [here](https://github.com/kreeben/resin/issues).

## Documentation
### A document.

	{
		"_id": "Q1",
		"label":  "universe",
		"description": "totality of planets, stars, galaxies, intergalactic space, or all matter or all energy",
		"aliases": "cosmos The Universe existence space outerspace"
	}

### Many like that.
	
	var docs = GetWikipediaAsJson();

### Index them.

	var dir = @"C:\Users\Yourname\Resin\wikipedia";
	using (var write = new WriteOperation(dir, new Analyzer()))
	{
		write.Write(docs);
	}

### Query the index.
<a name="inproc" id="inproc"></a>

	// Resin will scan a disk based trie for terms that are an exact match,
	// a near match or is prefixed with the query term/-s.
	
	// A set of postings is produced for each query statement.
	// A final answer is compiled by reducing the query tree into one node, postings into one set.
		
	// Postings are resolved into top scoring documents. A total hit count is also included.
	// Paging is fast using the built-in paging mechanism.
	
	var result = new Searcher(dir).Search("label:good bad~ description:leone");
	
	// Document scores, i.e. the aggregated tf-idf weights a document recieve from a simple or compound query,
	// are also included in the result:
	
	var scoreOfFirstDoc = result.Docs.First().Fields["__score"];

[More documentation here](https://github.com/kreeben/resin/wiki). 

### Roadmap

- [x] Layout basic architecture and infrastructure of a modern IR system - v0.9b
- [x] Query faster than Lucene - v1.0 RC1
- [x] ___Index faster than Lucene - v1.0 RC2___
- [ ] Compress better than Lucene - v1.0 RC3
- [ ] Migrate to Core - v1.0
- [ ] Build Sir, a distributed search engine

### Sir

[Sir](https://github.com/kreeben/sir) is an Elasticsearch clone with improved querying capabilities, built on Resin.
