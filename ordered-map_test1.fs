NEED forth-map
NEED simple-tester

ordered-map constant transient

Tstart

s" first-value" s" FIRST" transient =>
s" second-value" s" SECOND" transient =>
s" replacement" s" FIRST" transient =>

T{ transient count-keys }T 2 ==
T{ s" FIRST" transient >string hashS }T s" replacement" hashS ==
T{ s" SECOND" transient >string hashS }T s" second-value" hashS ==

T{ 0 transient ordered-map-entry count hashS }T s" FIRST" hashS ==
T{ 1 transient ordered-map-entry count hashS }T s" SECOND" hashS ==

transient reset-map
T{ transient count-keys }T 0 ==

s" after-reset" s" FRESH" transient =>
T{ transient count-keys }T 1 ==
T{ s" FRESH" transient >string hashS }T s" after-reset" hashS ==

Tend

transient free-map
bye
