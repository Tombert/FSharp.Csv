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

[<Fact>]
let ``Deserialize From File`` () =
    let fileName = "../../../test.csv"
    let result =
        fileName
        |> Csv.deserializeFromFile<Blah> "," 10 
        |> Seq.toArray 
    printfn "result: %A" result
    Assert.Equal(result.[0].Hello,"This")
    Assert.Equal(result.[0].World,"is")
    Assert.Equal(result.[1].Hello,"a")
    Assert.Equal(result.[1].World,"test")
    Assert.Equal(result.[2].Hello,"for")
    Assert.Equal(result.[2].World,"you")
    ()

