@echo off
set fname=faceLABConstructSensor.e683e0c5-85fb-4788-81a1-f39cb2054687.1000.zip
cd bin/Release
7za.exe a -tzip %fname% .
move /Y %fname% ../..
cd ../..
ftp -s:ftp.txt
pause