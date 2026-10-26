# Installing and maintaining the UI MVC framework

## 1. Understand the package and its prerequisite

A Unity package is a versioned folder of reusable code and assets. A game's `Packages/manifest.json` tells Unity which packages to install. Each package's own `package.json` identifies it and lists its package-managed dependencies.

This private repository contains one package: `Packages/com.alieksieiev.ui`, version `0.1.0`. The repository's `v0.1.0` tag identifies the reviewed UI snapshot. Zenject source, binaries, editor tools, and optional extras are not included in this repository.

The package's public namespace is `UI`; the assembly name is `Alieksieiev.UI`. Use `using global::UI;` in a game that already has a namespace named `UI` beneath its own namespace.

The first supported baseline is Unity 6000.0.58f2, uGUI 2.0.0, Input System 1.14.2, and an independently installed Zenject 9.2.0. Other Unity releases and target platforms have not been verified. This uses Canvas-based uGUI, rather than UI Toolkit.

Zenject remains a required assembly dependency. It is managed by the consuming game, separately from the UI framework. The UI package manifest declares only uGUI and Input System, allowing an existing Zenject installation under Assets or a separately installed Zenject package to supply the assembly.

The initial release is prepared locally. The Git URL below is an installation instruction for the published release; it is not evidence that publication or a Unity installation has already happened.

## 2. Prepare private GitHub access

1. Install Git for Windows and ensure `git --version` works in a new terminal.
2. Ensure your GitHub account has read access to `alieksieiev-dmytro/unity-ui-mvc-framework`.
3. Authenticate Git outside Unity using Git Credential Manager. Unity Package Manager cannot interactively ask for GitHub credentials.
4. After publication, run `git ls-remote https://github.com/alieksieiev-dmytro/unity-ui-mvc-framework.git refs/tags/v0.1.0`. It should return the release commit and tag reference.
5. Never place passwords or access tokens in manifest URLs or commit them to either repository. Another computer or build machine needs its own Git authentication.

