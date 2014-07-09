
Command format:
	referencefixutility.exe s:c:/path/to/lib s:c:/path/to/other/lib t:c:/path/to/target/projects/folder

Changes absolute library references to be relative references, assuming that a the requested library
	can be found in the reference folders. References will only be updated if the .NET runtime version
	matches the version requested in the project reference, and library versions can optionally be
	specifically matched.

If the program is set to specifically match libraries, the matching will fail if a library isn't found
	with the same exact version as the project requested.

If library versions aren't specifically matched (matched by lib version)
	but a matching library is found, the matched library will be used. If there is no library with a
	matching version, then the library with the latest version is used. Libraries with the same assembly
	name are considered to be different versions of the same library.