codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        SalesHeader: Record "Sales Header";
        myInteger: Integer;
        DocumentNo: Code[20];
    begin
        [|SalesHeader.Get(myInteger, DocumentNo)|];
    end;
}

table 50100 "Sales Header"
{
    fields
    {
        field(1; "Document Type"; Enum "Sales Document Type") { }
        field(2; "No."; Code[20]) { }
    }
    keys
    {
        key(Key1; "Document Type", "No.") { }
    }
}

enum 50100 "Sales Document Type" { }