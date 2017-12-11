namespace Vertigo.CsvParse 
module Csv =
    open Microsoft.FSharp.Reflection
    open CSVParse
    open System.Text
    open System.IO
    open System.CodeDom.Compiler
    open FSharp.Reflection

    let handleResult<'a> header rest = 
        let aType = typeof<'a>
        let recordFields = FSharpType.GetRecordFields(aType)
        let toMapFunc (x: Map<string,string>) =
            let mappedData =
                recordFields
                |> Array.map
                    (fun y ->
                        let converter =
                            System.ComponentModel.TypeDescriptor.GetConverter(y.PropertyType)
                        converter.ConvertFromString(Map.find y.Name x))
            mappedData

        let mappedData = 
            rest
            |> Seq.map (List.zip header
                >> Map.ofList
                >> toMapFunc
                >> (fun x -> FSharpValue.MakeRecord(aType, x) :?> 'a))
        mappedData

    let toLiteral (input : string) = 
         let literal = new StringBuilder(input.Length + 2)
         //literal.Append("\"") |> ignore
         input |> String.iter (fun x ->
             let c = int x
             match x with
             |'\'' -> literal.Append(@"\'")
             |'\"' -> literal.Append("\\\"")
             |'\\' -> literal.Append(@"\\")
             | '\a' -> literal.Append(@"\a")
             | '\b' -> literal.Append(@"\b")
             | '\f' -> literal.Append(@"\f")
             | '\n' -> literal.Append(@"\n")
             | '\r' -> literal.Append(@"\r")
             | '\t' -> literal.Append(@"\t")
             | '\v' -> literal.Append(@"\v")
             | _ ->
                 if c >= 0x20 && c <= 0x7e then
                     literal.Append(x)
                 else 
                     literal.Append(@"\u") |> ignore
                     literal.Append(c)
             |> ignore
             ())
         literal.ToString()
         
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
            |> Array.map ((fun x ->
                let ats = getAttributes x
                (x.Name, ats.Quote)))
        let res =
            objekt
            |> Seq.map
                ((fun i ->
                 newfields
                 |> Array.map
                     (fun (name, quote) ->
                         let myString = aType.GetProperty(name).GetValue(i).ToString() |> toLiteral
                         if quote then
                             sprintf "\"%s\"" myString
                         else myString

                      ) ) >> (String.concat delim))
        
        yield
            (newfields
                |> Array.map
                    (fun (name, quote) ->
                        if quote then
                            sprintf "\"%s\"" name
                        else name)
               |> String.concat delim)
        yield! res
        }

    let serializeToFile delim (filepath:string) (objekt : #obj seq) =
        let result = serialize delim objekt
        use sw = new System.IO.StreamWriter(filepath)
        result 
        |> Seq.iter (fun x -> sw.WriteLine x)
