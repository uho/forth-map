NEED forth-map
NEED simple-tester

ordered-map constant transient
ordered-map constant value-test
ordered-map constant invalid-map
map constant dictionary-test
wordlist constant plain-wordlist-test
ordered-map constant stale-map
stale-map free-map
variable invalid-map-capacity
variable invalid-map-item-size
variable invalid-map-key-space
create test-value-bytes 257 allot

: test-fill-values ( -- )
    257 0 do i test-value-bytes i + c! loop
;
: ordered-test-overlong ( -- )
    test-value-bytes 256 s" TOO-LONG" value-test =>
;
: dictionary-test-overlong ( -- )
    test-value-bytes 256 s" TOO-LONG" dictionary-test =>
;
: ordered-test-negative ( -- )
    test-value-bytes -1 s" NEGATIVE" value-test =>
;
: dictionary-test-negative ( -- )
    test-value-bytes -1 s" NEGATIVE" dictionary-test =>
;
: ordered-test-negative-key ( -- )
    s" value" test-value-bytes -1 value-test =>
;
: dictionary-test-negative-key ( -- )
    s" value" test-value-bytes -1 dictionary-test =>
;
: stale-lookup ( -- ) s" key" stale-map item? 2drop ;
: stale-insert ( -- ) s" key" stale-map new-item drop ;
: stale-iterate ( -- ) ['] 2drop stale-map iterate-map ;
: stale-simple-iterate ( -- ) ['] drop stale-map simple-iterate-map ;
: stale-free ( -- ) stale-map free-map ;
: bogus-lookup ( -- ) s" key" 1234 item? 2drop ;
: null-insert ( -- ) s" value" s" key" 0 => ;
test-fill-values

Tstart

T{ test-value-bytes 255 s" MAX" value-test => }T ==
T{ s" MAX" value-test >string nip }T 255 ==
T{ ' ordered-test-overlong catch 0<> }T -1 ==
T{ test-value-bytes 255 s" MAX" dictionary-test => }T ==
T{ s" MAX" dictionary-test >string nip }T 255 ==
T{ ' dictionary-test-overlong catch 0<> }T -1 ==
T{ ' ordered-test-negative catch }T -2 ==
T{ ' dictionary-test-negative catch }T -2 ==
T{ ' ordered-test-negative-key catch }T -2 ==
T{ ' dictionary-test-negative-key catch }T -2 ==
T{ ' stale-lookup catch }T -2 ==
T{ ' stale-insert catch }T -2 ==
T{ ' stale-iterate catch }T -2 ==
T{ ' stale-simple-iterate catch }T -2 ==
T{ ' stale-free catch }T -2 ==
T{ ' bogus-lookup catch }T -2 ==
T{ ' null-insert catch }T -2 ==
T{ s" normal" s" key" plain-wordlist-test => }T ==
T{ s" key" plain-wordlist-test >string hashS }T s" normal" hashS ==
T{ dictionary-test free-map }T ==
T{ plain-wordlist-test count-keys }T 1 ==

s" first-value" s" FIRST" transient =>
s" second-value" s" SECOND" transient =>
s" replacement" s" FIRST" transient =>

: ordered-test-value ( c-addr u map -- hash )
    >string hashS
;

: invalid-map-iterate ( -- )
    ['] ordered-test-value invalid-map simple-iterate-map drop
;

: invalid-map-lookup ( -- )
    s" FIRST" invalid-map item? 2drop
;

: invalid-map-reset ( -- )
    invalid-map reset-map
;

: invalid-map-free ( -- )
    invalid-map free-map
;

T{ transient count-keys }T 2 ==
T{ transient ordered-map-valid? }T -1 ==
invalid-map ORDERED_MAP_CAPACITY @ invalid-map-capacity !
invalid-map ORDERED_MAP_ITEM_SIZE @ invalid-map-item-size !
invalid-map ORDERED_MAP_KEY_SPACE @ invalid-map-key-space !
1234 invalid-map ORDERED_MAP_NEXT !
T{ invalid-map ordered-map? }T -1 ==
T{ invalid-map ordered-map-valid? }T -1 ==
invalid-map ORDERED_MAP_CAPACITY @ 1+ invalid-map ORDERED_MAP_COUNT !
T{ invalid-map ordered-map-valid? }T 0 ==
T{ ' invalid-map-iterate catch }T -2 ==
invalid-map-capacity @ invalid-map ORDERED_MAP_COUNT !
0 invalid-map ORDERED_MAP_ITEM_SIZE !
T{ invalid-map ordered-map-valid? }T 0 ==
T{ ' invalid-map-lookup catch }T -2 ==
T{ ' invalid-map-reset catch }T -2 ==
T{ ' invalid-map-free catch }T -2 ==
invalid-map-item-size @ invalid-map ORDERED_MAP_ITEM_SIZE !
0 invalid-map ORDERED_MAP_KEY_SPACE !
T{ invalid-map ordered-map-valid? }T 0 ==
T{ ' invalid-map-iterate catch }T -2 ==
invalid-map-key-space @ invalid-map ORDERED_MAP_KEY_SPACE !
T{ s" FIRST" transient >string hashS }T s" replacement" hashS ==
T{ s" SECOND" transient >string hashS }T s" second-value" hashS ==
T{ ' ordered-test-value transient simple-iterate-map }T
    s" replacement" hashS s" second-value" hashS ==

T{ 0 transient ordered-map-entry count hashS }T s" FIRST" hashS ==
T{ 1 transient ordered-map-entry count hashS }T s" SECOND" hashS ==

T{ 1234 transient reset-map }T 1234 ==
T{ transient count-keys }T 0 ==

s" after-reset" s" FRESH" transient =>
T{ transient count-keys }T 1 ==
T{ s" FRESH" transient >string hashS }T s" after-reset" hashS ==

Tend

transient free-map
value-test free-map
invalid-map free-map
bye
