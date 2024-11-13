codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|enum|]::MyEnum::MyValue;
    end;
}

enum 50100 MyEnum { value(0; MyValue) { } }