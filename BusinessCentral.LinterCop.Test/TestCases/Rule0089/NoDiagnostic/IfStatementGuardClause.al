codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]() // Cognitive Complexity: 0 (threshold >=15)
    begin
        if true then exit;          // +0 (nesting = 0)
        if true then Error('');     // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then                // +0 (nesting = 0)
            Error('something went wrong');
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
        if true then exit;          // +0 (nesting = 0)
    end;
}