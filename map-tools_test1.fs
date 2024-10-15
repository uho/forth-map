\ test for map-tools.fs
include "%idir%\..\ForthBase\libraries\libraries.f"
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
NEED simple-tester

CR Tstart

 	map-strings
CR
.( basic map operations) CR
	hex
	map CONSTANT colourTable
	s" red" colourTable >value ff0000 swap !
	s" green" colourTable >value 00ff00 swap !
	s" blue" colourTable >value 0000ff swap !

T{ s" red" colourTable >value @ }T ff0000 ==
T{ s" blue" colourTable >value @ }T 0000ff ==
	s" blue" colourTable >value 000099 swap !
T{ s" blue" colourTable >value @ }T 000099 ==
	s" blue" colourTable >value 0000ff swap !
	
CR	
.( map iteration) CR
	: iterator-for-values-in-reverse ( v1 ... map c-addr u -- v1 ... vn map -1 )
		>R >R dup R> R> rot >value @ swap -1
	;	
T{ colourTable ' iterator-for-values-in-reverse over iterate-map drop }T 0000ff 00ff00 ff0000 ==

CR
.( extension words) CR
T{ colourTable count-keys }T 3 ==
	20 dup BUFFER: buffer_1
	buffer_1 + CONSTANT hook_1	
T{ hook_1 colourTable buffer-keys }T hook_1 6 cells - ==

CR
.( simple forward iteration) CR
	: iterator-for-values-in-order ( v1 ... c-addr u map -- v1 ... vn )
		>value @
	;
T{ ' iterator-for-values-in-order colourTable simple-iterate-map }T ff0000 00ff00 0000ff ==

CR
.( map for strings) CR
	80 -> map.space

	map CONSTANT nameTable
	s" Neil" nameTable >value s" Armstrong" rot place
	s" Buzz" nameTable >value s" Aldrin" rot place
	s" Michael" nameTable >value s" Collins" rot place

T{ s" Neil" nameTable >value count hashS }T s" Armstrong" hashS ==
T{ s" Michael" nameTable >value count hashS }T s" Collins" hashS ==
	s" Michael" nameTable >value s" Jackson" rot place
T{ s" Michael" nameTable >value count hashS }T s" Jackson" hashS ==	

	1 cells -> map.space
	
Tend CR