codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myKeyValue: Text;
        mySecret: Text;
    begin
        IsolatedStorage.Set(myKeyValue, [|mySecret|], DataScope::Company);
    end;
}