namespace Vertigo.CsvParse 
type CsvProperty(prop) = 
    member val public Order : int = 1000 with get, set
    member val public Quote : bool = true with get, set
    new () = CsvProperty(null)
with
    static member Default = CsvProperty()



