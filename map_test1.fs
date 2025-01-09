\ test for map.fs
NEED ForthBase
NEED simple-tester
include "%idir%\map.fs"

CR
Tstart

	hex
	map CONSTANT colourTable
	s" red" colourTable >addr ff0000 swap !
	s" green" colourTable >addr 00ff00 swap !
	s" blue" colourTable >addr 0000ff swap !

T{ s" red" colourTable >addr @ }T ff0000 ==
T{ s" blue" colourTable >addr @ }T 0000ff ==
	s" blue" colourTable >addr 000099 swap !
T{ s" blue" colourTable >addr @ }T 000099 ==
	s" blue" colourTable >addr 0000ff swap !
	
CR
Tend