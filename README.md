# forth-map

String-keyed maps for VFX Forth, with two storage representations behind one
access API.

## Choosing a representation

| Representation | Storage | Use it for |
|---|---|---|
| `map` | Forth dictionary wordlist | Long-lived configuration and other data created once |
| `ordered-map` | Bounded heap allocation | Multiple instances and transient data that must be reset or freed |

A dictionary map permanently consumes dictionary space as new keys are
created. An ordered map allocates its complete capacity once; insertion and
reset do not grow the dictionary or allocate further memory.

Both representations store counted-string values, compare keys
case-insensitively, and use the same lookup, assignment, and iteration words.

## Loading

With ForthBase:

```forth
NEED forth-map
```

Without the manifest:

```forth
include map.fs
include map-tools.fs
```

## Dictionary map

```forth
map constant settings

s" M42" s" TARGET" settings =>
s" 30"  s" EXPOSURE" settings =>

s" TARGET" settings >string type
settings .map
```

Use a dictionary map when its keys are effectively permanent. It cannot be
reset or reclaimed individually.

## Ordered map

```forth
ordered-map constant metadata

s" Light" s" IMAGETYP" metadata =>
s" M42"   s" OBJECT" metadata =>

s" OBJECT" metadata >string type

metadata reset-map
s" Dark" s" IMAGETYP" metadata =>

metadata free-map
```

Keys retain their first insertion order. Assigning a new value to an existing
key replaces its value without moving it. `reset-map` clears all entries while
retaining the allocation for reuse. Call `free-map` exactly once when the
ordered map is no longer needed; do not use the handle afterward.

## Allocation limits

The following values configure subsequent `ordered-map` allocations:

```forth
64  -> map.key-space   \ maximum key bytes
128 -> map.capacity    \ maximum entries
256 -> map.space       \ bytes per counted value, including its count byte
```

Set them before calling `ordered-map`. Each map records its own limits, so
later changes do not alter existing maps. The default maximum value length is
255 bytes. Inserting an overlong key or exceeding capacity aborts explicitly.

An ordered map uses approximately:

```text
header + map.capacity * (map.key-space + 1 + map.space)
```

bytes. Lookup is linear in the number of entries; this keeps the implementation
small and is appropriate for bounded FITS metadata and similar structures.

## Public words

### Creation and lifecycle

```forth
map          ( -- map )   \ create a dictionary wordlist map
ordered-map  ( -- map )   \ allocate a bounded ordered map
reset-map    ( map -- )   \ clear an ordered map for reuse
free-map     ( map -- )   \ free an ordered map; no-op for a dictionary map
```

`reset-map` accepts only ordered maps. Addresses returned for old values must
not be retained across `reset-map` or `free-map`.

### Lookup and assignment

```forth
item?    ( c-addr u map -- addr true | 0 false )
>addr    ( c-addr u map -- addr )
=>       ( value-addr value-u key-addr key-u map -- )
=>"      ( value-addr value-u map "<key>" -- )
>string  ( key-addr key-u map -- value-addr value-u )
```

`item?` performs a non-creating lookup. `>addr`, `=>`, and `=>"` create a
missing key. `>string` is a strict read and aborts with `map item not found`
when the key is absent.

Example of the parsed-key form:

```forth
s" M31" settings =>" TARGET"
```

### Inspection and iteration

```forth
count-keys         ( map -- n )
.map               ( map -- )
iterate-map        ( i*x xt map -- j*x )
simple-iterate-map ( i*x xt map -- j*x )
```

`simple-iterate-map` presents ordered-map keys in insertion order. Its callback
receives:

```forth
( i*x key-addr key-u map -- j*x )
```

and is called for every key. `iterate-map` follows the lower-level
`traverse-wordlist` convention: its callback receives the key and returns a
flag controlling continuation.

`+map` and `-map` manipulate the Forth search order and therefore apply only
to dictionary wordlist maps.