See [Unity's Git package and authentication documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-git.html).

## 3. Install or retain Zenject separately

A project must contain exactly one Zenject implementation. The supported baseline is the Zenject 9.2.0 installation already used by Blacksmith, providing the `Zenject` assembly and its `Zenject-usage` DLL.

1. If the game already has that installation, keep it. Installing the UI framework does not require removing or replacing it.
2. For a new game, install Zenject 9.2.0 from your independently managed dependency source before adding the UI framework. That can be a Unity asset import or a separate package; its source and version belong to the game's dependency setup.
3. Keep only one implementation. Do not combine a copy under Assets with a second Zenject/Extenject package.
4. If deliberately changing Zenject versions or distributions, back up the game and compare asset GUIDs before migrating existing ProjectContext and SceneContext references. The UI package does not perform that migration.

Blacksmith currently keeps its preserved Zenject installation as a separate, game-owned embedded package `com.mathijsbakker.extenject` 9.2.0. That folder is not exported into the UI framework repository. No separate Zenject repository is created by this UI change.

Zenject is not automatically downloaded by the UI package. Its existing assembly reference remains required, and missing Zenject will produce assembly/type errors until the game installs it separately. Zenject's own license and notices stay with its independent installation.

## 4. Install the UI framework

After installing Zenject, close Unity and open the game's `Packages/manifest.json`. Merge this entry into its existing `dependencies` object, preserving all other entries:

```json
{
  "dependencies": {
    "com.alieksieiev.ui": "https://github.com/alieksieiev-dmytro/unity-ui-mvc-framework.git?path=/Packages/com.alieksieiev.ui#v0.1.0"
  }
}
```

This is a merge snippet, not a replacement for the entire game manifest. Preserve any `scopedRegistries`, the game's independent Zenject dependency, and all other settings. The UI package declares uGUI 2.0.0 and Input System 1.14.2 as package dependencies. It does not declare a specific Zenject package ID or force a registry lookup for a separately installed asset.

An embedded `com.alieksieiev.ui` folder overrides the Git dependency. Remove that UI folder before switching to Git installation. Keep the game's independent Zenject installation. Do not keep two authoring copies of the UI package under the same Unity project's Assets or Packages folders.

Let Unity maintain `Packages/packages-lock.json` when it resolves dependencies, and commit the resolved lock file with the manifest. Blacksmith's current lock entries describe its embedded UI and separate Zenject installation; they do not prove a Git download or Package Manager resolution.

## 5. Configure the persistent framework

1. Enable Input System Package (New), or Both, in the game's Active Input Handling setting.
2. Reuse the game's `Assets/Resources/ProjectContext.prefab`. If absent, create one with Zenject's **Edit > Zenject > Create Project Context** command. There must be only one Resources ProjectContext prefab.
3. Add the package's `UIFrameworkInstaller` component to that prefab and add the component to ProjectContext's **Mono Installers** list. Keep existing installers in the list.
4. Assign **Root Prefab** to the package's `Runtime/Prefabs/UIRoot.prefab`.
5. Assign **Input Actions** to a game-owned actions asset containing the required UI map. Reuse the game's existing asset or create one under its Assets folder. The required map/action names are listed in section 8; no input-actions asset is bundled with this package.
6. Remove scene-local copies of the UI root and EventSystem that this framework replaces. The persistent root already contains its EventSystem and input module.
7. Keep Zenject SceneContext in scenes that need gameplay services or scene-owned UI. Starting one of those scenes directly still creates ProjectContext through Zenject.

At runtime the installer creates the root as a child of the persistent ProjectContext. Do not also place the root in a scene or add another DontDestroyOnLoad owner. The framework installer rejects installation outside ProjectContext or a second local installation.

The supplied root uses a screen-space overlay Canvas, a 1920x1080 scaling reference, HUDs below Windows, and an input module that starts disabled until the framework configures it.

## 6. Choose each screen's owner

| Registration owner | Dependencies available to controllers | Lifetime |
| --- | --- | --- |
| ProjectContext scope | Project-wide services | Root, cache, and open state survive scene changes |
| SceneContext scope | Scene services and inherited project services | All owned screens and registrations end with that scene |

Register global menus through a project-owned startup service. Do not inject a scene's player, camera, or inventory into a global controller: ProjectContext does not own those services.

For scene UI, call `UISceneInstaller.Install(Container)` once from the game's scene installer. This creates a local `UIScreenScope`, shadowing the project scope for scene injection. Bind the game's UI startup service in that same scene container. Zenject automatically disposes the local scope when SceneContext ends.

The framework configures scope disposal at priority 100. This priority is inherited by scene containers; do not bind it again in the scene. Zenject disposes higher numeric priorities first. Keep scene dependencies used by screens below 100 so screens unsubscribe before their services disappear.

The framework uses one controller instance per controller type across the application. Two simultaneous scopes cannot register the same controller type. This also applies to additive scenes. Use different controller types for independent screens, or keep genuinely shared screens in the global scope.

## 7. Build and register game-owned screens

The package intentionally supplies no example screens or scenes. A game supplies its own models, controllers, views, and inactive screen prefabs:

- Models implement `IUIScreenModel.Reset()`. The framework calls Reset whenever a closed screen reopens.
- HUD views/controllers derive from `UIHudView` and `UIHudController`.
- Window views/controllers derive from `UIWindowView` and `UIWindowController`.
- Controllers accept their typed model and view in the constructor, passing both to the base constructor. Zenject supplies their remaining game-service dependencies.
- Subscribe to game events in `OnOpen`; unsubscribe in `OnClose`. Use `OnDispose` for additional final cleanup.
- Call protected `RequestClose()` from a controller to ask the service to close it. Override `HandleCancel()` in a window controller to define dismissal behavior. Its default implementation does nothing.

Screen prefabs must start inactive. Their root view requires Canvas and CanvasGroup, inherited Canvas sorting, and normal parent CanvasGroup behavior. A window additionally needs GraphicRaycaster and a direct, full-screen background Image assigned to its view; its optional default Selectable must be inside that window. HUDs are passive: their graphics do not intercept input.

Inject `UIScreenScope` into the owning startup service. Its registration operations are:

```csharp
screens.RegisterHud<TModel, TView, TController>(prefab);
screens.RegisterWindow<TModel, TView, TController>(prefab);
```

These are API signatures; replace the type parameters with the game's concrete types. Register all required screens before calling `IUIService.Open<TController>()`. Bind startup services through Zenject's `IInitializable`, after the framework's input initialization at priority -50. Blacksmith starts its HUDs at -40.

Zenject creates the model, clones and injects the view, and constructs the controller through the owning scope's container. Keep scene-controller registrations in the local scope. No manual controller construction, root parenting, or manual scope disposal is needed with the supplied installers.

If the game uses its own assembly definition, add references to `Alieksieiev.UI`, `Zenject`, and any Unity assemblies its own screen code uses. Games in Assembly-CSharp can use the package's auto-referenced assembly.

## 8. Open, close, and connect gameplay policy

Inject `IUIService` into game services. Use `Open<TController>()`, `Close<TController>()`, and `IsOpen<TController>()`. Open creates a screen on first use and subsequently reuses it. Close hides it without destroying it. Reopening an already open window brings it to the top; reopening a closed screen resets its model.

Only the top window receives interaction and Cancel. Lower windows retain their selection for restoration. HUDs stay below windows and never block pointer input. Cancel with no open window does nothing.

Subscribe a game-owned bridge to `WindowStackChanged`, then immediately apply `HasOpenWindows` to the game's control policy. Unsubscribe on disposal. The package does not lock the cursor, pause time, or control a player.

In Blacksmith, `UIPlayerControlBridge` initializes at -70, before PlayerControlService at -60. It sets the initial blocked state before that service captures the cursor, and updates it when the window stack changes. The existing recapture frame guard prevents the closing input from reaching gameplay. The bridge unsubscribes at disposal priority 90, after screen cleanup and before player services.

The framework owns a runtime clone of only the supplied UI map. It never enables or disables the game's Player map. Required action names are `UI/Point`, `Click`, `RightClick`, `MiddleClick`, `ScrollWheel`, `Navigate`, `Submit`, `TrackedDevicePosition`, `TrackedDeviceOrientation`, and `Cancel`. Preserve these names while changing their bindings in a game-owned asset. Cancel is handled once by the framework, with the EventSystem module's Cancel reference unset.

Runtime rebinding and per-player device ownership are not provided by this release. Asset binding changes are read when the framework starts.

## 9. Customize safely

Create a prefab variant under the game's Assets folder to change the package root's scale settings or hierarchy configuration. Keep the required HUD/Window layer structure and EventSystem links. Assign that variant to the game's framework installer.

Create and maintain input-action assets in the game's Assets folder. Assign the game-owned asset to the framework installer and change bindings there. The package root prefab has no actions asset assigned; the input service configures its disabled module from the installer at runtime.

Do not modify `Library/PackageCache`: it is a downloaded, replaceable copy. Game-specific screen prefabs, artwork, models, controllers, and gameplay bridges belong in the game repository.

## 10. Update or roll back one game

1. Commit or back up the game before changing package versions.
2. Read the target release's changelog and migration instructions.
3. Change the UI Git entry to the selected framework release tag. Keep the separately managed Zenject installation at its current version unless an explicit dependency migration is required.
4. Let Package Manager resolve the manifest. Review and commit both manifest and generated lock changes.
5. To roll back, restore the previous manifest and its corresponding lock file from Git, or select the previous release tag and resolve again.

Never move a published release tag to different contents. Create a new version and tag for changes. During 0.x development, document breaking API changes in a new minor version, and use patch versions for compatible fixes. Other games stay on their selected releases until explicitly updated.

## 11. Develop the framework locally

Clone the private framework repository outside the consuming Unity project. Temporarily replace the UI Git entry with a `file:` reference to its `Packages/com.alieksieiev.ui` folder. Leave the independent Zenject installation in place. Local paths are relative to the game's `Packages/manifest.json` directory, or can be absolute with forward slashes. A folder dependency uses `file:`, without `//`.

Edit the clone's UI package source, not the downloaded package cache. Keep exactly one UI package source in the Unity project. Changes are visible to every game currently pointing at that same local clone; use this only deliberately during development.

For the initial Blacksmith implementation, the committed embedded UI folder is the framework authoring source. The separate local repository is a prepared UI-only release snapshot. Before publication, synchronize and compare its UI package with that reviewed embedded folder. After switching Blacksmith to Git dependencies, the separate repository becomes the authoring source.

Keep local development paths out of a published game commit. Before releasing, commit the package change, update its version and changelog, create a new immutable repository tag, and switch the game back to that tag. Review changes in the game separately from package publication.

## 12. Verification boundary and troubleshooting

This task permits external C# compilation and static asset/reference checks. It does not permit Unity import checks, Play Mode, visual checks, or scene-transition validation. Those behaviors remain unverified until separately exercised by the project owner.

- Authentication failure: confirm terminal Git access to the private repository first.
- Missing Zenject dependency: install the supported Zenject version separately and ensure its `Zenject` assembly and usage DLL are available. The UI repository does not supply or automatically install them.
- Old package still active: check for an embedded folder or local `file:` override with the same package ID.
- Missing UI action: restore the required UI map/action names in the game-owned asset, and ensure that asset is assigned to the framework installer.
- Duplicate controller registration: check whether a scene registered a type already owned by a live scene or the global scope.
- Missing scene service: register that screen in the scene scope, not the project scope.
- Gameplay continues receiving input through a window: connect and initialize the game-owned window-state bridge.
- Scene HUD remains alive: check that the scene installed a local scope and registered its startup service in that SceneContext.

On scope disposal, the registry first removes its creators, then the service removes cached instances and windows. Cleanup continues across controller-disposal exceptions, and the remaining stack regains selection. A destroyed scope cannot register or reopen its screens; another live scope may register that controller type after cleanup.
