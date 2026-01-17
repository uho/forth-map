\ test for map-tools.fs

include "%idir%\map.fs"
include "%idir%\map-tools.fs"

NEED simple-tester

CR
Tstart

map CONSTANT CITY

T{ s" Berlin" s" Germany" CITY => }T ==
T{ s" Paris" CITY =>" France"     }T ==

: inside_test s" London" CITY =>" Great Britain" ;

T{ inside_test }T ==
T{ s" Great Britain" CITY >string hashS }T s" London" hashS ==
T{ s" France" CITY >string hashS }T s" Paris" hashS ==
T{ s" Germany" CITY >string hashS }T s" Berlin" hashS ==

Tend