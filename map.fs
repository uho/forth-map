[defined] unit 0= [IF] include unit.fs [THEN]

unit -map-

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

: map ( -- map ) wordlist ;

: Map: ( <name> -- )
    map Create ,
    Does> ( c-addr u -- addr ) @ >value ;

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

*test*

[THEN]
