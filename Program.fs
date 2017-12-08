namespace Vertigo.CsvParse 
module main = 
  open Vertigo.CsvParse
  open Csv
  open Microsoft.FSharp.Reflection

  type yo = {
      Head: string
      Shoulder: string
      Knees: string
      Toes: string
     }
  [<EntryPoint>]
  let main (_) =
      let csvthingy =
          """Head, Shoulder, Knees, Toes
Legs, Holders, Bees, Grow
Farts, Yo, Blah, Foo"""


      let parsedResult = Csv.deserialize<yo> "," csvthingy
      // let result = CSV.ParseCsv csvthingy ","
      // let t = dawg.GetType()
      // let b = FSharpType.GetRecordFields t
      // let blah = result.Result
      // let columns = Seq.head blah |> List.toArray
      // let body = Seq.tail blah
      // let f = 
      //   body
      //   |> Seq.map(Seq.zip columns >> Map.ofSeq)
      //   |> Seq.toArray

      printfn "Hello world: %A"  parsedResult
      0
