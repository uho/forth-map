synonym map wordlist
\ Maps aka key/value structures are implemente with wordlists. 
\ So internally we call them word lists `wid` but externally we call them maps `map`.

64 value map.key-space
128 value map.capacity

5 cells constant ORDERED_MAP_HEADER

: ORDERED_MAP_NEXT       ( map -- addr ) ;
: ORDERED_MAP_COUNT      ( map -- addr ) 1 cells + ;
: ORDERED_MAP_CAPACITY   ( map -- addr ) 2 cells + ;
: ORDERED_MAP_ITEM_SIZE  ( map -- addr ) 3 cells + ;
: ORDERED_MAP_KEY_SPACE  ( map -- addr ) 4 cells + ;
: ORDERED_MAP_ITEMS      ( map -- addr ) ORDERED_MAP_HEADER + ;

256 VALUE map.space
\ map.space includes the count byte, so the maximum string value is 255 bytes.

variable ordered-maps
0 ordered-maps !

: ordered-map? ( map -- flag )
    ordered-maps @
    begin
        dup
    while
        2dup = if 2drop true exit then
        ORDERED_MAP_NEXT @
    repeat
    2drop false
;

: ordered-map { | item-size map -- map }
\ Allocate a bounded insertion-ordered map outside dictionary space.
    map.key-space 1+ map.space + -> item-size
    item-size map.capacity * ORDERED_MAP_HEADER +
        allocate throw -> map
    ordered-maps @ map ORDERED_MAP_NEXT !
    0 map ORDERED_MAP_COUNT !
    map.capacity map ORDERED_MAP_CAPACITY !
    item-size map ORDERED_MAP_ITEM_SIZE !
    map.key-space map ORDERED_MAP_KEY_SPACE !
    map ORDERED_MAP_ITEMS item-size map.capacity * erase
    map ordered-maps !
    map
;

: ordered-map-entry { index map -- entry }
    map ORDERED_MAP_ITEMS
    index map ORDERED_MAP_ITEM_SIZE @ * +
;

: ordered-map-value { entry map -- value-addr }
    entry map ORDERED_MAP_KEY_SPACE @ 1+ +
;

: ordered-map-item? { c-addr u map | entry -- addr true | 0 false }
    u map ORDERED_MAP_KEY_SPACE @ > if 0 false exit then
    map ORDERED_MAP_COUNT @ 0 ?do
        i map ordered-map-entry -> entry
        c-addr u entry count icompare 0= if
            entry map ordered-map-value true unloop exit
        then
    loop
    0 false
;

: ordered-map-new-item { c-addr u map | entry value-addr -- addr }
    u map ORDERED_MAP_KEY_SPACE @ >
        abort" ordered map key is too long"
    map ORDERED_MAP_COUNT @ map ORDERED_MAP_CAPACITY @ >=
        abort" ordered map is full"
    map ORDERED_MAP_COUNT @ map ordered-map-entry -> entry
    c-addr u entry place
    entry map ordered-map-value -> value-addr
    0 value-addr c!
    1 map ORDERED_MAP_COUNT +!
    value-addr
;

: reset-map ( map -- )
    dup ordered-map? 0= abort" only ordered maps can be reset"
    dup >r
    r@ ORDERED_MAP_ITEMS
    r@ ORDERED_MAP_ITEM_SIZE @
    r@ ORDERED_MAP_CAPACITY @ * erase
    0 r> ORDERED_MAP_COUNT !
;

: unlink-ordered-map { map | link current -- }
    ordered-maps -> link
    begin
        link @ dup
    while
        -> current
        current map = if
            current ORDERED_MAP_NEXT @ link !
            exit
        then
        current ORDERED_MAP_NEXT -> link
    repeat
    drop
;

: free-map ( map -- )
    dup ordered-map? if
        dup unlink-ordered-map
        free throw
    else
        drop
    then
;

: +map ( map --)
\ make the keys of a map visible in the search order
	+ORDER
;

: -map ( map --)
\ make the keys of a map visible in the search order
	-ORDER
;

: wordlist-item? ( c-addr u wid -- addr true | 0 false )
\ Check if there is an item in the given word list `wid` with key `c-addr` `u`.
\ If the item exists return its body address and `true` or return 0 and `false`.	
   search-wordlist IF 
   	>body true 
   ELSE
   	0 false
   THEN
;

: "string ( c-addr u -- )
\ Create a string value-like item with name `c-addr` `u` in the current word list.	
   ($create)
   	map.space ALLOT
   DOES>		( -- c-addr u)
   	count
 ;

: wordlist-new-item ( c-addr u wordlist -- addr )
\ Create a new item with name `c-addr` `u` in the word list `wid`. Return its PFA (body address)	
   get-current >r set-current
   ['] "string catch 
   r> set-current throw 
   here map.space - ; 


: item? ( c-addr u map -- addr true | 0 false )
    dup ordered-map? if ordered-map-item? else wordlist-item? then
;

: new-item ( c-addr u map -- addr )
    dup ordered-map? if ordered-map-new-item else wordlist-new-item then
;

: >addr ( c-addr u map -- addr )
\ Return the body address of an item with key `c-addr` `u` in the key/value map `map`	
    >r 2dup r@ item? IF 
    	nip nip r> drop
    ELSE
    	drop r> new-item 
    THEN
  ;

: invoke-xt ( i*x xt nt -- j*x xt flag )
\ this word defines the xt for traverse-wordlist in interate-map
\ xt is a user-defined xt that will be applied to each item in the map
\ invoke-xt obtains the name of each key and then calls the user-defined xt
\ the user-defined xt has stack effect (i*x c-addr u -- j*x)
   swap >r  name>string r@ execute  r> swap ;

: wordlist-iterate-map ( i*x xt map -- j*x )
\ interate the map with a user-defined xt
\ xt is passed the name of the key and has stack effect (i*x c-addr u -- j*x)
   ['] invoke-xt swap traverse-wordlist drop ;

: ordered-map-iterate { xt map -- }
\ Match traverse-wordlist semantics: visit newest-to-oldest and consume the
\ flag returned by xt. simple-iterate-map reverses this to insertion order.
    map ORDERED_MAP_COUNT @ 0 ?do
        map ORDERED_MAP_COUNT @ 1- i - map ordered-map-entry count
        xt execute 0= if unloop exit then
    loop
;

: iterate-map ( i*x xt map -- j*x )
    dup ordered-map? if ordered-map-iterate else wordlist-iterate-map then
;
