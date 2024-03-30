\ test for map-tools.fs
\ illustrate the use of structures with a 
include "%idir%\map.fs"
include "%idir%\map-tools.fs"
include "%idir%\..\simple-tester\simple-tester.f"

BEGIN-STRUCTURE DATA_STRUCTURE
	 1 +FIELD DATA_ATTRIBUTE
	63	+FIELD DATA_TYPE
	64 +FIELD DATA_VALUE
END-STRUCTURE

: DATA_ATTRIBUTE! ( x addr --)
	DATA_ATTRIBUTE c!
;

: DATA_TYPE! ( c-addr n addr --)
	DATA_TYPE place
;

: DATA_VALUE! ( c-addr n addr --)
	DATA_VALUE place
;

: DATA_STRUCTURE! ( x c-addr n c-addr n addr --)
	>R
	R@ DATA_VALUE!
	R@ DATA_TYPE!
	R> DATA_ATTRIBUTE!
;

	DATA_STRUCTURE -> map.space
	
	map CONSTANT DB1
	
	-1 s" Float64" s" 195.4997911" s" OBSERVATION:CENTER:RA" DB1 >value DATA_STRUCTURE!
	-1 s" Float64" s" 47.2661464" s" OBSERVATION:CENTER:DEC" DB1 >value DATA_STRUCTURE!
	-1 s" Timepoint" s" '2024-3-31:00:00:05'" s" Observation.time" DB1 >value DATA_STRUCTURE!
	 0 s" String" s" Observatorio de Aras de los Olmos (OAO)" s" OBSERVATION:LOCATION:NAME" DB1 >value DATA_STRUCTURE!
	
: iterator
	>R 2dup CR type CR R>
	>value DATA_STRUCTURE dump
;

' iterator DB1 simple-iterate-map
