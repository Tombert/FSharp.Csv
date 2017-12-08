namespace Vertigo.CsvParse 
module Csv =
    open Microsoft.FSharp.Reflection
    open CSVParse
    open FSharp.Reflection

    let handleResult<'a> header rest = 
        let aType = typeof<'a>
        let recordFields = FSharpType.GetRecordFields(aType)
        let toMapFunc x =
            let mappedData =
                recordFields
                |> Array.map (fun y -> Map.find y.Name x :> obj)
            mappedData
        printfn "Header: %A" header
        printfn "Rest: %A" rest

        let mappedData = 
            rest
            |> Seq.map (
                List.zip header
                >> Map.ofList
                >> toMapFunc
                >> (fun x -> FSharpValue.MakeRecord(aType, x) :?> 'a))
        mappedData

    let deserialize<'a> delimiter mystring =
        let csvResult = CSVParse.ParseCsv mystring delimiter
        let csvSeq = csvResult.Result
        let (header, rest) = (csvSeq |> Seq.head |> List.ofSeq, Seq.tail csvSeq)
        handleResult<'a> header rest 

    let deserializeFromFile<'a> delimiter filepath = 
        let csvResult = CSVParse.ParseCsvStream filepath delimiter
        let csvSeq = csvResult.Result
        let (header, rest) = (csvSeq |> Seq.head |> List.ofSeq, Seq.tail csvSeq)
        handleResult<'a> header rest 


