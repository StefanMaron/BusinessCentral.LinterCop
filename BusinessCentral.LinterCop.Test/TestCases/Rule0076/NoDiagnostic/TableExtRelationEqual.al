table 50108 MyTable
{
    fields
    {
        field(1; [|MyText|]; Text[100])
        {
            TableRelation = MyTable.MyTextExt;
        }

        field(2; [|MyCode|]; Text[100])
        {
            TableRelation = if (MyCode = const('const')) MyTable.MyCodeExt 
            else 
            MyTable.MyTextExt;
        }
    }

    keys
    {
        key(PK; MyText) { Clustered = true; }
    }
}


tableextension 50108 MyExtension extends MyTable
{
    fields
    {
        field(3; MyTextExt; Text[100]) { }

        field(4; MyCodeExt; Text[100]) { }
    }
}