table 50100 MyTable
{
    fields
    {
        field(1; Id; Integer) { }
    }

    internal procedure CallBaseMethods()
    var SomeText: Text;
    begin
        Clear([|Rec|]);
        SomeText := Format([|Rec|]);
    end;
}