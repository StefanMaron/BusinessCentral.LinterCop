codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]() // Cognitive Complexity: 6 (threshold >=15)
    var
        i: Integer;
        Numbers: List of [Integer];
        Statement: Boolean;
    begin
        for i := 0 to 1 do begin        // +1 (nesting = 0)
            if Statement then           // +0 Guard Clause
                Continue;
        end;

        for i := 0 to 1 do              // +1 (nesting = 0)
            if Statement then           // +0 Guard Clause
                Continue;

        foreach i in Numbers do begin   // +1 (nesting = 0)
            if Statement then           // +0 Guard Clause
                Continue;
        end;

        foreach i in Numbers do         // +1 (nesting = 0)
            if Statement then           // +2 (nesting = 1)
                if Statement then       // +0 Guard Clause
                    Continue;
    end;
}