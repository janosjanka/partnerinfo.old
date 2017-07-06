del /s /f *.cache
del /s /f *.keep
del /s /ah StyleCop.Cache

rd /s /q bin obj ClientBin Deploy TestResults

del dirs.txt
dir /s /b /ad bin > dirs.txt
dir /s /b /ad obj >> dirs.txt
dir /s /b /ad ClientBin >> dirs.txt
dir /s /b /ad Deploy >> dirs.txt
dir /s /b /ad TestResults >> dirs.txt

for /f "delims=;" %%i in (dirs.txt) DO rd /s /q "%%i"
del dirs.txt