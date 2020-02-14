# DNN.Announcements
DNN Announcements is a basic module used for displaying news items on your DNN site.

## Compatibility
| Dnn Announcements version | Min Tested Dnn version | Max Tested Dnn version |
| -------------------------:| ----------------------:| ----------------------:|
|  Included or use 04.00.03 |               00.00.00 |               08.00.04 |
|                  07.02.00 |               08.00.00 |               09.02.00 |

## Building the module from source and submitting pull requests
1. Install Dnn (latest stable version) from https://github.com/dnnsoftware/Dnn.Platform/releases
2. Fork this repository to your own github account
3. Clone your fork to the Dnn DesktopModules folder
4. Important, the project name id Dnn.Announcements, but the deployment directory is just Announcements (more later)
5. Build the project in release mode using Visual Studio, this will create the installable packages in the Install/version/ folder of Dnn.Announcements folder
6. Install one of the zip packages using the Dnn extension installer as any other module
7. In Visual Studio, create a new branch to isolate your changes.
8. In Visual studio, to test any changes, you need to build in debug mode, this will compile and copy all files from Dnn.Announcements (the source code) to Announcements (the deployment folder). To debug, use the attach to process feature and attach it to the w3wp process that matches the running site.
9. Commit and push your changes with clear descritions, then in github, create a pull request from the branch you created to the Dnn.Community repository, again please add a good description of the changes. You can also mention issues with #issueNumber to automatically associte your pull request with existing issues.

## Maintainers
This module is currently maintained by @erikvb and Joseph Craig

Please coordinate with them before publishing any new release and mark them as reviewers on pull requests

## Contributing
If you would like to contribute to this project, please read [CONTRIBUTING.md](https://github.com/DNNCommunity/DNN.Announcements/blob/development/.github/CONTRIBUTING.md)
