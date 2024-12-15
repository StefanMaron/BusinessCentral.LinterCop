table 50108 MyTable
{
    fields
    {
        field(1; [|MyText|]; Text[1])
        {
            TableRelation = MyTable.MyTextExt;
        }

        field(2; [|MyCode|]; Text[1])
        {
            TableRelation = if (MyCode = const('const')) MyTable.MyCodeExt
            else 
            MyTable.MyTextExt;
        }
        field(3; MyTextExt; Text[100]) { }

        field(4; MyCodeExt; Text[100]) { }        
    }

    keys
    {
        key(PK; MyText) { Clustered = true; }
    }
}