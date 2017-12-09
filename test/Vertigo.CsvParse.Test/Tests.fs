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

[<Fact>]
let ``Serializing String`` () =
    let testInput =
        [|{
            Hello = "howdy"
            World = "yall"
          }
          {
            Hello = "Another"
            World = "Test"
          }
         |]
    let result = Csv.serialize "," testInput
    let first = Seq.head result
    let rest = Seq.tail result
    Assert.Equal(first, "Hello,World")
    let second = Seq.head rest
    let rest = Seq.tail rest
    Assert.Equal(second, "howdy,yall")
    let third = Seq.head rest
    Assert.Equal(third, "Another,Test")

