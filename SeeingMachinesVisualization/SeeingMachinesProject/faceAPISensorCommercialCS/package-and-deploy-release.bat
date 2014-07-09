@echo off
set fname=faceAPICommercial.2caa5763-3e25-49a9-ac57-7cae5d02ae09.1000.zip
cd bin/Release
7za.exe a -tzip %fname% .
move /Y %fname% ../..
cd ../..
ftp -s:ftp.txt
pause