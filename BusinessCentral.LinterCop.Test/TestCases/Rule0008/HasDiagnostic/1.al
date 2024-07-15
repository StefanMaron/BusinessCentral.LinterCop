table 50100 MyTable
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; MyField; Code[20])
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
    
    procedure MyProcedure()
    var
        MyCode: Code[20];
    begin
        [|Rec.SetRange(MyField ,'<>%1', MyCode);|]
    end;
}