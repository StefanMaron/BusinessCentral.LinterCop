codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myKeyValue: Text;
        mySecret: Text;
    begin
        IsolatedStorage.Get(myKeyValue, DataScope::Module, [|mySecret|]);
    end;
}