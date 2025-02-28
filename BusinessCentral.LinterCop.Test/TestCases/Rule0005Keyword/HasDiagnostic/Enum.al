
[|ENUM|] 50100 "My Enum"
{
    AssignmentCompatibility = [|TRUE|];
    Extensible = [|TRUE|];

    [|VALUE|](0; MyValue)
    {
        Caption = 'My value', Comment = 'My Comment', Locked = [|FALSE|], MaxLength = 100;
    }
}

[|ENUM|] 50101 "My Interface Enum" [|IMPLEMENTS|] "My Interface"
{
    [|VALUE|](0; MyValue)
    {
        Implementation = "My Interface" = "My Codeunit";
    }
}

interface "My Interface"
{
    procedure MyProcedure();
}

codeunit 50100 "My Codeunit" implements "My Interface"
{
    procedure MyProcedure()
    begin
    end;
}