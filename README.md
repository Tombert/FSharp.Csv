# Vertigo.CsvParse
## A reflection-based CSV Parser that 

```
open Vertigo.CsvParse

let mystring = """hello, world
parsing,csv
"""
type MyCoolType = {
    hello: string
    world: string
}
let myDeserializedType : MyCoolType seq = Csv.deserialize<MyCoolType> "," mystring
```
