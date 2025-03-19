codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]() // Cognitive Complexity: 15 (threshold >=15)
    begin
        if true then                    // +1 (nesting = 0)
            if true then                // +2 (nesting = 1)
                if true then            // +3 (nesting = 2)
                    if true then        // +4 (nesting = 3)
                        if true then;   // +5 (nesting = 4)
    end;
}