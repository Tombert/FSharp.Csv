namespace Vertigo.CsvParse 
module main = 

  open Csv
  type Fart = {
      Howdy: string
      Blah : string
      }
  [<EntryPoint>]
  let main args =
      let yo = deserializeFromFile<Fart> "," "/home/tombert/blah.csv" 
      printfn "Hello world: %A" yo
      0
