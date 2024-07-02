table 50100 MyTable
{
    DataClassification = ToBeClassified;

    fields
    {
        field(1; MyField; Integer)
        {
            DataClassification = ToBeClassified;
        }
        field(2; MyField2; Boolean)
        {
            FieldClass = FlowField;
            CalcFormula = exist(MyTable where (MyField = field(MyField)));
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
    
    procedure MyProcedure();
    begin
        [|Rec.MyField2 := false;|]
    end;
}