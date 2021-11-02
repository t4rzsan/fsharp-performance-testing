// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.Diagnostics

type Data =
    { Property1: string
      Property2: int
      Property3: DateTime 
      Property4: float
      Property5: decimal }

let processA data = { data with Property1 = "blabla2" }
let processB data = { data with Property2 = 100 }
let processC data = { data with Property3 = DateTime.Now }
let processD data = { data with Property4 = 41.2 }
let processE data = { data with Property5 = 23m }

let processResultA data = Ok { data with Property1 = "blabla2" }
let processResultB data = Ok { data with Property2 = 100 }
let processResultC data = Ok { data with Property3 = DateTime.Now }
let processResultD data = Ok { data with Property4 = 41.2 }
let processResultE data = Ok { data with Property5 = 23m }

let runWithPiping data =
    data
    |> processA
    |> processB
    |> processC
    |> processD
    |> processE

let runWithBinding data =
    data
    |> processResultA
    |> Result.bind processResultB
    |> Result.bind processResultC
    |> Result.bind processResultD
    |> Result.bind processResultE

let (>>=) m f =
    Result.bind f m 

let runWithOperatorWithoutInlining data =
    data
    |> processResultA
    >>= processResultB
    >>= processResultC
    >>= processResultD
    >>= processResultE

let inline (>>==) m f =
    Result.bind f m 

let runWithOperatorWithInlining data =
    data
    |> processResultA
    >>== processResultB
    >>== processResultC
    >>== processResultD
    >>== processResultE

let inline (>>===) m f =
    match m with
    | Error e -> Error e
    | Ok x -> f x

let runWithOperatorWithOwnImplementationAndInlining data =
    data
    |> processResultA
    >>=== processResultB
    >>=== processResultC
    >>=== processResultD
    >>=== processResultE

let inline (>=>) a b x =
    match a x with
    | Ok v -> b v
    | Error e -> Error e

let runWithKleisli data =
    data
    |> (processResultA
    >=> processResultB
    >=> processResultC
    >=> processResultD
    >=> processResultE)

let timeFunction f data =
    let sw = Stopwatch()
       
    sw.Start()
    
    [0 .. 1_000_000]
    |> List.iter (fun _ -> f data |> ignore)
    
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
    
let print functionName elapsed =
    let timespan = TimeSpan.FromMilliseconds(elapsed)
    printfn "Elapsed on average (%s): %A" functionName timespan
    
let run f fName =
    let data =
        { Property1 = "Blabla"
          Property2 = 42
          Property3 = DateTime.Today
          Property4 = 23.2
          Property5 = 23m }

    [1 .. 10]
    |> List.averageBy (fun i -> timeFunction f data)
    |> print fName

[<EntryPoint>]
let main argv =
    run runWithPiping (nameof runWithPiping)
    run runWithBinding (nameof runWithBinding)
    run runWithOperatorWithInlining (nameof runWithOperatorWithInlining)
    run runWithOperatorWithoutInlining (nameof runWithOperatorWithoutInlining)
    run runWithOperatorWithOwnImplementationAndInlining (nameof runWithOperatorWithOwnImplementationAndInlining)
    run runWithKleisli (nameof runWithKleisli)

    0  
