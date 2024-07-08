codeunit 50100 MyCodeunit
{
    var
        MySingleInstanceCodeunit: Codeunit MySingleInstanceCodeunit;

    procedure DoSomething()
    begin
        [|ClearAll();|]
    end;
}

codeunit 50101 MySingleInstanceCodeunit
{
    SingleInstance = true;

    var
        GlobalDocumentNumber: Code[20];
}