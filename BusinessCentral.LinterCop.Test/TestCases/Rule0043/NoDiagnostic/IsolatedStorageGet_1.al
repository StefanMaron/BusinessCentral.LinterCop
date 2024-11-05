codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myKeyValue: Text;
        mySecret: SecretText;
    begin
        IsolatedStorage.Get(myKeyValue, DataScope::Module, [|mySecret|]);
    end;
}