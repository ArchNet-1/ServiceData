<p align="center">
  <a href="https://archnet-plateform.fr"><img src="https://archnet-plateform.fr/ArchNetLogoWithBck.png" alt="ArchNet" width="240"></a>
</p>

# ArchNet · Data Service

Your game data (items, enemies, levels, dialogues…) as JSON tables you edit in a window, read in one line with typed classes.

- Unity **6000.0** or newer · Needs [ArchNet Core](https://github.com/ArchNet-1/ArchNetCore)
- Part of [ArchNet](https://archnet-plateform.fr), free and open-source Unity packages

## Add it to your game (one click)

*ArchNet > Hub > Services > Data > Add to game* creates, in `Assets/ArchNet/GameData/`:

| File | What it is |
|---|---|
| `Resources/ArchNetData/Items.json` | An example table: `{ "rows": [ { "id": "sword", "name": "Iron sword", "price": 120 }, … ] }` |
| `Scripts/ItemRow.cs` | One row of that table (fields named like the JSON keys). |
| `Scripts/GameData.cs` | The service: one property per table (`Items`, `Item("sword")`). |
| `Prefabs/GameData.prefab` | Under the ArchNet Bootstrap. |

## Use

```csharp
foreach (ItemRow item in Services.Get<GameData>().Items) …
ItemRow sword = Services.Get<GameData>().Item("sword");
```

A new table: **Open tool > New table** (e.g. `Enemies.json`), a row class (`EnemyRow`), and one line in `GameData`:

```csharp
public IReadOnlyList<EnemyRow> Enemies => Table<EnemyRow>("Enemies");
```

Files are read once, then cached (`ClearCache()` to read them again). A missing or broken file says which one and why. Override `ReadText(name)` to read tables from elsewhere (a download, mods).

**Open tool**: pick a table, edit it, **Save** checks the JSON before writing.

## License

MIT, see [LICENSE.md](LICENSE.md).
