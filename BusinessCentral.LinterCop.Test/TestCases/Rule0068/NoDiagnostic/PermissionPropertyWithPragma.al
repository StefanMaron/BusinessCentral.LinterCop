codeunit 50000 PragmaTestCodeunit
{
    // example from issue #923 (pragma in permissions property)

    Permissions =
        tabledata MyTableOne = r,
#pragma warning disable AA0123
        tabledata MyTableTwo = r,
#pragma warning restore AA0123
        tabledata MyTableThree = r;

    trigger OnRun()
    var
        MyTableOne: Record MyTableOne;
        MyTableTwo: Record MyTableTwo;
        MyTableThree: Record MyTableThree;
    begin
        MyTableOne.FindFirst();
        [|MyTableTwo.FindFirst();|]
        [|MyTableThree.FindFirst();|]
    end;
}
table 50000 MyTableOne
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}

table 50001 MyTableTwo
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}

table 50002 MyTableThree
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}