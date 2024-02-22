# forth-map: Forth Key Value Data Structure map

This repository implements a key/value data structure
in Forth.

## Usage:

`include map.fs` ( requires UNIT words to be present )

## Glossary:

- `map ( -- map )`  
Create a new map with the identifier `map` that can be 
passed on the stack and stored in variables etc.  
Items in the map are accessed via `>value` in the form  
c-addr u map `>value` -- addr

- `Map: ( <name> -- )`  
Create a new named map `<name>`.  
Items in `<name>` are accessed in the form: 
    c-addr u `<name>` -- addr

- `>value ( c-addr u map -- addr )`  
Return the body address of an item with key `c-addr` `u`
in the key/value map `map`.

## Examples:

```forth
map Constant ma

s" xlerb" ma >value .    ( creates new item in map ma and prints its address )
s" xlerb" ma >value .    ( uses existing item in map ma and prints its address )
s" xlerb" ma >value @ .  ( uses existing item in map ma and prints its value )
99 s" xlerb" ma >value ! ( uses existing item in map ma and sets its value )

Map: mama

s" xlerb" mama .    ( creates new item in named map mama and prints its address )
s" xlerb" mama .    ( uses existing item in named map mama and prints its address )
s" xlerb" mama @ .  ( uses existing item in named map mama and prints its value )
99 s" xlerb" mama ! ( uses existing item in named map mama and sets its value )
```
