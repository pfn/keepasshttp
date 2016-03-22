@echo off
set base=%~dp0
set findnet=powershell -command "$([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory())"

for /f "tokens=* USEBACKQ" %%I in (`%findnet%`) DO (
  set netframework=%%I
)

pushd %base%
cd /d %base%
del KeePassHttp.plgx
del mono\KeePassHttp.dll
rd /s /q KeePassHttp\bin
rd /s /q KeePassHttp\obj
"%ProgramFiles(x86)%\KeePass Password Safe 2\KeePass.exe" --plgx-create %base%KeePassHttp
%netframework%MSBuild.exe /target:clean KeePassHttp.sln
%netframework%MSBuild.exe /p:Configuration=Release KeePassHttp.sln
copy /y KeePassHttp\bin\Release\KeePassHttp.dll mono
popd
