codeunit 50000 CommentTestCodeunit
{
    Permissions =
        tabledata MyTableOne = r,
        // single line comment
        tabledata MyTableTwo = r;

    trigger OnRun()
    var
        MyTableOne: Record MyTableOne;
        MyTableTwo: Record MyTableTwo;
    begin
        MyTableOne.FindFirst();
        [|MyTableTwo.FindFirst();|]
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