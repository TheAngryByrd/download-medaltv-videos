#r "nuget: Newtonsoft.Json"

open System
open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.Net.Http
open System.Threading.Tasks

type Poster = {
    userName : string
}

type Content = {
    contentTitle : string
    contentUrl : string
    poster : Poster
    created : int
}

let readInFile filePath =
    File.ReadAllText(filePath)

let parseToJson text =
    JsonConvert.DeserializeObject<Content array> text

let client = new HttpClient()

let runSync (t : Task<_>) = t.GetAwaiter().GetResult()

let sanitizeFileName (origFileName : string) =
    let invalids =System.IO.Path.GetInvalidFileNameChars()
    String.Join("_", origFileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries) ).TrimEnd('.')

let (</>) (path1 : string) path2 = Path.Join(path1, path2)



let downloadFile totalLength index (content : Content)  = 
    task {
        try
            printfn $"Downloading {index}/{totalLength}"
            let fileName = sanitizeFileName content.contentTitle
            let userName = sanitizeFileName content.poster.userName
            let downloadDir = "downloads" </> userName
            Directory.CreateDirectory(downloadDir) |> ignore
            let downloadPath = downloadDir  </> $"{fileName}-{content.created}.mp4"
            use fs = new FileStream(downloadPath, FileMode.CreateNew)
            let! result = client.GetAsync(content.contentUrl)
            let! downloadStream = result.Content.ReadAsStreamAsync()
            do! downloadStream.CopyToAsync fs
        with e ->
            eprintf $"Failed download {content.contentTitle} -> %A{e}"
            printfn "Attempting next file..."
    }
    |> runSync

let jsonFilePaths =  
    [|
        "your-json-files-here.json"
    |]
    |> Array.map (fun file -> __SOURCE_DIRECTORY__ </> "content-files" </> file)

let content = 
    jsonFilePaths
    |> Array.collect (readInFile >> parseToJson)

content
|> Array.iteri (downloadFile content.Length)