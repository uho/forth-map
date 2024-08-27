\ test for map-tools.fs
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
include "%idir%\..\simple-tester\simple-tester.f"

CR Tstart
CR
	map-strings
	
	map CONSTANT colourTable
	s" red" colourTable s" 0xff0000" |<=|
	s" green" colourTable <=" 0x00ff00"
	s" blue" colourTable <=" 0x0000ff"

T{ s" red" colourTable >string hashS   }T s" 0xff0000" hashS ==
T{ s" green" colourTable >string hashS }T s" 0x00ff00" hashS ==
T{ s" blue" colourTable >number }T 255 ==

CR Tend