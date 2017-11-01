# Dubit Unity Component Kit (DUCK)
![dubiduck_256.png](https://bitbucket.org/repo/KLedqM/images/461945451-dubiduck_256.png)

# Version: v2.0.0

## What is it ?
Duck is a suite of commonly used components and extensions for unity game development, intended to be used within a larger game project to limit code duplication across projects and promote good practice by setting a standard and encouraging decoupled independent components. The project is split accross multiple repositories. This repo contains the core features & utilities. New features can be pull requested into here, and if they become sizable, they can be moved into their own repository.

There are also useful editor tools and extensions that can speed up development and bypass monotonous tasks

## Requirements

* Unity 5.3 or newer

## Philosophy

DUCK uses the philosophy set out by unity. This means making heavy use of the editor where possible and using MonoBeahviour, ScriptableObject and SerializableObject. Other approaches which wrap these up or reinvent the wheel in attempts to gain more control often result in over abstraction, and codebases that are far more difficult to jump into for new developers, (especially those who already have experience with unity). Using MonoBehaviours over objects away from the scene, make it easier to reuse components, since they can be added in the editor

We also want to promote using composition (component based design), over deep inheritance hierarchies, because it is easier to reuse smaller bits of functionality and promotes the single responsibility principle.

## Contribution & Workflow

All Contribution is done via pull request using gitflow.

## Tests

The Unity 5.3+ editor comes with test tools built in. We try to maximize test coverage of all features. Anything that can be unit tested, **should** be unit tested. Test coverage is an on going part of our maintenence.

## Documentation

Each feature should have it's own basic documentation in the feature's directory or repository under a `Docs` folder in the form of a structure of markdown files with links between them. On top of this there should be inline c# Documentation on all public apis as a minimum. Non public api documentation is also encouraged and inline comments where necessary.
