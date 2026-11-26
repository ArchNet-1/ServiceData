# Changelog

## [2.0.0] - 2026-11-25

### Renamed
- Repository `ServiceRessource` is now `ServiceData`, package id `com.archnet.service.ressource` is now the one in package.json. Remove the old id from Packages/manifest.json and install this one.

First version, numbered like the other ArchNet 2.0 packages.

### Added
- `DataService`: `Load<T>(name)` and `Table<TRow>(name)` from `Resources/ArchNetData/*.json`, cached; clear errors for missing or broken files; `ReadText` overridable.
- ArchNet Hub service **Data**: `ItemRow.cs`, `GameData.cs`, example `Items.json`, `GameData.prefab` under the Bootstrap; Data tables window.
- Edit Mode tests.
