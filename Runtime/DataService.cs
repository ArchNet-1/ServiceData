// ArchNet · Data Service
// Game data that does not change while playing (items, enemies, levels, dialogues…) kept as
// JSON files in a Resources/ArchNetData folder and read into typed classes. A table file looks
// like { "rows": [ { "id": "sword", "price": 10 }, … ] }. Files are read once, then cached.
// The Hub creates GameData (your class) with an example table; add a property per table:
//
//   public IReadOnlyList<ItemRow> Items => Table<ItemRow>("Items");
//
// Docs: https://archnet-plateform.fr/#ServiceData

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ArchNet.Core;
using UnityEngine;

[assembly: InternalsVisibleTo("ArchNet.Service.Data.Tests")]

namespace ArchNet.Service
{
    /// <summary>The shape of a table file: { "rows": [ … ] }.</summary>
    [Serializable]
    public class DataTable<TRow>
    {
        public List<TRow> rows = new List<TRow>();
    }

    public class DataService : GameService
    {
        /// <summary>Folder inside any Resources folder that holds the JSON files.</summary>
        public const string ResourcesFolder = "ArchNetData";

        readonly Dictionary<(string, Type), object> _cache = new Dictionary<(string, Type), object>();

        /// <summary>Reads Resources/ArchNetData/{name}.json into T (cached).</summary>
        public T Load<T>(string name) where T : class
        {
            if (_cache.TryGetValue((name, typeof(T)), out object cached)) return (T)cached;
            string json = ReadText(name)
                ?? throw new FileNotFoundException($"No data file \"{name}\": expected Resources/{ResourcesFolder}/{name}.json (ArchNet > Hub > Services > Data > Open tool).");
            T data;
            try { data = JsonUtility.FromJson<T>(json); }
            catch (ArgumentException exception)
            {
                throw new InvalidDataException($"{name}.json is not valid JSON for {typeof(T).Name}: {exception.Message}", exception);
            }
            _cache[(name, typeof(T))] = data;
            return data;
        }

        /// <summary>The rows of a table file.</summary>
        public IReadOnlyList<TRow> Table<TRow>(string name) => Load<DataTable<TRow>>(name).rows;

        /// <summary>Forgets what was read, so the next call reads the files again.</summary>
        public void ClearCache() => _cache.Clear();

        /// <summary>Where the text comes from; tests and custom sources (downloads, mods) override it.</summary>
        protected internal virtual string ReadText(string name)
        {
            var file = Resources.Load<TextAsset>($"{ResourcesFolder}/{name}");
            return file != null ? file.text : null;
        }
    }
}
