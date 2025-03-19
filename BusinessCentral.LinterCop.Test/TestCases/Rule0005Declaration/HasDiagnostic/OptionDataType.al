codeunit 50000 MyCodeunit
{
    var
        myOption: [|OPTION|] A,B,C;
}

table 50000 MyTable
{
    fields
    {
        field(1; "Primary Key"; Integer) { }
        field(2; myOption; [|OPTION|])
        {
            OptionMembers = A,B,C;
        }
    }
}