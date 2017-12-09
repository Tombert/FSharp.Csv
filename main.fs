namespace Vertigo.CsvParse 
module main = 

  open Csv
  type Fart = {
      [<CsvProperty(Order=2)>]
      Howdy: string
      [<CsvProperty(Order=1)>]
      Blah : string
      }
  [<EntryPoint>]
  let main args =
      let yo = deserializeFromFile<Fart> "," 10 "/home/tombert/yo.csv" 
      let blah = serializeToFile "," "/home/tombert/ffff.csv" yo
      printfn "Done" 
      0
