codeunit 50100 MyCodeunit
{
    procedure DoSomething()
    var
        MySingleInstanceCodeunit: Codeunit MySingleInstanceCodeunit;
    begin
        [|Clear(MySingleInstanceCodeunit);|]
    end;
}

codeunit 50101 MySingleInstanceCodeunit
{
    SingleInstance = true;

    var
        GlobalDocumentNumber: Code[20];
}