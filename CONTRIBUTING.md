AppCore .NET Contributor Guide
------------------------------

The following guidelines aim to help ensure the smooth running of the project, and keep a
consistent standard across the codebase. There might be good reasons to deviate from them -
please try to adhere to them as closely as possible.

If you want to request a new feature or report a bug please create an issue.

Contributing code or documentation to AppCore .NET is higly appreciated, for that matter we
welcome pull requests.

## Process

**Please file issues in the repository containing the code in question** (e.g. If the issue is
with the AppCore Logging framework, file it in that repo).

### Reporting feature requests, bugs

Either request a feature or note a defect. If it's a feature, explain the challenge you're
facing and how you think the feature should work. If it's a defect, include a description
and steps, ideally with code or unit tests, how to reproduce.

### Contributing code

If there is'nt any issue for the code you are willing to contribute please create one
(please see above).

- **Discussion**
  
  For feature requests, some discussion on the issue will take place to determine
  if it's something that should be included or be a user-supplied extensions. It's very
  likely that the design of the new feature will also be discussed.

  For bugs, there might be a discussion, wheter the issue is a bug or intended behavior, how
  to resolve it without breaking anything else, and the project version where the fix should
  be included.

- **Pull request**

  To submit changes to the code, create a [pull request](https://help.github.com/articles/using-pull-requests/)
  targeting the branch where the issue should be resolved. 

  For new features, the `dev` branch should be targeted.
  For bug fixes, the `hotfixes/<version>` branch should be the target of the pull request. 
  See [A successful Git branching model](https://nvie.com/posts/a-successful-git-branching-model/)
  for a guide on how our branching model works.

  Pull requests need to pass the CI build and should include unit tests to verify the work.

- **Code review**
  
  Before the code is integrated, there will be a review by any of the project members. Some iteration may
  take place requiring updates to the pull reuqest. For example, adding tests, fixing some errors, ...

- **Pull request acceptance**

  The pull request will be merged into the target branch and pushed to the next feature or hotfix release.

## License

By contributing documentation or code to the AppCore project, you assert that:

1. The contribution is your own original work.
2. You have the right to assign the *copyright* for the work.
3. You license it under the terms applied to the AppCore project.

## Coding

The AppCore .NET project uses [A successful Git branching model](https://nvie.com/posts/a-successful-git-branching-model/)
for handling releases. Please make sure your pull request targets the correct branch.

For versioning our libraries and packages we use [semantic versioning](https://semver.org/). Release versions
are calculated from the branching history of the repository, using [GitVersion](https://gitversion.net/).

### Developer Environment

- Visual Studio 2019
- .NET Core SDK 2.1 and 3.1
- PowerShell 5+

### Build / Test

Project repositories generally have a `build.ps1` script. This PowerShell script can be used to build the code,
execute tests and create packages.

The build script accepts the argument `--target` which specifies the action to execute. The most important targets are:

- **build** - This is the default target, it will restore dependencies and build all sources (except tests).
- **test** - Builds the tests and executes them. Test results and code coverage reports can be found in the `artifacts`
  directory.
- **publish** - Creates packages and publishes them to the `artifacts` directory.

Unit tests are written in XUnit using FluentAssertions and NSubstitute. **Please note that, code contributions should include tests.**

Furthermore **everything should build and test with no warnings or errors**.

### Coding standards

The source code uses four spaces for indents. We use [EditorConfig](https://editorconfig.org/) and/or [ReSharper](https://www.jetbrains.com/resharper/)
to ensure consistent formatting.

Usally we apply standard .NET coding guidelines. See the [Framework Design Guidelines](https://msdn.microsoft.com/en-us/library/ms229042.aspx) for suggestions.
