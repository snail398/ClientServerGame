@echo off

rem === Remove dll, just in case ===

pushd "%~dp0"
echo %cd%

mkdir ..\Clone1

mklink /D ..\Clone1\Assets %CD%\Assets
mklink /D ..\Clone1\Packages %CD%\Packages
mklink /D ..\Clone1\ProjectSettings %CD%\ProjectSettings
mklink /D ..\Clone1\Library %CD%\Library

mkdir ..\Clone2

mklink /D ..\Clone2\Assets %CD%\Assets
mklink /D ..\Clone2\Packages %CD%\Packages
mklink /D ..\Clone2\ProjectSettings %CD%\ProjectSettings
mklink /D ..\Clone2\Library %CD%\Library
