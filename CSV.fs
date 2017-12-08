namespace Vertigo.CsvParse 
module Csv =
    open Microsoft.FSharp.Reflection
    open CSVParse
    open FSharp.Reflection
    let deserialize<'a> delimiter mystring =
        let csvResult = CSVParse.ParseCsv mystring delimiter
        let csvSeq = csvResult.Result
        let (header, rest) = (csvSeq |> Seq.head |> List.ofSeq, Seq.tail csvSeq)
        let aType = typeof<'a>
        let recordFields = FSharpType.GetRecordFields(aType)
        let mappedData = 
            rest
            |> Seq.map (List.zip header >> Map.ofList)
            |> Seq.map (
                fun x ->
                    let mappedData =
                        recordFields
                        |> Array.map (fun y -> Map.find y.Name x :> obj)
                    mappedData)
            |> Seq.map (fun x -> FSharpValue.MakeRecord(aType, x) :?> 'a)
        mappedData



