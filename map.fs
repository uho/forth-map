synonym map wordlist
\ Maps aka key/value structures are implemente with wordlists. 
\ So internally we call them word lists `wid` but externally we call them maps `map`.

: +map ( map --)
\ make the keys of a map visible in the search order
	+ORDER
;

: -map ( map --)
\ make the keys of a map visible in the search order
	-ORDER
;

256 VALUE map.space
\ The size in bytes of the storage space for each value - default is 256 bytes for counted strings


: item? ( c-addr u wid -- addr true | 0 false )
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

: new-item ( c-addr u wordlist -- addr )
\ Create a new item with name `c-addr` `u` in the word list `wid`. Return its PFA (body address)	
   get-current >r set-current
   ['] "string catch 
   r> set-current throw 
   here map.space - ; 


: >addr ( c-addr u map -- addr )
\ Return the body address of an item with key `c-addr` `u` in the key/value map `map`	
    >r 2dup r@ item? IF 
    	nip nip r> drop
    ELSE
    	drop r> new-item 
    THEN
  ;

: invoke-xt ( i*x xt nt -- j*x xt flag )
   swap >r  name>string r@ execute  r> swap ;

: iterate-map ( i*x xt map -- j*x )
\ map iterator
   ['] invoke-xt swap traverse-wordlist drop ;


