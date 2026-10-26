# UI MVC Framework

Private Unity package `com.alieksieiev.ui`, version `0.1.0`. Public namespace: `UI`. Runtime assembly: `Alieksieiev.UI`.

The framework manages persistent UI, cached MVC screens, passive HUDs, stacked windows, selection, and UI input. Each screen belongs to a project or scene scope. Scene screens are destroyed when their scope ends; global screens survive scene changes.

Baseline: Unity 6000.0.58f2, uGUI 2.0.0, Input System 1.14.2, and an independently installed Zenject 9.2.0. This repository contains only the UI package. Install Zenject separately before installing the framework; existing compatible installations can remain in place.

Input-action assets belong to each game. Assign a game-owned actions asset with the required UI map to `UIFrameworkInstaller`; the package does not include an `.inputactions` asset.

Read [the numbered setup and maintenance guide](Documentation~/GettingStarted.md) before installing or migrating an existing game. The package contains setup assets, with no demo screens, sample scenes, or game-specific HUDs.

The initial release is prepared locally. Remote installation URLs become usable after the private repository and `v0.1.0` tag are published. Unity import, Package Manager resolution, visuals, and scene transitions have not been validated in Unity.
