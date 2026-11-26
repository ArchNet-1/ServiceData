// ArchNet · Data Service — Data tool (ArchNet Hub > Services > Data > Open tool)
// Lists the JSON tables, edits one in a text box with a syntax check before saving, and makes
// new tables from a template.
// Docs: https://archnet-plateform.fr/#ServiceData

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArchNet.Service.Editor
{
    public class DataWindow : EditorWindow
    {
        const string StylePath = "Packages/com.archnet.hub/Editor/UI/Hub.uss";
        const string NewTable = "{\n    \"rows\": [\n        { \"id\": \"first\" }\n    ]\n}\n";

        [Serializable]
        class AnyRow { public string id; }

        [SerializeField] string _selected;
        TextField _editor;
        Label _status;

        public static void Open()
        {
            var window = GetWindow<DataWindow>();
            window.titleContent = new GUIContent("Data tables");
            window.minSize = new Vector2(620, 380);
        }

        void CreateGUI() => Rebuild();

        void Rebuild()
        {
            VisualElement root = rootVisualElement;
            root.Clear();
            root.AddToClassList("archnet-hub");
            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (style != null) root.styleSheets.Add(style);
            root.style.paddingLeft = root.style.paddingRight = root.style.paddingTop = 8;

            string[] files = Directory.Exists(DataRecipe.TablesFolder)
                ? Directory.GetFiles(DataRecipe.TablesFolder, "*.json").Select(f => f.Replace('\\', '/')).OrderBy(f => f).ToArray()
                : new string[0];
            if (_selected == null || !files.Contains(_selected)) _selected = files.FirstOrDefault();

            var toolbar = new VisualElement();
            toolbar.AddToClassList("row");
            toolbar.AddToClassList("toolbar");
            var tables = new DropdownField(files.Select(Path.GetFileNameWithoutExtension).ToList(), Array.IndexOf(files, _selected));
            tables.style.minWidth = 180;
            tables.RegisterValueChangedCallback(evt =>
            {
                _selected = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == evt.newValue);
                Rebuild();
            });
            toolbar.Add(tables);
            toolbar.Add(new Button(CreateTable) { text = "New table" });
            var spacer = new VisualElement();
            spacer.AddToClassList("grow");
            toolbar.Add(spacer);
            var save = new Button(Save) { text = "Save" };
            save.AddToClassList("primary");
            save.SetEnabled(_selected != null);
            toolbar.Add(save);
            root.Add(toolbar);

            var help = new Label("A table is { \"rows\": [ … ] }; each row's keys match the fields of its C# row class (see ItemRow.cs). Read it with Table<YourRow>(\"FileName\").");
            help.AddToClassList("intro");
            root.Add(help);

            _editor = new TextField { multiline = true, value = _selected != null ? File.ReadAllText(_selected) : "" };
            _editor.style.flexGrow = 1;
            _editor.SetEnabled(_selected != null);
            var scroll = new ScrollView();
            scroll.style.flexGrow = 1;
            scroll.Add(_editor);
            root.Add(scroll);

            _status = new Label(_selected == null ? $"No table in {DataRecipe.TablesFolder}. Add the Data service from ArchNet > Hub first." : _selected);
            _status.AddToClassList("status-bar");
            root.Add(_status);
        }

        void Save()
        {
            string json = _editor.value;
            try
            {
                JsonUtility.FromJson<DataTable<AnyRow>>(json); // throws on broken JSON
            }
            catch (ArgumentException exception)
            {
                _status.text = $"Not saved: the JSON is broken ({exception.Message}).";
                _status.AddToClassList("status--error");
                return;
            }
            File.WriteAllText(_selected, json);
            AssetDatabase.ImportAsset(_selected);
            _status.RemoveFromClassList("status--error");
            _status.text = $"Saved {_selected}. Restart Play Mode (or call ClearCache()) to read it again.";
        }

        void CreateTable()
        {
            string path = EditorUtility.SaveFilePanelInProject("New data table", "Enemies", "json", "Name of the table (one row class per table).", DataRecipe.TablesFolder);
            if (string.IsNullOrEmpty(path)) return;
            File.WriteAllText(path, NewTable);
            AssetDatabase.ImportAsset(path);
            _selected = path;
            Rebuild();
        }
    }
}
