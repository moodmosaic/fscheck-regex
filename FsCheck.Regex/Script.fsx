
#r "../packages/FsCheck/lib/net45/FsCheck.dll"


open FsCheck
open System

let matching pattern =
    Gen.sized (fun size ->
        let xeger = Xeger pattern
        let count = if size < 1 then 1 else size
        [ for i in 1..count -> xeger.Generate() ]
        |> Gen.elements
        |> Gen.resize count)

let generate = Gen.sample 1000 9

let a = generate (matching "[0-9a-z]")
let b = generate (matching "[0-9a-z]")
let c = generate (matching "[0-9a-z]")

let d = generate (matching "\d{1,3}\.\d{1,3}\.\d{1,3}")
let e = generate (matching "\d{1,3}\.\d{1,3}\.\d{1,3}")
let f = generate (matching "\d{1,3}\.\d{1,3}\.\d{1,3}")

let g = generate (matching "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$")
let h = generate (matching "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$")
let i = generate (matching "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$")

let j = generate (matching "^[a-zA-Z''-'\s]{1,40}$")
let k = generate (matching "^[a-zA-Z''-'\s]{1,40}$")
let l = generate (matching "^[a-zA-Z''-'\s]{1,40}$")
