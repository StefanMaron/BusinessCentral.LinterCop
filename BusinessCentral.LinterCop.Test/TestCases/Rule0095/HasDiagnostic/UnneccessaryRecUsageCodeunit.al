table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }

    procedure DoSth()
    begin
    end;
}


codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        Rec: Record MyTable;
    begin
        [|Rec|].DoSth();
        [|Rec|].Name := 'Name';
    end;
}