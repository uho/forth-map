\ test for map.fs
NEED ForthBase
NEED simple-tester
include "%idir%\map.fs"

CR
Tstart

CR ." create a map and test some numeric key-value pairs" CR
map CONSTANT colourTable1 hex

s" red" colourTable1 >addr ff0000 swap !
s" green" colourTable1 >addr 00ff00 swap !
s" blue" colourTable1 >addr 0000ff swap !
T{ s" red" colourTable1 >addr @ }T ff0000 ==
T{ s" blue" colourTable1 >addr @ }T 0000ff ==
	s" blue" colourTable1 >addr 000099 swap !
T{ s" blue" colourTable1 >addr @ }T 000099 ==
	s" blue" colourTable1 >addr 0000ff swap !
decimal

CR ." create a map and test some string key-value pairs" CR
map CONSTANT colourTable2

s" red" colourTable2 >addr 		s" ff0000" rot $!
s" green" colourTable2 >addr 	s" 00ff00" rot $!
s" blue" colourTable2 >addr 	s" 0000ff" rot $!
T{ s" red" colourTable2 >addr $@ hashS }T s" ff0000" hashS  ==
T{ s" blue" colourTable2 >addr $@ hashS }T s" 0000ff" hashS ==
	s" blue" colourTable2 >addr s" 000099" rot $!
T{ s" blue" colourTable2 >addr $@ hashS }T s" 000099" hashS ==
	s" blue" colourTable2 >addr s" 0000ff" rot $!

CR ." test with the map keys as Forth words in the input stream" CR
colourTable2 +map
T{ red hashS }T s" ff0000" hashS ==
T{ blue hashS }T s" 0000ff" hashS ==
colourTable2 -map

\ Quotations for a single use 

CR
Tend