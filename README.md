# Unity UI MVC Framework

Prepared source for the private repository `alieksieiev-dmytro/unity-ui-mvc-framework`.

This repository contains one package, `com.alieksieiev.ui` version `0.1.0`: persistent MVC screens, passive HUDs, stacked windows, ownership scopes, UI input, and reusable setup assets. Public namespace: `UI`. Runtime assembly: `Alieksieiev.UI`.

Input-action assets are supplied by each game. This repository contains no `.inputactions` asset. Assign a game-owned asset with the required UI map to `UIFrameworkInstaller`; the root prefab has no actions asset assigned.

Zenject is required and installed separately by each consuming game. This repository does not contain or automatically install Zenject source, DLLs, editor tools, or optional extras. Keep an existing compatible installation in place. The supported baseline is Zenject 9.2.0, Unity 6000.0.58f2, uGUI 2.0.0, and Input System 1.14.2.

Read [the numbered installation and maintenance guide](Packages/com.alieksieiev.ui/Documentation~/GettingStarted.md) for prerequisites, the single framework manifest entry, ProjectContext setup, scene ownership, registration, customization, upgrades, and rollback. No demo screens, example scenes, or sample projects are included.

## Release preparation

The UI-only package and `v0.1.0` tag are prepared locally. Nothing has been pushed. Blacksmith still uses its embedded UI package and an independent game-owned Zenject installation; switching the UI to a published Git dependency requires separate authorization.

Read [the release record](RELEASE.md) for the source revision, static checks, and verification limits. Package Manager resolution, Unity import, visuals, and actual scene transitions remain unverified.

The UI source is private; see its [license notice](Packages/com.alieksieiev.ui/LICENSE.md). Zenject's license stays with the consuming game's independent installation.
