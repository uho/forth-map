\ Dual-representation string map: dictionary wordlists for enduring data and
\ bounded heap-backed ordered maps for reusable instances.

synonym map wordlist
\ A map may use either a dictionary wordlist or bounded heap storage. Wordlist
\ maps suit enduring configuration; ordered maps suit multiple reusable
\ instances. The public access and iteration words dispatch on the handle.

\ These values are sampled when an ordered map is allocated. Changing them
\ affects subsequent allocations, not maps which already exist.
64 value map.key-space
128 value map.capacity

\ Ordered-map header, in cells:
\   link | used entries | capacity | bytes per entry | key bytes
\ Each following entry is:
\   counted key (key bytes + count byte) | counted value (map.space bytes)
5 cells constant ORDERED_MAP_HEADER

: ORDERED_MAP_NEXT       ( map -- addr ) ;
: ORDERED_MAP_COUNT      ( map -- addr ) 1 cells + ;
: ORDERED_MAP_CAPACITY   ( map -- addr ) 2 cells + ;
: ORDERED_MAP_ITEM_SIZE  ( map -- addr ) 3 cells + ;
: ORDERED_MAP_KEY_SPACE  ( map -- addr ) 4 cells + ;
: ORDERED_MAP_ITEMS      ( map -- addr ) ORDERED_MAP_HEADER + ;

256 VALUE map.space
\ map.space includes the count byte, so its default permits 255 value bytes.

\ VFX wordlist identifiers cannot safely be dereferenced to read a type tag.
\ Keep allocated handles in this private linked list so the shared API can
\ distinguish ordered maps from wordlist maps.
variable ordered-maps
0 ordered-maps !

: ordered-map? ( map -- flag )
\ Handle recognition is intentionally linear in the number of live ordered
\ maps; normal sessions own only a small number of frame-local instances.
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
\ Allocate one header and its fixed entry array outside dictionary space.
\ No later insertion allocates memory or advances the Forth dictionary.
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
\ Convert a zero-based insertion index to its fixed-size entry address.
    map ORDERED_MAP_ITEMS
    index map ORDERED_MAP_ITEM_SIZE @ * +
;

: ordered-map-value { entry map -- value-addr }
\ Skip the entry's counted-key field to reach its counted-value field.
    entry map ORDERED_MAP_KEY_SPACE @ 1+ +
;

: ordered-map-item? { c-addr u map | entry -- addr true | 0 false }
\ Search existing entries without creating one. Keys compare case-insensitively
\ to match wordlist-map lookup. The returned address is the counted value.
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
\ Append a new key at the next insertion slot. >addr performs lookup first, so
\ replacing an existing value does not append or change its order.
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
\ Reuse an ordered map without allocation or dictionary growth. Reset erases
\ all keys and values, so addresses returned before reset become invalid data.
    dup ordered-map? 0= abort" only ordered maps can be reset"
    dup >r
    r@ ORDERED_MAP_ITEMS
    r@ ORDERED_MAP_ITEM_SIZE @
    r@ ORDERED_MAP_CAPACITY @ * erase
    0 r> ORDERED_MAP_COUNT !
;

: unlink-ordered-map { map | link current -- }
\ Remove a handle from the recognition list before releasing its allocation.
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
\ Ordered maps own heap storage and must be freed once. Wordlist maps are
\ dictionary objects and deliberately make this operation a no-op.
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
\ Non-creating lookup shared by both representations.
    dup ordered-map? if ordered-map-item? else wordlist-item? then
;

: new-item ( c-addr u map -- addr )
\ Representation-specific insertion; callers normally use >addr or =>.
    dup ordered-map? if ordered-map-new-item else wordlist-new-item then
;

: >addr ( c-addr u map -- addr )
\ Return the counted-value address, creating the key only when absent.
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
\ traverse-wordlist visits dictionary keys newest-first. Visit ordered entries
\ in the same direction so buffer-keys and simple-iterate-map continue to
\ reverse that traversal and expose stable insertion order to callers.
    map ORDERED_MAP_COUNT @ 0 ?do
        map ORDERED_MAP_COUNT @ 1- i - map ordered-map-entry count
        xt execute 0= if unloop exit then
    loop
;

: iterate-map ( i*x xt map -- j*x )
    dup ordered-map? if ordered-map-iterate else wordlist-iterate-map then
;
