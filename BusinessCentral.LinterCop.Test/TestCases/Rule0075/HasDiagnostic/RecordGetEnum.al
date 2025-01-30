codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        MyEnum: Enum MyEnum;
    begin
        [|MyTable.Get(MyEnum::MyValue)|];
    end;
}

table 50100 MyTable
{
    fields { field(1; MyField; Code[20]) { } }
}
enum 50100 MyEnum
{
    value(0; MyValue) { }
}