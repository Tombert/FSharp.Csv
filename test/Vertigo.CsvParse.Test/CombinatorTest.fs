module CombinatorTest

open System
open Xunit
open Vertigo.CsvParse
open FParsec.Primitives
open FParsec.CharParsers



[<Fact>]
let ``Test Escape Characters`` () =
    let result = run CSVParse.escapeChar "\\n"
    match result with
    | Success (x, y, z) -> Assert.Equal(x, '\n')
    | Failure (s, _, _) -> failwith (sprintf "Broken: %A" s)
    let result = run CSVParse.escapeChar "\\r"
    match result with
    | Success (x, y, z) -> Assert.Equal(x, '\r')
    | Failure (x, _, _)-> failwith (sprintf "Broken: %A" x)

[<Fact>]
let ``Test Parsing String`` () =
    let csvstring = "blah,test\nyo,foo"
    let result = run (CSVParse.csv ",") csvstring
    match result with
    | Success (x, _, _) ->
        let finalArray =
            x
            |> Array.ofList
            |> Array.map (Array.ofList)
        Assert.Equal(x.[0].[0], "blah")
        Assert.Equal(x.[0].[1], "test")
        Assert.Equal(x.[1].[0], "yo")
        Assert.Equal(x.[1].[1], "foo")
    | Failure (x, _, _) -> failwith (sprintf "Broken: %A" x)
