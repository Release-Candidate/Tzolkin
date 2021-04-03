# F# Project Template

[![MIT license badge](https://img.shields.io/github/license/Release-Candidate/FSHARP_TEMPLATE)](https://github.com/Release-Candidate/FSHARP_TEMPLATE/blob/main/LICENSE)
[![Documentation Status badge](https://readthedocs.org/projects/fsharp-template/badge/?version=latest)](https://fsharp-template.readthedocs.io/en/latest/?badge=latest)
[![OS badge](https://img.shields.io/badge/Runs%20on-Linux%7COS%20X%7CWindows-brightgreen?style=flat)](https://dotnet.microsoft.com/download)
[![F# 5.0 badge](https://img.shields.io/badge/F%23-5.0-brightgreen?style=flat)](https://fsharp.org/)
[![.Net 5.0 badge](https://img.shields.io/badge/.Net-5.0-brightgreen?style=flat)](https://dotnet.microsoft.com/download)
[![Binder badge](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/Release-Candidate/FSharp_Template/main?filepath=TestFSharp.ipynb)
[more badges](#badges)



This is a cross platform Github template for F#, using [Paket](https://fsprojects.github.io/Paket/) as
Nuget package manager, [Fake](https://fake.build/) as build system, [MkDocs](https://www.mkdocs.org/)
to generate HTML documentation at [Read The Docs](https://readthedocs.org/),
[Expecto](https://github.com/haf/expecto), [Unquote](https://github.com/SwensenSoftware/unquote)
and [FsCheck](https://fscheck.github.io/FsCheck/) for the testing, [AltCover](https://github.com/SteveGilham/altcover) to generate
coverage reports for [CodeCov](https://about.codecov.io/), [FSharpLint](https://fsprojects.github.io/FSharpLint/)
as static code checker and [Github workflows](https://github.com/Release-Candidate/FSharp_Template/actions).
Includes a F# [Jupyter Notebook](https://jupyter.org/) with a [`postBuild`](./postBuild) script for [myBinder](https://mybinder.org/)
to be able to use C# and PowerShell Kernels for the notebooks too.

This template (and .DotNet 5.0) is tested and runs on Linux, Mac OS X and Windows.

## Table of Content

- [F# Project Template](#f-project-template)
  - [Table of Content](#table-of-content)
  - [Template Usage](#template-usage)
  - [Jupyter Notebook](#jupyter-notebook)
  - [Fake](#fake)
    - [Build Targets](#build-targets)
  - [MkDocs Files](#mkdocs-files)
  - [GitHub Workflows](#github-workflows)
  - [GitHub Issue Template](#github-issue-template)
- [Begin of the Template](#begin-of-the-template)
  - [Download](#download)
  - [Installation and Usage](#installation-and-usage)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Usage](#usage)
  - [Contributing](#contributing)
  - [License](#license)
  - [Badges](#badges)
    - [GitHub Workflows/Actions](#github-workflowsactions)
      - [Static Code Checkers](#static-code-checkers)
      - [Tests on various OSes](#tests-on-various-oses)
    - [External Websites](#external-websites)

## Template Usage

1. Replace the string `FSHARP_TEMPLATE` (and *_TEMPALTE and FHARP_*)
with the real project name(s), and my name too, if
you aren't me ;).
2. Add the Nuget key and the Codecov token as secrets
to the repository, named `CODECOV_SECRET` and `NUGET_PACKAGE`.
3. Rename, move and edit the two project files,
`src/LibTemplate/LibTemplate.fsproj` and `tests/TestsTemplate/TestsTemplate.fsproj`
Same with the source files in `src/LibTemplate`
and `tests/TestsTemplate`
4. Change this [`Readme.md`](./README.md)
5. Change the documentation in `docs/`
6. Add a solution file (`.sln`) in this root directory
7. Add the source project(s) in `src/` and the test
projects in `tests/` to the solution
8. Edit the GitHub workflow [`.github/workflows/create_packages.yml`](./.github/workflows/create_packages.yml)
to append your generated binaries to the releases

## Jupyter Notebook

There is an interactive F# [Jupyter Notebook](https://jupyter.org/) at [![Binder badge](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/Release-Candidate/FSharp_Template/main?filepath=TestFSharp.ipynb).

The file [`postBuild`](./postBuild) holds the configuration for the Docker image generation
at MyBinder, the Jupyter Notebook file is [`TestFSharp.ipynb`](./TestFSharp.ipynb). The
Notebook supports C# and Powershell too.

For more information, see [.Net Interactive](https://github.com/dotnet/interactive#notebooks-with-net)

## Fake

Before you can use the configured Tools of this template,
You have to download and install (aka. `restore`) the packages
of the tools.

1. First, download and install the "dotnet tools" using the command

    ```shell
    dotnet tool restore
    ```

    now you have installed Fake, Paket and FSharpLint,
configured in the file [`.config/dotnet-tools.json`](./.config/dotnet-tools.json)

2. Download and install ("restore") the Paket Nuget packages.

    ```shell
    dotnet paket restore
    ```

3. Delete the file `build.fsx.lock` and run Fake, to download and install (restore) it's nuget packages.

    ```shell
    dotnet fake run build.fsx
    ```

4. To generate the documentation using MkDocs, a virtual Python environment is needed. A virtual Python
environment is the same as the locally installed Nuget packages above.
So, first you need to install Python, if you don't
have it installed already - either from your distributions repository, using the XCode or [Homebrew](https://brew.sh/) version,
or getting it from [Python.org](https://www.python.org/downloads/).
In the file [`Pipfile`](./Pipfile) there is a stanza saying

    ```ini
    [requires]
    python_version = "3.9"
    ```

    That's just because I used 3.9 when generating that
template, and Pipenv is picky about the version mentioned
there. Just edit that to match your installed
Python version.
    Install `pipenv` using the package
manager pip

    ```shell
    pip install pipenv
    ```

    Now you're ready to download and install the needed
packages using pipenv

    ```shell
    pipenv install --dev
    ```

### Build Targets

* All build artifacts are generated in the directory `artifacts`, set using the expression
  `buildOutputPath` in [`build.fsx`](./build.fsx).
* NuGet packages generated by the target `Packages` are saved to the directory `packages`, set using the expression
  `packageOutputPath` in [`build.fsx`](./build.fsx).
* The test results of the targets `Tests` and `TestsDeb` are saved as a `.trx` file in the
  directory `test_results`, set using the expression  `testOutputPath` in [`build.fsx`](./build.fsx).
* The coverage report `coverage.xml` of the targets `TestsCoverage` and `TestsCoverageDeb`
  is saved to the directory `test_results`, set using the expression  `testOutputPath`
  in [`build.fsx`](./build.fsx).
* The output of the `Publish` target is copied to the directory `bin`, set using the expression  `exeOutPath` in [`build.fsx`](./build.fsx).

Warning:

All of these directories are cleaned - that means all files in them are deleted - using the target `clean`.
Which is called as the first build step of all targets. In other words: all generated
files (including Nuget packages) are deleted when calling any target.

The Fake script [`build.fsx`](./build.fsx) defines the following targets:

* `Clean` deletes all files generated by the build. **This is called first in every other target, all generated files are deleted before running any target!**
* `Distclean` deletes everything that isn't checked in to Git
* `Build` and `BuildDeb` build the configured projects
   using `dotnet build`. Without suffix a release build is made, `BuildDeb` uses the
   debug configuration. Default is to build all projects found in the directories
   `src` and `tests`. The expression `buildProjs` in [`build.fsx`](./build.fsx) defines
   the projects to build.
* `Docs` generates the HTML documentation using MkDocs. The HTML is created in the directory `sites`.
  The output directory is configured in the MkDocs config file [`mkdocs.yml`](./mkdocs.yml)
* `Lint` runs FSharpLint on all configured projects. Default is all projects, `lintProjs` in [`build.fsx`](./build.fsx)
  defines the projects to run `Lint` on.
* `Tests` and `TestsDeb` build and run all test projects in the directory `tests` using `dotnet test`.
  Without suffix a release build is made, `TestsDeb` uses the  debug configuration.
  Default is testing all projects in the directory `tests`, `testProjs` in [`build.fsx`](./build.fsx) defines the projects to run `Tests` and `TestsDeb` on.
* `TestsCoverage` and `TestsCoverageDeb`  build and run all tests projects and generate a coverage report using AltCover.
  With and without suffix a debug configuration is build and tested.
  Default is testing all projects in the directory `tests`, `testProjs` in [`build.fsx`](./build.fsx) defines the projects to run `TestsCoverage` and `TestsCoverageDeb` on.
* `Publish` runs `dotnet publish` on the configured projects, default is all in `src`. The
  build is done with the release configuration, the version **must** be given as argument on the command line
  and be the same as the newest one in [CHANGELOG.md](./CHANGELOG.md).
  `publishProjs` in [`build.fsx`](./build.fsx) defines the projects to run `Publish` on.
  Published binaries are saved to the directory `bin`.
* `Packages` builds the NuGet packages of all projects using a release build. The
  build is done with the release configuration, the version **must** be given as argument on the command line
  and be the same as the newest one in [CHANGELOG.md](./CHANGELOG.md).
  `packageProjs` in [`build.fsx`](./build.fsx) defines the projects to run `Packages` on.
  Packages are saved to the directory `packages`
* `Upload` uploads all packages in the directory `packages` to NuGet.org. The NuGet API key
  needs to be saved to the configuration using

  ```shell
  nuget setApiKey API_KEY
  ```

* `Release` is a pseudo-target, the same as `Build`, `Docs`, `Publish`, `Packages` and `Upload`. The
  build is done with the release configuration, the version **must** be given as argument on the command line
  and be the same as the newest one in [CHANGELOG.md](./CHANGELOG.md).
* `All` is a pseudo-target, the same as `Build` and `Docs`,

Usage:

```shell
dotnet fake build.fsx target TARGET VERSION_STRING
```

* `TARGET` is the name of the target to call, see above list for possible targets
* `VERSION_STRING` is the version to use for the target, **must** be the same as the first (newest)
  version in the file [`CHANGELOG.md`](./CHANGELOG.md). Targets that need `VERSION_STRING` are
  `Packages`, `Publish`, `Upload` and `Release`.

Run all default targets (see above):

```shell
dotnet fake build.fsx
```

Which is the same as

```shell
dotnet fake run build.fsx target All
```

E.g. to build a debug version of all projects:

```shell
dotnet fake run build.fsx target BuildDeb
```

Generate the Nuget packages of version `2.65.93` and upload to [NuGet.org](https://www.nuget.org/)

```shell
dotnet fake run build.fsx target Upload 2.65.93
```

Run `dotnet publish` on the configured projects (`publishProjs` in [build.fsx](./build.fsx))  setting the
version to 6.2.53

```shell
dotnet fake run build.fsx target Publish 6.2.53
```

Run the tests - result is in the directory `test_results`:

```shell
dotnet fake run build.fsx target Tests
```

Run the tests with coverage analysis - result is in the directory `test_results`:

```shell
dotnet fake run build.fsx target TestsCoverage
```


## MkDocs Files

* `mkdocs.yml` the MkDocs configuration, specially
    the configuration of the navigation sidebar in `nav`
    Which you need to edit

    ```yml
    nav:
    - Home: index.md
    - Project Links:
      - 'GitHub Project Page': 'https://github.com/Release-Candidate/FSHARP_TEMPLATE'
      - 'Nuget Package': 'https://pypi.org/project/FSHARP_TEMPLATE/'
      - 'Report a Bug or a Feature Request': 'https://github.com/Release-Candidate/FSHARP_TEMPLATE/issues/new/choose'
      - 'Issue Tracker at GitHub': 'https://github.com/Release-Candidate/FSHARP_TEMPLATE/issues'
    - 'Installation & Usage':
      - 'Installation & Usage': usage.md
      - License: license.md
    - Contributing:
      - Contributing: contributing.md
   ```

* `/docs` the markdown files that are used to generate the
   HTML sites in the directory `sites`

## Read the Docs Configuration

Generate an account at [Read the Docs](https://readthedocs.org/), and link your GitHub repositories.

* `.readthedocs.yaml` the configuration for Read the Docs
* `/docs/requirements.txt` the packages needed by MkDocs
   when generating the documentation at Read the Docs.
   Locally needed packages are configured in `Pipfile`

Read the Docs automatically generates the MkDocs documentation after each `git push`.

## CodeCov Configuration

Generate an account at [CodeCov](https://about.codecov.io/), link your GitHub repositories, add the CodeCov token
as a secret to the GitHub repositories named `CODECOV_SECRET` and upload the coverage result
using a GitHub action. Used in the workflows [linux_test.yml](./.github/workflows/linux_test.yml),
[osx_test.yml](./.github/workflows/osx_test.yml) and [windows_test.yml](./.github/workflows/windows_test.yml)

```yml
      - name: Run Tests
        run: |
          cd $GITHUB_WORKSPACE
          dotnet fake run ./build.fsx target TestsCoverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v1
        if: ${{ always() }}
        with:
          token: ${{ secrets.CODECOV_SECRET }}
          files: ./test_results/coverage.xml
          directory: ./coverage/reports/
          # flags: unittest
          env_vars: OS,PYTHON
          name: Linux-Test-Src
          fail_ci_if_error: false
          path_to_write_report: ./coverage/codecov_report.txt
          verbose: true
```

## GitHub Workflows

All tests and builds are executed on Linux,
Mac OS X and Windows.

These are the GitHub workflows defined in the directory `.github/workflows`

* `create_packages.yml` creates and uploads the NuGet packages, runs `dotnet publish` and
  generates a new GitHUb release with these files appended. Run automatically after tagging
  the source with a release tag of the form `v?.?.?` (**must be the same version as the newest in [CHANGELOG.md](./CHANGELOG.md)**).
  Appends the newest entry in [CHANGELOG.md](./CHANGELOG.md) to the release - script [`scripts/get_changelog.sh`](./scripts/get_changelog.sh)
  See the [latest release](https://github.com/Release-Candidate/FSharp_Template/releases/latest) as an example
* `fsharplint.yml` runs FSharpLint on all projects, after each `git push`
* `linux.yml` runs a default build (target `All`) on Linux, after each `git push`
* `linux_test.yml` runs the tests and coverage tests on Linux, uploads the test results as artifacts,
  uploads the coverage results to CodeCov.
* `osx.yml`  runs a default build (target `All`) on Mac OS X, after each `git push`
* `osx_test.yml` runs the tests and coverage tests on Mac OS X, uploads the test results as artifacts,
  uploads the coverage results to CodeCov.
* `windows.yml`  runs a default build (target `All`) on Windows, after each `git push`
* `windows_test.yml` runs the tests and coverage tests on Windows, uploads the test results as artifacts,
  uploads the coverage results to CodeCov.

The badges of the workflows are linked in the section [Badges](#badges)

## GitHub Issue Template

Issue templates for GitHub in `.github/ISSUE_TEMPLATE/`

* `bug_report.md` Bug report template
* `feature_request.md` Feature request template

# Begin of the Template

## Download

List of changes: [CHANGELOG.md](https://github.com/Release-Candidate/FSHARP_TEMPLATE/blob/main/CHANGELOG.md)

[Link to the latest release](https://github.com/Release-Candidate/FSHARP_TEMPLATE/releases/latest)

## Installation and Usage

### Prerequisites

### Installation

### Usage

## Contributing

## License

All content of FSHARP_TEMPLATE is licensed under the MIT license, see file [LICENSE](https://github.com/Release-Candidate/FSHARP_TEMPLATE/blob/main/LICENSE).

## Badges

### GitHub Workflows/Actions

#### Static Code Checkers

[![FsharpLint Linter](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/fsharplint.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/fsharplint.yml)

#### Tests on various OSes

[![Tests Mac OS X latest](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/osx_test.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/osx_test.yml)
[![Tests Ubuntu 20.04](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/linux_test.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/linux_test.yml)
[![Tests Windows 2019](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/windows_test.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/windows_test.yml)
[![Mac OS X latest](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/osx.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/osx.yml)
[![Ubuntu 20.04](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/linux.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/linux.yml)
[![Windows 2019](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/windows.yml/badge.svg)](https://github.com/Release-Candidate/FSharp_Template/actions/workflows/windows.yml)

### External Websites

[![codecov](https://codecov.io/gh/Release-Candidate/FSharp_Template/branch/main/graph/badge.svg)](https://codecov.io/gh/Release-Candidate/FSharp_Template)
