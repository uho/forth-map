\ map-tools.fs
\ Andrew Read spring 2024

\ some extensions for UHO's map.fs
\ note that simple-iterate-map requires local variables - written here for VFX Forth

alias: >addr >value
\ to avoid confusion, as Forth value-flavoured types do not need @

: >string
\ typical usage
	>addr count
;

internal

: iterator-for-counting ( n c-addr u -- n+1 -1)
\ an interator to count the number of items in a map
	2drop 1+ -1
;

external

: count-keys ( map -- n)
\ count the number of keys in the map
	0 ['] iterator-for-counting rot iterate-map
;

internal

: iterator-for-buffering ( buffer c-addr u -- buffer -1)
\ the buffer grows downwards so the argument should be 1 cell ahead of the present end-of-buffer
	2>R					( buffer R: c-addr u)
	cell - dup R> swap !
	cell - dup R> swap !
	-1
;

external

: buffer-keys ( hook map -- buffer)
\ copy the keys into a buffer and return the start address
\ the buffer grows downwards from its hook
\ [c1 addr1 c2 addr2 ... cn addrn]
\ ^buffer								^hook
	['] iterator-for-buffering swap iterate-map
;

: simple-iterate-map { xt map | count buffer hook -- }
\ iterate the full map in the forward direction
\ xt has stack effect ( x*i c-addr u map -- x*j), noting that
\		1. the stack parameters are the same as for >value and x*i, x*j are freely accessible
\		2. parameter 'map' should be consumed (it is replaced each time by the iteration loop)
\		3. there is no T|F flag - all keys are processed

\ Stage 0 - allocate a buffer to hold the keys
	map count-keys	-> count
	count 0= IF EXIT THEN					
	2 cells count * dup allocate THROW -> buffer ( size-in-bytes)
	buffer + -> hook
	
\ Stage 1 - fill the buffer with keys
	hook map buffer-keys ( buffer) drop

\ Stage 2 - execute xt against each key in the buffer			
	hook buffer DO
		i @ i cell + @	( caddr n)
		map xt execute
	2 cells +LOOP
	
\ Stage 3 - tidy up
	buffer free throw
;
		
