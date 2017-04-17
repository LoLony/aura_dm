Aura
==============================

Aura is an open-source MMORPG server software.
It's solely being developed for educational purposes, learning about programming, MMORPGs,
maintaining huge projects, working with other people, and improving knowledge.
It's not about playing a game or competing with any services provided by
Nexon or its partners, and we don't endorse such actions.

Aura is completely free and licensed under the GNU GPL.
As such, every user is free to use Aura and choose how to use it,
in the context of its license.

Compatibility
------------------------------
Aura is only compatible to the latest version of NA,
compatibility to all other versions was dropped on
2013-09-13 (in Aura Legacy).

Requirements
------------------------------
To *run* Aura, you need
* .NET 4.5 (Mono 3.2.7 or above)
* MySQL 5 compatible database

To *compile* Aura, you need
* C# 5 compiler, such as:
  * [Visual Studio](http://www.visualstudio.com/en-us/products/visual-studio-express-vs.aspx) (2012 or later)
  * [Monodevelop](http://monodevelop.com/) (With Mono version 3.2.7 or greater)
  * [SharpDevelop](http://www.icsharpcode.net/OpenSource/SD/) (Version 4.4 or greater)

Installation
------------------------------
* Compile Aura
* Run `sql/main.sql` to setup the database
* Copy `system/conf/database.conf` to `user/conf/`,
  adjust the necessary values and remove the rest.

Afterwards, you should be able to start Aura via the provided scripts or
directly from the bin directories. If not, or if you need a more detailed guide,
head over to our [forums](http://aura-project.org/forum/), [Gitter chat](https://gitter.im/aura-project/aura), or [wiki](https://github.com/aura-project/aura/wiki).

Contribution
------------------------------
There are 4 ways **you** can help us to improve Aura:

1. Research
2. Bug reports
3. Pull Requests
4. Releases on the forums

### 1. Research
Do research on NPCs, quests, skills, anything really that isn't implemented yet and
post it on our [research forum](http://aura-project.org/forum/forum/36-research/).
The information you post will help developers to implement the features. 
You can also keep a lookout for GitHub issues where we
[need help](https://github.com/aura-project/aura/issues?q=is%3Aissue+is%3Aopen+label%3A%22help+wanted%22)
or information.

### 2. Bug reports
Report bugs on [GitHub](https://github.com/aura-project/aura/issues), so they can be fixed ASAP.

### 3+4. Code
The fastest way to get code contributions into the source is a pull request, which,
if well written, can be merged right into *master*. To expediate this process, 
all pull requests must comply with our coding conventions below.

Alternatively you can make "casual" releases on the forum, which developers might pick up
as research or as a base to implement the features into the official source.

#### Coding conventions
* Base: [MS Naming Guidelines](http://msdn.microsoft.com/en-us/library/xzf533w0%28v=vs.71%29.aspx), [MS C# Coding Conventions](http://msdn.microsoft.com/en-us/library/ff926074.aspx)
* Exceptions:
  * Use `_private` for private fields and `this.Foobar` for properties, public fields, and methods.
  * Use tabs, not spaces.
* Comment lines shouldn't exceed ~80 characters, other lines' lengths are irrevelant.
* Excessive usage of the auto-formatting feature is encouraged. (Default VS settings)
* Avoid overuse of regions.

Common problems
------------------------------

### Errors after updating Aura
Usually all errors are solveable by recompiling and deleting the cache folder.

### I can't move/am naked after login
This usually happens when the server and the client aren't compatible to
each other. Make sure you're running the latest version of Aura and *NA*.

If you did update both and are still running into problems, an official update
probably broke compatibility. Wait a few hours for us to update Aura
if NA was just updated or create an issue on [GitHub](https://github.com/aura-project/aura/issues).

Links
------------------------------
* Forums: http://aura-project.org/
* GitHub: https://github.com/aura-project
* Gitter chat: https://gitter.im/aura-project/aura
* Backlog: [https://github.com/aura-project/aura/issues](https://github.com/aura-project/aura/issues?utf8=%E2%9C%93&q=is%3Aissue+is%3Aopen+label%3Abacklog)
* Wiki: https://github.com/aura-project/aura/wiki

Build Status
------------------------------
[![Build Status](https://travis-ci.org/aura-project/aura.png?branch=master)](https://travis-ci.org/aura-project/aura)
