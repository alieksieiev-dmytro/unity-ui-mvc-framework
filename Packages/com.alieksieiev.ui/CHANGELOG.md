# Changelog

## 0.1.0 - 2026-10-26

- Extract the MVC screen framework into its own assembly and namespace `UI`.
- Install a persistent UI root through Zenject ProjectContext.
- Add project and scene screen-registration scopes with deterministic cleanup.
- Preserve controller-type screen operations, caching, model resets, passive HUDs, window stacking, selection restoration, and top-window Cancel routing.
- Isolate UI input in an owned runtime action map and keep gameplay control policy in the consuming game.
- Include the root prefab and installation and maintenance documentation. Input-action assets are supplied by the consuming game.
- Keep Zenject as a separately managed project prerequisite; exclude its implementation from the framework repository.

Initial local preparation; publication and Unity runtime verification are separate.
