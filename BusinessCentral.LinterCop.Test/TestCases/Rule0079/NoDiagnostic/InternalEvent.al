table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }

    [BusinessEvent(true)]
    internal procedure [|MyProcedure|]()
    begin
    end;

    [IntegrationEvent(true, false)]
    internal procedure [|MyProcedure2|]()
    begin
    end;

    [InternalEvent(false)]
    internal procedure [|MyProcedure3|]()
    begin
    end;
}