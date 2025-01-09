\ test for map-tools.fs
include "%idir%\..\ForthBase\libraries\libraries.f"
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
NEED simple-tester

CR
Tstart

map CONSTANT colourTable
s" red" colourTable >addr 0xff0000 swap !
s" green" colourTable >addr 0x00ff00 swap !
s" blue" colourTable >addr 0x0000ff swap !

CR ." natural order (backwards) iteration" CR
	: iterator-for-values-in-reverse ( v1 ... map c-addr u -- v1 ... vn map -1 )
		>R >R dup R> R> rot >addr @ swap -1
	;	
T{ colourTable ' iterator-for-values-in-reverse over iterate-map drop }T 0x0000ff 0x00ff00 0xff0000 ==

CR ." forward iteration" CR
	: iterator-for-values-in-order ( v1 ... c-addr u map -- v1 ... vn )
		>addr @
	;
T{ ' iterator-for-values-in-order colourTable simple-iterate-map }T 0xff0000 0x00ff00 0x0000ff ==

CR ." counting and buffering)" CR
T{ colourTable count-keys }T 3 ==
	20 dup BUFFER: buffer_1
	buffer_1 + CONSTANT hook_1	
T{ hook_1 colourTable buffer-keys }T hook_1 6 cells - ==

CR
Tend