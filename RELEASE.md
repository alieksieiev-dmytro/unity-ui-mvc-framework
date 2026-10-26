# Prepared UI-only release v0.1.0

Source: Blacksmith T-14 commit `0c66d5752552ac3fd828df018673694148ec350c`.

This snapshot contains only `com.alieksieiev.ui` 0.1.0. Zenject is a separate project prerequisite, with supported baseline 9.2.0. No Zenject implementation or binary is included in the snapshot or its initial Git history.

UI package SHA-256 tree digest: `c058fe71edb6b4056230178cd18221cde1636f62ee1a42eddf74bc3b0c82bd59`. The digest uses sorted relative paths, a NUL separator, and each file's binary SHA-256 digest. Package files match Blacksmith's reviewed embedded UI package byte-for-byte.

External Roslyn compilation passed for the UI framework (15 sources) and Blacksmith (58 sources), using the game's separate Zenject dependency. Static checks confirm that the framework manifest declares only uGUI and Input System, the assembly still references Zenject, and all 674 original game-owned Zenject files are unchanged.

No separate test/check source files were added. Unity was not used for validation. Package Manager resolution, import, visuals, target-platform builds, and actual scene transitions remain unverified.

The former two-package draft and its local tag were preserved in the sibling TaskRecovery directory before preparing this clean UI-only snapshot. No existing game task history was rewritten. Publishing this prepared tag requires a separate user instruction; Blacksmith remains on its task branch with T-14 In Progress.

Input-action assets are game-owned and are excluded from this snapshot and its initial Git history. The root prefab has no actions asset reference, and Blacksmith retains its existing installer assignment. This follow-up changes assets and documentation; the 15 framework C# sources match the previously compiled snapshot. The preceding draft was preserved in TaskRecovery before rebuilding this local candidate.
