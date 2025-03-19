table 50100 MyTable
{
    fields
    {
        field(1; "No."; Code[20]) { }
        field(2; "Warehouse Status"; [|Option|])
        {
            CalcFormula = min("My Warehouse Shipment Header"."Document Status" where("No." = field("No.")));
            FieldClass = FlowField;
            OptionMembers = " ","Partially Picked","Partially Shipped","Completely Picked","Completely Shipped";
        }
    }
}

table 50101 "My Warehouse Shipment Header"
{
    fields
    {
        field(1; "No."; Code[20]) { }
        field(2; "Document Status"; Option)
        {
            OptionMembers = " ","Partially Picked","Partially Shipped","Completely Picked","Completely Shipped";
        }
    }
}

enum 50100 "Warehouse Status" { }