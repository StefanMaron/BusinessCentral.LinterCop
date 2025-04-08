table 50100 "My Table Test"
{
    Caption = 'My Table Test';
    DataClassification = CustomerContent;

    fields
    {
        field(1; Test; Code[10])
        {
            Caption = 'Test';
        }
        field(2; [|Test2|]; Decimal)
        {
            Caption = 'Test2';
            FieldClass = FlowField;
            CalcFormula = sum("Sales Line".Amount);
        }
    }
    
    keys
    {
        key(PK; Test)
        {
            Clustered = true;
        }
    }
}