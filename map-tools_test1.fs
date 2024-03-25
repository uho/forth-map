\ test for map-tools.fs
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
include "%idir%\..\simple-tester\simple-tester.f"

map CONSTANT colourTable

hex

s" red" colourTable >value ff0000 swap !
s" green" colourTable >value 00ff00 swap !
s" blue" colourTable >value 0000ff swap !

: iterator-for-values-in-reverse ( v1 ... map c-addr u -- v1 ... vn map -1 )
\ an iterator for iterate-map
	>R >R dup R> R> rot >value @ swap -1
;

: iterator-for-values-in-order ( v1 ... c-addr u map -- v1 ... vn )
\ an interator for simple-iterate-map
	>value @
;

20 dup BUFFER: buffer_1
buffer_1 + CONSTANT hook_1	

CR Tstart CR

T{ s" red" colourTable >value @ }T ff0000 ==
T{ s" blue" colourTable >value @ }T 0000ff ==
	s" blue" colourTable >value 000099 swap !
T{ s" blue" colourTable >value @ }T 000099 ==
	s" blue" colourTable >value 0000ff swap !
	
T{ colourTable count-keys }T 3 ==
	
T{ colourTable ' iterator-for-values-in-reverse over iterate-map drop }T 0000ff 00ff00 ff0000 ==

T{ hook_1 colourTable buffer-keys }T hook_1 6 cells - ==

T{ ' iterator-for-values-in-order colourTable simple-iterate-map }T ff0000 00ff00 0000ff ==

Tend CR