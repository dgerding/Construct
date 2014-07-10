ECHO Construct Server Installer.

del "C:\ConstructServer\*.*" /f/s/q

MD "C:\ConstructServer"



xcopy *.* /e  "C:\ConstructServer"

GOTO End1


:End1