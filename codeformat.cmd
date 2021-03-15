@echo off

:: Microsoft CoreFx & Rosylin Compiler Coding Styles
:: https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md

set Root=%cd%

set CodeFormatterPath=%Root%\tools\CodeFormatter
set CodeFormatterExe="%CodeFormatterPath%\CodeFormatter.exe"
set CodeFormatterParams=/copyright:"%CodeFormatterPath%\CopyrightHeader.md"

%CodeFormatterExe% "Partnerinfo.sln" %CodeFormatterParams%