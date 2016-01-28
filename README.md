# OctopusDeploy-Kraken

[![Build status](https://ci.appveyor.com/api/projects/status/pc3qmlqlqne6n1jy/branch/master?svg=true)](https://ci.appveyor.com/project/JohnCruikshank/octopusdeploy-kraken/branch/master)

Kraken is a companion tool for Octopus Deploy that facilitates deploying multiple projects at once. Kraken uses the Octopus Deploy API and even authenticates users using Octopus rather than keeping its own user store.  Kraken maintains a collection of release batches. A release batch is a collection of project releases that can be deployed to a target environment with a single command. All deploys are triggered at once, allowing the built-in Octopus deploy queue to work through them.

### Features

* Maintain release batches
* Deploy release batches using Octopus
* Sync releases from an environment in Octopus
* Sync releases from the latest releases in Octopus
* Sync releases from the latest NuGet packages
* Octopus login (no new user accounts to deal with)
* Installed using Octopus (because why not)

### Installation

See [Installation Guide](https://github.com/Zywave/OctopusDeploy-Kraken/wiki/Installation)

### Getting started

You will quicly notice that there is very little in the way of UI for maintaining batches. That is because all of the functionality is exposed via commands in [cmdrjs](https://github.com/cmdrjs). If you click **Manage** or press the (~/`) key (Quake style), you will be presented with a console.  This console is where you will manage batches. There are various commands available to you and I would recommend using the help (/?) feature of the commands to determine usage.  

Once you have created a batch and linked projects to that batch, you can go ahead and sync from the appropriate source and subsequently deploy by using more commands or using the menus in the UI (which simply invoke commands for you).

Keep in mind that a sync is a point in time sync and there is nothing that will automatically keep batches up to date.

### Project status

The project is currently in alpha and under active development, which means releases may not be stable or long lived. We would love feedback.
