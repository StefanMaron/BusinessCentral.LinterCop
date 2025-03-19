[|enum|] 50100 "My Enum"
{
    AssignmentCompatibility = [|true|];
    Extensible = [|true|];

    [|value|](0; MyValue)
    {
        Caption = 'My value', Comment = 'My Comment', Locked = [|false|], MaxLength = 100;
    }
}

[|enum|] 50101 "My Interface Enum" [|implements|] "My Interface"
{
    [|value|](0; MyValue)
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