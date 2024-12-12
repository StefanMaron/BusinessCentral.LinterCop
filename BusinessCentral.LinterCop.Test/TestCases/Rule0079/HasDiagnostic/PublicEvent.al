table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }

    [BusinessEvent(true)]
    procedure [|MyProcedure|]()
    begin
    end;

    [IntegrationEvent(true, false)]
    procedure [|MyProcedure2|]()
    begin
    end;

    [InternalEvent(false)]
    procedure [|MyProcedure3|]()
    begin
    end;
}