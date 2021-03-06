namespace Vertigo.CsvParse 
module CSVParse =
  open FParsec.Primitives
  open FParsec.CharParsers

  type CsvResult = { IsSuccess : bool; ErrorMsg : string; Result : seq<string list> }

  let isWs = Seq.map (isAnyOf "\t ") >> Seq.reduce (&&)

  let ws = spaces
  let chr c = skipChar c
  let st str = if isWs str then skipString str else skipString str .>> ws
  let ch c = chr c .>> ws

  let escapeChar = chr '\\' >>. anyOf "\"\\/bfnrt," 
                    |>> function
                      | 'b' -> '\b'
                      | 'f' -> '\u000C'
                      | 'n' -> '\n'
                      | 'r' -> '\r'
                      | 't' -> '\t'
                      | c   -> c


  let nonQuotedCellChar delim = escapeChar <|> (noneOf (delim + System.Environment.NewLine))
  let cellChar = escapeChar <|> (noneOf "\"")

  let cell delim = between (chr '\"') (chr '\"') (manyChars cellChar) <|> manyChars (nonQuotedCellChar delim)

  let row delim = sepBy (cell delim) (st delim)
  let csv delim = sepBy (row delim) newline .>> eof
  let commaCsv = csv ","

  let stripEmpty ls = Seq.filter (fun (row:string list) -> (row.Length <> 0) && (row <> [""]) ) ls

  let ParseCsv s delim  =
      let res = run (csv delim) s in
          match res with
          | Success (rows, _, _) -> { IsSuccess = true; ErrorMsg = "Ok"; Result = stripEmpty rows }
          | Failure (s, _, _) -> { IsSuccess = false; ErrorMsg = s; Result = [[]] |> Seq.ofList}

