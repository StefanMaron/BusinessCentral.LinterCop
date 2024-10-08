codeunit 50000 MyCodeunit
{

    trigger OnRun()
    var
        MyTable: Record MyTable;
    begin
        [|MyTable.Insert();|]
        [|MyTable.Modify();|]

        [|MyTable.Rename(1);|]
        [|MyTable.ModifyAll(MyField2, 2);|]

        [|MyTable.Find();|]
        [|MyTable.FindFirst();|]
        [|MyTable.FindLast();|]
        [|MyTable.FindSet();|]
        [|if MyTable.IsEmpty() then;|]

        [|MyTable.Delete();|]
        [|MyTable.Insert();|]
        [|MyTable.DeleteAll();|]
    end;
}

table 50000 MyTable
{
    Caption = '', Locked = true;

    fields
    {
        field(1; MyField; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
        field(2; MyField2; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
    }
}