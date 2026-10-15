NEED forth-map
NEED simple-tester

ordered-map constant transient
ordered-map constant value-test
ordered-map constant invalid-map
map constant dictionary-test
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
test-fill-values

Tstart

T{ test-value-bytes 255 s" MAX" value-test => }T ==
T{ s" MAX" value-test >string nip }T 255 ==
T{ ' ordered-test-overlong catch 0<> }T -1 ==
T{ test-value-bytes 255 s" MAX" dictionary-test => }T ==
T{ s" MAX" dictionary-test >string nip }T 255 ==
T{ ' dictionary-test-overlong catch 0<> }T -1 ==

s" first-value" s" FIRST" transient =>
s" second-value" s" SECOND" transient =>
s" replacement" s" FIRST" transient =>

: ordered-test-value ( c-addr u map -- hash )
    >string hashS
;

: invalid-map-iterate ( -- )
    ['] ordered-test-value invalid-map simple-iterate-map drop
;

T{ transient count-keys }T 2 ==
T{ transient ordered-map-valid? }T -1 ==
invalid-map ORDERED_MAP_CAPACITY @ 1+ invalid-map ORDERED_MAP_COUNT !
T{ invalid-map ordered-map-valid? }T 0 ==
T{ ' invalid-map-iterate catch }T -2 ==
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
