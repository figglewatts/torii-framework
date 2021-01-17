# Torii Framework
A framework for Unity 2019.4+. Basically trying to make my life easier.

Torii is super easy to use. It comes as a Unity package, and even has a project
template that comes with it preinstalled.

The easiest way to get up and running is to download the project template and
install it in Unity, then create a new Unity project from the template.

Torii has a dedicated CLI application for integrating with its projects too.
You can find ToriiCLI here: https://github.com/Figglewatts/toriicli

With ToriiCLI you can build, release, and even
manage NuGet packages for your project. Try it out!

## Installation

### New project
1. Download [the file `com.figglewatts.torii.template.tgz`](https://github.com/Figglewatts/torii-framework/releases/latest/download/com.figglewatts.torii.template.tgz) from the releases section of this repository.
2. Place it in the following folder in your Unity installation: `{editor-path}/Editor/Data/Resources/PackageManager/ProjectTemplates`.
3. Now, when you create a new project in Unity, the option for a Torii Project will be there. If you create a Torii Project lke this then Unity will install the Torii package for you, as well as setting up Torii-specific project settings.

### Existing project
1. Place the following entry in your Unity project's `./Packages/manifest.json`, in among the rest of the `dependencies` section, replacing `{latest-version}` with the version number of [the latest release](https://github.com/Figglewatts/torii-framework/releases/latest), i.e. `0.2.0`:
   ```
   {
      "dependencies": {
          ...
          "com.figglewatts.torii": "https://github.com/Figglewatts/torii-framework.git?path=/Packages/com.figglewatts.torii#{latest-version}",
          ...
      }
   }
   ```
2. Save the file, when Unity detects that it has changed (when the Unity window is next focused if it's already open, or when you next open your project if not) it will download and install Torii into your project.
