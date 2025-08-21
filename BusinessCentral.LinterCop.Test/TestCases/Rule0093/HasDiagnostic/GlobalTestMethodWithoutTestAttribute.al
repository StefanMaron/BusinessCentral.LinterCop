codeunit 50100 MyTestCodeunit
{
    Subtype = Test;
    TestPermissions = Disabled;

    procedure [|MyGlobalTestProcedure()|]
    begin
    end;

    [Test]
    procedure MyProcedure()
    begin
    end;
}