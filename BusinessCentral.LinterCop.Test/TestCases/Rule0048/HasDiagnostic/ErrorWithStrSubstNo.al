codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        UnexpectedErr: Label '%1 is not an valid Record.';
    begin
        [|Error(StrSubstNo(UnexpectedErr, MyTable.MyField))|];
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}