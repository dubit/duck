# Contributing #

## Conduct ##
Please refer to [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)

## Process ##
All contribution is done via forking & pull requests. To get changes merged there must be approval from at least 2 developers at Dubit. Reviews from external developers are welcome and encouraged.

### Reviewing ###
All reviewers must thoroughly check through any requested changes and comment on points of contention. It's recommended that the changes are checked out, compiled, run and pass any tests before carrying out a more in depth review.

### Communication
Although we cannot guarantee that changes are reviewed and approved/merged within a particular time frame, we do aim to respond with some communication within 7 days of a new pull request. Communication between contributors and maintainers should be polite & well mannered.

### Releases ###
We will maintain a 2 week release cycle. Every 2 weeks we will check if any changes have been merged and make a release if warranted. At this point any pending pull requests will be noted and reviews scheduled. These may not be full in-depth reviews, it will depend on the volume of pull-requests and commitments of the Dubit developers.

## Style Guide ##
To keep things readable and consistent please follow our style guide. It is mainly based upon MSDN's guide, but adapted for unity and to conform with existing practises at Dubit.

## Requirements ##
New features or changes to existing features must meet a set of requirements before being reviewed & accepted.

### Documentation ###
Each feature should have it's own basic documentation in the feature's directory or repository under a Docs folder in the form of a structure of markdown files with links between them. On top of this there should be inline C# documentation on all public APIs as a minimum. Non public API documentation is also encouraged and inline comments where necessary.

### Tests ###
The Unity 5.3+ editor comes with test tools built in. We try to maximize test coverage of all features. Anything that can be unit tested, should be unit tested. Test coverage is an on going part of our maintenance.