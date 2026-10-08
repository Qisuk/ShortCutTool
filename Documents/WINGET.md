# Publishing to WinGet

ShortCutTool is published to the [Windows Package Manager community repository](https://github.com/microsoft/winget-pkgs) as **`Qisuk.ShortCutTool`**, using the Inno Setup installer from each GitHub release.

| | |
|---|---|
| Installer | `ShortCutTool-<version>-win-x64-setup.exe` (`InstallerType: inno`, `Scope: user`) |
| Dependency | `Microsoft.DotNet.DesktopRuntime.10` (WinGet installs it automatically) |
| Product code | `{681BDEBB-6E4C-451C-A509-91C4D9F71A02}_is1` (the installer's `AppGuid`; never change it) |
| Publisher | `Qisuk` (must match `AppPublisher` in `installer\ShortCutTool.iss`) |

## First submission (one time)

The automated workflow can only update a package that already exists in winget-pkgs, so the first version is submitted by hand.

1. **Release.** Merge to `master`, then tag `vX.Y.Z` matching `<Version>` in the csproj. The Release workflow publishes the setup exe.
2. **Test the release installer**, ideally in Windows Sandbox: fresh install, upgrade while the app is running, uninstall.
3. **Generate and validate the manifests** from the published installer:
   ```powershell
   .\winget\New-WingetManifest.ps1 -Version 1.1.0
   ```
   This writes `artifacts\winget\manifests\q\Qisuk\ShortCutTool\1.1.0\` and runs `winget validate`.
4. **Test the install through WinGet.** Either in Windows Sandbox with the winget-pkgs [`SandboxTest.ps1`](https://github.com/microsoft/winget-pkgs/blob/master/Tools/SandboxTest.ps1) script:
   ```powershell
   .\SandboxTest.ps1 .\artifacts\winget\manifests\q\Qisuk\ShortCutTool\1.1.0
   ```
   or locally (enabling local manifests needs an elevated prompt once):
   ```powershell
   winget settings --enable LocalManifestFiles
   winget install --manifest .\artifacts\winget\manifests\q\Qisuk\ShortCutTool\1.1.0
   ```
5. **Submit.** `wingetcreate` forks winget-pkgs to your account and opens the pull request:
   ```powershell
   wingetcreate token --store            # paste a classic PAT with the public_repo scope
   wingetcreate submit .\artifacts\winget\manifests\q\Qisuk\ShortCutTool\1.1.0
   ```
6. **Follow the PR** on microsoft/winget-pkgs. The validation pipeline installs the package in a clean VM and labels the PR; a moderator then reviews it. Answer any comments on the PR. New packages typically take a few days.

Once it is merged, `winget install Qisuk.ShortCutTool` works for everyone (allow a few hours for the index to update).

## Later versions (automatic)

After the first version is accepted, turn on the `winget` job in `.github/workflows/release.yml`:

1. Create a classic personal access token with the **`public_repo`** scope on the account that owns your winget-pkgs fork.
2. In the ShortCutTool repository settings:
   - **Secrets → Actions:** add `WINGET_TOKEN` with that token.
   - **Variables → Actions:** add `WINGET_AUTO_SUBMIT` = `true`.

From then on, every stable tag (no `-suffix`) runs `wingetcreate update Qisuk.ShortCutTool --urls <setup exe> --submit` after the GitHub release is published, which opens the winget-pkgs PR with the new URL, hash and version. Pre-releases are never submitted.

## Things that break the WinGet package

- Changing the installer's `AppGuid`, `AppPublisher` or the `-win-x64-setup.exe` asset name.
- Replacing a release asset after the WinGet PR was opened: the SHA256 in the manifest would no longer match. Publish a new version instead.
- Making the installer require admin rights without updating `Scope` in the manifest.
