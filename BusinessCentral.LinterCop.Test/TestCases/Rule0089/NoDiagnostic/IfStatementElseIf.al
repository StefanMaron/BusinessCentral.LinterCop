codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]() // Cognitive Complexity: 14 (threshold >=15)
    var
        myBoolean: Boolean;
    begin
        if myBoolean then                  // +1 (nesting = 0)
            if myBoolean then begin        // +2 (nesting = 1)
                if true then;              // +3 (nesting = 2)
                if true then;              // +3 (nesting = 2)
            end else                       // +0 (else-if)
                if true then               // +2 (nesting = 1)
                    if true then;          // +3 (nesting = 2)
    end;
}