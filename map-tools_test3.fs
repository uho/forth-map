\ test for map-tools.fs
include "%idir%\..\ForthBase\ForthBase.f"
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
include "%idir%\..\simple-tester\simple-tester.f"

CR Tstart CR

 	map-strings
	map CONSTANT colourTable
\ 	value			 map		    =>" key"

	s" yellow" colourTable >addr s" 0xffff00" rot place
T{ s" yellow" colourTable >addr count hashS   }T s" 0xffff00" hashS ==
T{ s" yellow" colourTable >string hashS   }T s" 0xffff00" hashS ==

	s" 0xff0000" colourTable =>" red" 
T{ s" red" colourTable >string hashS   }T s" 0xff0000" hashS ==

: set-green
	s" 0x00ff00" colourTable =>" green" 
;
	set-green
T{ s" green" colourTable >string hashS }T s" 0x00ff00" hashS ==	

	s" 0x0000ff" colourTable =>" blue"
T{ s" blue" colourTable >number }T 255 ==

CR colourtable .map

CR Tend

