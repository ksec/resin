﻿using System.IO;
using System.Linq;
using Resin.IO;
using Resin.IO.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class MappedTrieReaderTests : Setup
    {
        [TestMethod]
        public void Can_find_within_range()
        {
            var fileName = Path.Combine(CreateDir(), "MappedTrieReaderTests.Can_find_within_range.tri");

            var tree = new LcrsTrie();
            tree.Add("ape");
            tree.Add("app");
            tree.Add("apple");
            tree.Add("banana");
            tree.Add("bananas");
            tree.Add("xanax");
            tree.Add("xxx");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            File.WriteAllText("Can_find_within_range.log", tree.Visualize(), System.Text.Encoding.UTF8);

            tree.Serialize(fileName);

            IList<Word> words;

            using (var reader = new MappedTrieReader(fileName))
            {
                words = reader.WithinRange("app", "xerox").ToList();
            }

            Assert.AreEqual(4, words.Count);
            Assert.AreEqual("apple", words[0].Value);
            Assert.AreEqual("banana", words[1].Value);
            Assert.AreEqual("bananas", words[2].Value);
            Assert.AreEqual("xanax", words[3].Value);
        }

        [TestMethod]
        public void Can_find_near()
        {
            var fileName = Path.Combine(CreateDir(), "MappedTrieReaderTests.Can_find_near.tri");

            var tree = new LcrsTrie();

            tree.Add("bad");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 1).Select(w => w.Value).ToList();

                Assert.AreEqual(1, near.Count);
                Assert.IsTrue(near.Contains("bad"));
            }

            tree = new LcrsTrie();
            tree.Add("baby");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            File.WriteAllText("Can_find_near.log", tree.Visualize(), System.Text.Encoding.UTF8);

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 1).Select(w => w.Value).ToList();

                Assert.AreEqual(1, near.Count);
                Assert.IsTrue(near.Contains("bad"));
            }

            tree = new LcrsTrie();
            tree.Add("b");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);
            File.WriteAllText("Can_find_near.log", tree.Visualize(), System.Text.Encoding.UTF8);

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 1).Select(w => w.Value).ToList();

                Assert.AreEqual(2, near.Count);
                Assert.IsTrue(near.Contains("bad"));
                Assert.IsTrue(near.Contains("b"));
            }

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 2).Select(w => w.Value).ToList();

                Assert.AreEqual(3, near.Count);
                Assert.IsTrue(near.Contains("b"));
                Assert.IsTrue(near.Contains("bad"));
                Assert.IsTrue(near.Contains("baby"));
            }

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 0).Select(w => w.Value).ToList();

                Assert.AreEqual(0, near.Count);
            }

            tree = new LcrsTrie();
            tree.Add("bananas");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("ba", 6).Select(w => w.Value).ToList();

                Assert.AreEqual(4, near.Count);
                Assert.IsTrue(near.Contains("b"));
                Assert.IsTrue(near.Contains("bad"));
                Assert.IsTrue(near.Contains("baby"));
                Assert.IsTrue(near.Contains("bananas"));
            }

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("bazy", 1).Select(w => w.Value).ToList();

                Assert.AreEqual(1, near.Count);
                Assert.IsTrue(near.Contains("baby"));
            }

            tree = new LcrsTrie();
            tree.Add("bank");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                var near = reader.Near("bazy", 3).Select(w => w.Value).ToList();

                Assert.AreEqual(4, near.Count);
                Assert.IsTrue(near.Contains("baby"));
                Assert.IsTrue(near.Contains("bank"));
                Assert.IsTrue(near.Contains("bad"));
                Assert.IsTrue(near.Contains("b"));
            }
        }

        [TestMethod]
        public void Can_find_prefixed()
        {
            var fileName = Path.Combine(CreateDir(), "MappedTrieReaderTests.Can_find_prefixed.tri");

            var tree = new LcrsTrie('\0', false);

            tree.Add("rambo");
            tree.Add("rambo");

            tree.Add("2");

            tree.Add("rocky");

            tree.Add("2");

            tree.Add("raiders");

            tree.Add("of");
            tree.Add("the");
            tree.Add("lost");
            tree.Add("ark");

            tree.Add("rain");

            tree.Add("man");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            var prefixed = new MappedTrieReader(fileName).StartsWith("ra").Select(w => w.Value).ToList();

            Assert.AreEqual(3, prefixed.Count);
            Assert.IsTrue(prefixed.Contains("rambo"));
            Assert.IsTrue(prefixed.Contains("raiders"));
            Assert.IsTrue(prefixed.Contains("rain"));
        }

        [TestMethod]
        public void Can_find_exact()
        {
            var fileName = Path.Combine(CreateDir(), "MappedTrieReaderTests.Can_find_exact.tri");

            var tree = new LcrsTrie('\0', false);
            tree.Add("xor");
            tree.Add("xxx");
            tree.Add("donkey");
            tree.Add("xavier");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("xxx").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsFalse(reader.IsWord("baby").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsFalse(reader.IsWord("dad").Any());
            }

            tree = new LcrsTrie();
            tree.Add("baby");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("xxx").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("baby").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsFalse(reader.IsWord("dad").Any());
            }

            tree = new LcrsTrie();
            tree.Add("dad");
            tree.Add("daddy");

            foreach (var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            tree.Serialize(fileName);

            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("xxx").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("baby").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("dad").Any());
            }
            using (var reader = new MappedTrieReader(fileName))
            {
                Assert.IsTrue(reader.IsWord("daddy").Any());
            }
        }

        [TestMethod]
        public void Can_deserialize_whole_file()
        {
            var dir = CreateDir();

            var fileName = Path.Combine(dir, "MappedTrieReaderTests.Can_deserialize_whole_file.tri");

            var tree = new LcrsTrie('\0', false);
            tree.Add("baby");
            tree.Add("bad");
            tree.Add("bank");
            tree.Add("box");
            tree.Add("dad");
            tree.Add("dance");

            foreach(var node in tree.EndOfWordNodes())
            {
                node.PostingsAddress = new BlockInfo(long.MinValue, int.MinValue);
            }

            Assert.IsTrue(tree.IsWord("baby").Any());
            Assert.IsTrue(tree.IsWord("bad").Any());
            Assert.IsTrue(tree.IsWord("bank").Any());
            Assert.IsTrue(tree.IsWord("box").Any());
            Assert.IsTrue(tree.IsWord("dad").Any());
            Assert.IsTrue(tree.IsWord("dance").Any());

            tree.Serialize(fileName);
            File.WriteAllText("Can_deserialize_whole_file.log", tree.Visualize(), System.Text.Encoding.UTF8);

            var recreated = Serializer.DeserializeTrie(dir, new FileInfo(fileName).Name);

            Assert.IsTrue(recreated.IsWord("baby").Any());
            Assert.IsTrue(recreated.IsWord("bad").Any());
            Assert.IsTrue(recreated.IsWord("bank").Any());
            Assert.IsTrue(recreated.IsWord("box").Any());
            Assert.IsTrue(recreated.IsWord("dad").Any());
            Assert.IsTrue(recreated.IsWord("dance").Any());
        }
    }
}