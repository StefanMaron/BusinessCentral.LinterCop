table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }

    procedure MyProcedure()
    begin
        [|Rec|].DoSth(Rec);
    end;

    procedure DoSth(MyTable : Record MyTable)
    begin
    end;
}