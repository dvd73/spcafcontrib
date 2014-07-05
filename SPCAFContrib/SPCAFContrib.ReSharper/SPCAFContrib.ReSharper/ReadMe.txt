To add functionality to your plugin, use the Add|New Item... menu.

To debug your plugin, simply make sure it is selected as the start-up
project and press F5.

To link new rule with help page see project resources: you need to ensure that [CheckId - help page url] pair exists there.

To update package:
1. Modify assembly version.
2. Edit SPCAFContrib.ReSharper.nuspec file. Change version in the <version> tag.
3. Edit UpdatePackage.cmd. Set right package file name (SPCAFContrib.ReSharper.1.0.0.1.nupkg). 
4. Run UpdatePackage.cmd.

Note: ApiKey is a key for dvd73 resharper account. You can change it if you want.