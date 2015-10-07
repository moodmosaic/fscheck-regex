module FsCheck.Regex.Tests

open Fare
open FsCheck
open FsCheck.Xunit
open System.Text.RegularExpressions

let matching pattern =
    Gen.sized (fun size ->
        let xeger = Xeger pattern
        let count = if size < 1 then 1 else size
        [ for i in 1..count -> xeger.Generate() ]
        |> Gen.elements
        |> Gen.resize count)

[<Property>]
let ``Turns a regular expression into a generator of strings matching that regex``() =
    let pattern = "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$"
    let matches = Arb.fromGen (matching pattern)

    Prop.forAll matches (fun s -> Regex.IsMatch(s, pattern))
