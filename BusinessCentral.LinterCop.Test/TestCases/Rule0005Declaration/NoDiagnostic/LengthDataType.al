codeunit 50000 MyCodeunit
{
    var
        myCode: [|Code|][10];
        myText: [|Text|][100];
}

table 50000 MyTable
{
    fields
    {
        field(1; "Primary Key"; Integer) { }
        field(2; myCode; [|Code|][10]) { }
        field(3; myText; [|Text|][100]) { }
    }
}