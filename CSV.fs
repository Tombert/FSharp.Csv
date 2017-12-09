namespace Vertigo.CsvParse 
module Csv =
    open Microsoft.FSharp.Reflection
    open CSVParse
    open System.Text
    open FSharp.Reflection

    let handleResult<'a> header rest = 
        let aType = typeof<'a>
        let recordFields = FSharpType.GetRecordFields(aType)
        let toMapFunc x =
            let mappedData =
                recordFields
                |> Array.map (fun y -> Map.find y.Name x :> obj)
            mappedData

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

    let readFileStream (filepath: string) = seq {
        use filestream = new System.IO.StreamReader(filepath)
        while (not filestream.EndOfStream ) do
            yield filestream.ReadLine()
        done
        }
    let deserializeFromFile<'a> (delimiter: string) size (filepath: string) = 
        let res = 
            filepath
            |> readFileStream
            |> Seq.chunkBySize size
            |> Seq.collect (
                (String.concat "\n")
                >> (fun x ->
                    let res  = CSVParse.ParseCsv x ","
                    res.Result))
        //    |> Seq.concat
        let (header, rest) = (Seq.head res, Seq.tail res)
        handleResult<'a> header rest

    let getAttributes (x: System.Reflection.PropertyInfo) = 
        let at = x.GetCustomAttributes(typeof<CsvProperty>, false)
        if at.Length >= 1 then
            at.[0] :?> CsvProperty
        else
            CsvProperty.Default

    let serialize delim (objekt: #obj seq) = seq {
        let first = Seq.head objekt
        let aType = first.GetType()
        let recordFields = FSharpType.GetRecordFields(aType)
        let newfields = 
            recordFields
            |> Array.sortBy (fun x ->
                let ats = getAttributes x
                ats.Order)
            |> Array.map ((fun x -> x.Name) )
        let res =
            objekt
            |> Seq.map
                ((fun i ->
                 newfields
                 |> Array.map (fun j -> aType.GetProperty(j).GetValue(i).ToString()) ) >> (String.concat delim))
        
        yield (newfields |> String.concat delim)
        yield! res
        }



        
        // let csvResult = CSVParse.ParseCsvStream filepath delimiter
        // let csvSeq = csvResult.Result
        // let (header, rest) = (csvSeq |> Seq.head |> List.ofSeq, Seq.tail csvSeq)
        // handleResult<'a> header rest 


