table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }

    [BusinessEvent(true)]
    local procedure [|MyProcedure|]()
    begin
    end;

    [IntegrationEvent(true, false)]
    local procedure [|MyProcedure2|]()
    begin
    end;

    [InternalEvent(false)]
    local procedure [|MyProcedure3|]()
    begin
    end;
}