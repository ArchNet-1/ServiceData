// ArchNet · Data Service — Data recipe (ArchNet Hub > Services > Data)
// Creates, under Assets/ArchNet/GameData/: Scripts/ItemRow.cs and Scripts/GameData.cs (your
// classes), Resources/ArchNetData/Items.json (an example table) and Prefabs/GameData.prefab,
// placed under the ArchNet Bootstrap.
// This assembly only compiles when the ArchNet Hub (com.archnet.hub 2.0.0+) is installed.
// Docs: https://archnet-plateform.fr/#ServiceData

using System;
using ArchNet.Core.Editor;
using ArchNet.Installer.Editor;
using UnityEditor;
using UnityEngine;

namespace ArchNet.Service.Editor
{
    public sealed class DataRecipe : ServiceRecipe
    {
        public const string RecipeId = "data";
        const string Templates = "Packages/com.archnet.service.data/Editor/Recipe/Templates/";
        const string ServiceScript = "Scripts/GameData.cs";
        const string PrefabFile = "Prefabs/GameData.prefab";
        public const string TablesFolder = RecipeRunner.RootFolder + "/GameData/Resources/" + DataService.ResourcesFolder;

        public override string Id => RecipeId;
        public override string DisplayName => "Data";
        public override string Description => "Your game data (items, enemies, levels…) as JSON tables edited in a window, read with typed classes: Services.Get<GameData>().Items.";
        public override string Category => "Data";
        public override string FolderName => "GameData";
        public override string[] RequiredServices => new[] { Core.Editor.BootstrapRecipe.RecipeId };
        public override bool HasTool => true;

        public override void Create(RecipeContext context)
        {
            context.WriteTemplate("Scripts/ItemRow.cs", RecipeContext.LoadTemplate(Templates + "ItemRow.cs.txt"));
            context.WriteTemplate(ServiceScript, RecipeContext.LoadTemplate(Templates + "GameData.cs.txt"));
            context.WriteText($"Resources/{DataService.ResourcesFolder}/Items.json", RecipeContext.LoadTemplate(Templates + "Items.json.txt"));
        }

        public override void AfterScriptsCompiled(RecipeContext context)
        {
            Type serviceType = RecipeTools.GeneratedType<DataService>(context, "GameData", ServiceScript);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(context.PathOf(PrefabFile))
                                ?? context.SavePrefab(PrefabFile, new GameObject("GameData", serviceType));
            Core.Editor.BootstrapRecipe.AddUnderBootstrap(prefab);
        }

        public override void OpenTool() => DataWindow.Open();
    }
}
