ECHO Construct Server Installer.

del "C:\ConstructStudio\*.*" /f/s/q

MD "C:\ConstructStudio"



xcopy *.* /e  "C:\ConstructStudio"

GOTO End1


:End1