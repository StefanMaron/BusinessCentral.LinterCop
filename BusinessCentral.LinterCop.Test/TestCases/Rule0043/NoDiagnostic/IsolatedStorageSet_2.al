codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myKeyValue: Text;
        mySecret: SecretText;
    begin
        IsolatedStorage.Set(myKeyValue, [|mySecret|]);
    end;
}