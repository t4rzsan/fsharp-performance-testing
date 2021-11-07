// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.Diagnostics

type Data =
    { Property1: string
      Property2: int
      Property3: DateTime 
      Property4: float
      Property5: decimal }

let processData data = { data with Property1 = "some new string" }

let processDataResult data = Ok { data with Property1 = "some new string" }

let withPiping data =
    data
    |> processData
    |> processData
    |> processData
    |> processData
    |> processData

let withBinding data =
    data
    |> processDataResult
    |> Result.bind processDataResult
    |> Result.bind processDataResult
    |> Result.bind processDataResult
    |> Result.bind processDataResult

let (>>=) m f =
    Result.bind f m 

let withOperatorWithoutInlining data =
    data
    |> processDataResult
    >>= processDataResult
    >>= processDataResult
    >>= processDataResult
    >>= processDataResult

let inline (>>==) m f =
    Result.bind f m 

let withOperatorWithInlining data =
    data
    |> processDataResult
    >>== processDataResult
    >>== processDataResult
    >>== processDataResult
    >>== processDataResult

let inline (>>===) m f =
    match m with
    | Error e -> Error e
    | Ok x -> f x

let withOperatorWithOwnImplementationAndInlining data =
    data
    |> processDataResult
    >>=== processDataResult
    >>=== processDataResult
    >>=== processDataResult
    >>=== processDataResult

let inline (>=>) a b x =
    match a x with
    | Ok v -> b v
    | Error e -> Error e

let withKleisli data =
    data
    |> (processDataResult
    >=> processDataResult
    >=> processDataResult
    >=> processDataResult
    >=> processDataResult)

let timeFunction f data =
    let sw = Stopwatch()
       
    sw.Start()
    
    [1 .. 10_000_000]
    |> List.iter (fun _ -> f data |> ignore)
    
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
    
let print functionName elapsed =
    let timespan = TimeSpan.FromMilliseconds(elapsed)
    printfn "Elapsed on average (%s): %A" functionName timespan
    
let run f fName =
    let data =
        { Property1 = "some string"
          Property2 = 42
          Property3 = DateTime.Today
          Property4 = 23.2
          Property5 = 23m }

    [1 .. 100]
    |> List.averageBy (fun i -> timeFunction f data)
    |> print fName

[<EntryPoint>]
let main argv =
    run withPiping (nameof withPiping)
    run withBinding (nameof withBinding)
    run withOperatorWithInlining (nameof withOperatorWithInlining)
    run withOperatorWithoutInlining (nameof withOperatorWithoutInlining)
    run withOperatorWithOwnImplementationAndInlining (nameof withOperatorWithOwnImplementationAndInlining)
    run withKleisli (nameof withKleisli)

    0  
