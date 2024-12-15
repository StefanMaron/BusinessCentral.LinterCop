table 50108 MyTable
{
    fields
    {
        field(1; [|MyText|]; Text[1])
        {
            TableRelation = MyTable2.MyTextExt;
        }

        field(2; [|MyCode|]; Text[1])
        {
            TableRelation = if (MyCode = const('const')) MyTable2.MyCodeExt 
            else 
            MyTable2.MyTextExt;
        }
    }

    keys
    {
        key(PK; MyText) { Clustered = true; }
    }
}


table 50107 MyTable2
{
    fields
    {
        field(1; MyText; Text[1]) { }

        field(2; MyCode; Text[1]) { }
    }

    keys
    {
        key(PK; MyText) { Clustered = true; }
    }
}


tableextension 50110 MyExtension extends MyTable2
{
    fields
    {
        field(3; MyTextExt; Text[100]) { }

        field(4; MyCodeExt; Text[100]) { }
    }
}