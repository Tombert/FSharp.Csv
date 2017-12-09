module Tests

open System
open Xunit
open Vertigo.CsvParse

type Blah = {
    Hello: string
    World: string

    }

[<Fact>]
let ``Parsing String`` () =
    let input = """Hello,World
test,thing
another,test"""
    let result =
        input
        |> Csv.deserialize<Blah> ","
        |> Seq.toArray
    Assert.Equal(result.[0].Hello, "test")
    Assert.Equal(result.[0].World, "thing")
    Assert.Equal(result.[1].Hello, "another")
    Assert.Equal(result.[1].World, "test")
