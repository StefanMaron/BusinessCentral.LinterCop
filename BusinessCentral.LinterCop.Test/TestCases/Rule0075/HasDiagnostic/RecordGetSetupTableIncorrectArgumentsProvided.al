codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        CompanyInformation: Record "Company Information";
    begin
        [|CompanyInformation.Get('', 12345)|];
    end;
}

table 79 "Company Information"
{
    fields
    {
        field(1; "Primary Key"; Code[10]) { }
    }

    keys
    {
        key(Key1; "Primary Key") { }
    }
}