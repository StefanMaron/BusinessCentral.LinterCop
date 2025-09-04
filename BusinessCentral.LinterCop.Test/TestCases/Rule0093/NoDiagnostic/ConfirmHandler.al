codeunit 50100 MyTestCodeunit
{
    Subtype = Test;
    TestPermissions = Disabled;

    [Test]
    [HandlerFunctions('ConfirmYes')]
    procedure MyProcedure()
    begin
    end;

    [ConfirmHandler]
    procedure [|ConfirmYes(Question: Text[1024]; var Response: Boolean)|]
    begin
    end;
}