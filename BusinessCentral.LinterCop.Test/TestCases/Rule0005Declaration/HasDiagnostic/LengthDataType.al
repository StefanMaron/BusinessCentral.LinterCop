codeunit 50000 MyCodeunit
{
    var
        myCode: [|CODE|][10];
        myText: [|TEXT|][100];
}

table 50000 MyTable
{
    fields
    {
        field(1; "Primary Key"; Integer) { }
        field(2; myCode; [|CODE|][10]) { }
        field(3; myText; [|TEXT|][100]) { }
    }
}