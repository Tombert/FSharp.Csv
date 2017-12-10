module Tests

open System
open Xunit
open Vertigo.CsvParse

type Blah = {
    Hello: string
    World: string
    }

type MixedType = {
    MyString: string
    MyInt : int
    MyBool: bool
    MyFloat: decimal
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
let ``Parse Non String`` () =
    let input = """MyString,MyInt,MyBool,MyFloat
Hello,112,false,3.1415"""
    let parsed =
        Csv.deserialize<MixedType> "," input
        |> Seq.toArray
    Assert.Equal(parsed.[0].MyString,"Hello")
    Assert.Equal(parsed.[0].MyInt,112)
    Assert.Equal(parsed.[0].MyBool,false)
    Assert.Equal(parsed.[0].MyFloat,3.1415M)
    ()

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
    Assert.Equal(result.[0].Hello,"This")
    Assert.Equal(result.[0].World,"is")
    Assert.Equal(result.[1].Hello,"a")
    Assert.Equal(result.[1].World,"test")
    Assert.Equal(result.[2].Hello,"for")
    Assert.Equal(result.[2].World,"you")
    ()

[<Fact>]
let ``Serialize to File`` () = 
    let filename = "tempfile.csv"
    try 
        let initData = [|{Hello = "Howdy"; World = "Yall"}|]

        initData |> Csv.serializeToFile "," filename

        let yo = 
            filename
            |> Csv.deserializeFromFile<Blah> "," 10
            |> Array.ofSeq
        Assert.Equal(yo.[0].Hello, "Howdy")
        Assert.Equal(yo.[0].World, "Yall")
     finally
        System.IO.File.Delete filename
