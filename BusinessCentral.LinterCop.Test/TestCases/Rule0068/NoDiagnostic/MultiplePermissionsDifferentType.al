codeunit 50000 Test
{
    Permissions =
        tabledata MyTableOne = r,
        tabledata MyTableTwo = i;

    procedure Test()
    var
        MyTableOne: Record MyTableOne;
        MyTableTwo: Record MyTableTwo;
    begin
        [|MyTableOne.FindFirst();|]
        [|MyTableTwo.Insert();|]
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