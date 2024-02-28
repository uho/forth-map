[defined] end-unit 0= [IF] 
: unit ( <name> -- unit-sys ) Create ;
: internal ( unit-sys1 -- unit-sys2 ) ;
: external ( unit-sys1 -- unit-sys2 ) ;
: end-unit ( unit-sys -- ) ;
[THEN]

unit -map-

\ How to use maps:
\ In order to create a map you have two choices:
\ 
\ 1. Create a named map (similar to a Forth VARIABLE):
\     `Map: <name>`
\    `<name>` gets the stack effect `( c-addr u -- addr ).
\    Executing `<name>` later will take a key string and will return 
\    a cell address where a value can be stored and retrieved with `!` and `@`.
\    Each time you invoke `<name>` on the same string, you get the same address.
\    (*same string* in the sense of a string with the same characters, 
\    `c-addr` might be different).
\ 
\ 2. Create an unnamed map (similar to OPEN-FILE)
\     `map` 
\    This puts a map identifier on the stack which you can store in a
\    variable or use it to define a constant:
\     `VARIABLE my-map    map my-map !`
\ 
\    In order to access items in the map you put the map identifier on the
\    stack and the perfom the desired operation on the map:
\ 
\    - store a value (e.g. 99) for a given key (e.g. <a-key>)
\      `99   S" <a-key>"  my-map @  >value !`
\    - fetch a value for a given key (e.g. <a-key>)
\      `S" <a-key>"  my-map @  >value @`
\ 
\    - iterate through the keys of a map
\       ```:noname ( i*x c-addr u -- j*x flag ) 
\             type space true ;  
\          my-map @  iterate-map
\       ```
\ 
\     `iterate-map ( i*x xt map -- j*x )`
\     will take a map identifier `map` and an execution token `xt` from the stack
\     and invokes `xt` on every key in the map until all keys are processed
\     or until `xt` returns a `false` value. The stack `i*x` below the `xt` is
\     guaranteed to be accessible by `xt` so it can be used to manage context
\     information required for `xt` (such as the map identifier or a count).

\ ************ Implementation *************

\ Maps aka key/value structures are implemented
\ here with wordlists. 
\ So internally we call them word lists `wid` but externally
\ we call them maps `map`.

\ Check if there is an item in the given word list `wid`
\ with key `c-addr` `u`.
\ If the item exists return its body address and `true`.
\ If the item does not exist, return 0 and `false`.
: item? ( c-addr u wid -- addr true | 0 false )
   search-wordlist IF >body true EXIT THEN 0 false ;

\ Create a variable like item with name `c-addr` `u` 
\ in the current word list.
: "variable ( c-addr u -- )
   s" CREATE " >r pad r@ move \ CREATE 
   pad r@ + swap dup >r move  \ caddr u 
   pad r> r> + evaluate 
   0 , ;

\ Create a new item with name `c-addr` `u` in the word list `wid`.
\ Return its body address.
: new-item ( c-addr u wordlist -- addr )
   get-current >r set-current
   ['] "variable catch 
   r> set-current throw 
   here cell- ; 

external

\ Return the body address of an item with key `c-addr` `u`
\ in the key/value map `map`
: >value ( c-addr u map -- addr )
    >r 2dup r@ item? IF r> drop  nip nip EXIT THEN drop
    r> new-item ;

\ Create a new map with the identifier `map` that can be 
\ passed on the stack and stored in variables etc.
\ Items in the map are accessed via `>value` in the
\ ( c-addr u map -- addr )
: map ( -- map ) wordlist ;

\ Create a new named map `<name>`. Items in `<name>` are
\ accessed in the form: ( c-addr u <name> -- addr ).
: Map: ( <name> -- )
    map Create ,
    Does> ( c-addr u -- addr ) @ >value ;

internal

: invoke-xt ( i*x xt nt -- j*x xt flag )
   swap >r  name>string r@ execute  r> swap ;

external

: iterate-map ( i*x xt map -- j*x )
   ['] invoke-xt swap traverse-wordlist drop ;

end-unit

cr .( Key/Value maps loaded )
cr .( Usage: map Constant <map> )
cr .(          x s" key" <map> >value ! )
cr .(            s" key" <map> >value @ )
cr .(    or: )
cr .(        Map: <name> )
cr .(          x s" key" <name> ! )
cr .(            s" key" <name> @ )

1 [IF] \ do test

marker *test*

include ttester.fs

map Constant ma

t{ s" xlerb" ma >value dup @ -> s" xlerb" ma >value 0 }t

Map: mama

t{ s" xlerb" mama dup @ -> s" xlerb" mama 0 }t

: print-key ( c-addr u -- flag )
    type space true ;

: keys ( map -- ) 
    cr ." keys of map " dup . 
    cr ['] print-key swap iterate-map 
    cr ;

ma keys

*test*

: print-key/value ( ctr map c-addr u -- ctr map flag )
   >r >r 
   cr ."   " dup r> r> 2dup type ." : " rot >value @ 0 .r ." ,"
   swap 1+ swap
   true ;

: print-map ( map -- )
   >r 
   cr ." {"
   0  r@ ['] print-key/value  r> iterate-map  drop
   cr ." }"
   cr . ." items" ;

[THEN]
