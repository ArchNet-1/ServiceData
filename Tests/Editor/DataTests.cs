using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace ArchNet.Service.Tests
{
    [Serializable]
    public class ItemTestRow
    {
        public string id;
        public int price;
    }

    public class FakeData : DataService
    {
        public readonly Dictionary<string, string> Files = new Dictionary<string, string>();
        public int Reads;

        protected internal override string ReadText(string name)
        {
            Reads++;
            return Files.TryGetValue(name, out string text) ? text : null;
        }
    }

    public class DataServiceTests
    {
        GameObject _object;
        FakeData _data;

        [SetUp]
        public void SetUp()
        {
            _object = new GameObject("Data");
            _data = _object.AddComponent<FakeData>();
            _data.Files["Items"] = "{ \"rows\": [ { \"id\": \"sword\", \"price\": 120 }, { \"id\": \"potion\", \"price\": 10 } ] }";
        }

        [TearDown]
        public void TearDown() => UnityEngine.Object.DestroyImmediate(_object);

        [Test]
        public void ReadsTheRowsOfATable()
        {
            IReadOnlyList<ItemTestRow> items = _data.Table<ItemTestRow>("Items");

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual("sword", items[0].id);
            Assert.AreEqual(10, items[1].price);
        }

        [Test]
        public void ReadsEachFileOnceUntilTheCacheIsCleared()
        {
            _data.Table<ItemTestRow>("Items");
            _data.Table<ItemTestRow>("Items");
            Assert.AreEqual(1, _data.Reads);

            _data.ClearCache();
            _data.Table<ItemTestRow>("Items");
            Assert.AreEqual(2, _data.Reads);
        }

        [Test]
        public void SaysWhichFileIsMissingOrBroken()
        {
            Assert.Throws<FileNotFoundException>(() => _data.Table<ItemTestRow>("Enemies"));
            _data.Files["Broken"] = "{ rows: [";
            Assert.Throws<InvalidDataException>(() => _data.Table<ItemTestRow>("Broken"));
        }
    }
}
