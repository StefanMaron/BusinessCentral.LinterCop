codeunit 50100 MyCodeunit
{
    procedure GetCompanySystemId(): Guid
    var
        Company: Record Company;
    begin
        Company.Get({|Database.CompanyName()|]);
        exit(Company.Id);
    end;
}

table 50100 Company
{
    fields
    {
        field(1; Name; Text[30]) { }
        field(2; Id; Guid) { }
    }
}