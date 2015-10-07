# Regex-constrained strings with FsCheck #

## Prelude ##

In Haskell, there's the [quickcheck-regex](https://hackage.haskell.org/package/quickcheck-regex) package, by [Audrey (唐鳳) Tang](https://www.linkedin.com/in/tangaudrey), which allows to write and execute this:

```haskell
generate (matching "[xX][0-9a-z]")
// Prints -> "''UVBw"
```

It exports a `matching` function that turns a Regular Expression into a [DFA](https://en.wikipedia.org/wiki/Deterministic_finite_automaton)/[NFA](https://en.wikipedia.org/wiki/Nondeterministic_finite_automaton) finite-state machine and then into a generator of strings matching that regex:

```haskell
matching :: String -> Gen String
```

## FsCheck ##

In F#, a similar generator for [FsCheck](http://fscheck.github.io/FsCheck) can be written as:

```fsharp
let matching pattern =
    Gen.sized (fun size ->
        let xeger = Xeger pattern
        let count = if size < 1 then 1 else size
        [ for i in 1..count -> xeger.Generate() ]
        |> Gen.elements
        |> Gen.resize count)
```

The `matching` function uses the [.NET port](https://www.nuget.org/packages/Fare/) of [dk.brics.automaton](http://www.brics.dk/automaton/) and [xeger](https://code.google.com/p/xeger/), and has the signature:

```fsharp
val matching : pattern:string -> Gen<string>
```

### Use in F# Interactive ###

Here's a way to generate regex-constrained strings with FsCheck in F# Interactive:

```fsharp
#r "../packages/Fare/lib/net35/Fare.dll"
#r "../packages/FsCheck/lib/net45/FsCheck.dll"

open Fare
open FsCheck

let matching pattern =
    Gen.sized (fun size ->
        let xeger = Xeger pattern
        let count = if size < 1 then 1 else size
        [ for i in 1..count -> xeger.Generate() ]
        |> Gen.elements
        |> Gen.resize count)

let generate = Gen.sample 1000 9

let a = generate (matching "[xX][0-9a-z]")
// val a : string list = ["X3"; "X1"; "x3"; "Xg"; "X7"; "xt"; "x5"; "xe"; "Xl"]

let b = generate (matching "\d{1,3}\.\d{1,3}\.\d{1,3}")
// val b : string list =
//  ["4.2.2"; "31.28.6"; "6.9.2"; "12.25.1"; "61.6.3"; "61.6.174"; "8.6.3";
//   "859.3.052"; "4.5.332"]

let c = generate (matching "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$")
// val c : string list =
//  ["http://ng.EO-qzK8d8-o-2w.U1.-1-6-0793D-88t61.-Zk4K-crxu7L"+[222 chars];
//   "http://Xi-3SP6--K.VU";
//   "http://B96-4.-B..-..L.y.W4xDpM81...3Y.-BuR.Q8iZ.4-Aiv.---"+[101 chars];
//   "http://H.M.fQ..F.-dKYk.3.A-7.G-0rHIV--o81ft-h0G4sHBpx..-S"+[152 chars];
//   "http://55..-Zl33Pjh..97.b.9i1-e2y10..S1P-If.K-1KA-UW-O12L"+[108 chars];
//   "http://1L-3w..N22gftE2OI68.NU5.4F.D2jD43hG41DC.ATL...-r2V"+[66 chars];
//   "http://.3R8.-.-YJh3OP0MZgFn.l.Z.Zv..Y-wOy-.0P6j.-4Uyn7IB."+[167 chars];
//   "http://DD.1y54-4Bb8oU2-.T3-kVVzTtYT..-o..IU.2J8lNHZ8p...j"+[126 chars];
//   "http://mwa7NSM-Y-Ly5w..LFF"]

let d = generate (matching "^[a-zA-Z''-'\s]{1,40}$")
// val d : string list =
//  ["pqVm"; "tX'iu'SyM'"; "Q''DU'''''or'''O"; "l'L'M''Ew'YH";
//   "'O'CV'''U'''S''h"; "'"; "OQgJ'G'fR"; "''VGHWmcUB'T"; "''P"]
```

---

Notice how FsCheck yields different results on each run:

```fsharp
let a = generate (matching "[xX][0-9a-z]")
let b = generate (matching "[xX][0-9a-z]")
let c = generate (matching "[xX][0-9a-z]")

// val a : string list = ["x2"; "xn"; "Xc"; "Xf"; "Xm"; "xv"; "x6"; "Xi"; "Xn"]
// val b : string list = ["x7"; "X9"; "X3"; "xv"; "xn"; "xk"; "xn"; "xv"; "xo"]
// val c : string list = ["X5"; "xj"; "X0"; "xw"; "xq"; "Xg"; "xj"; "xv"; "X6"]
```

That's because `matching` takes [the size of generated test data](http://blog.nikosbaxevanis.com/2015/03/21/the-sample-function-and-the-size-of-generated-test-data/) into account.

### Use in FsCheck with xUnit.net ###

Finally, here's a [quantified property](https://fscheck.github.io/FsCheck/Properties.html) scenario using FsCheck with [xUnit.net](https://xunit.github.io/):

```fsharp
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
let ``Turns a regex into a generator of strings matching that regex``() =
    let pattern = "^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$"
    let matches = Arb.fromGen (matching pattern)
    Prop.forAll matches (fun s -> Regex.IsMatch(s, pattern))

// Output:
//  Ok, passed 100 tests.
//
// 1 passed, 0 failed, 0 skipped, took 1.34 seconds (xUnit.net 1.9.2).
```
