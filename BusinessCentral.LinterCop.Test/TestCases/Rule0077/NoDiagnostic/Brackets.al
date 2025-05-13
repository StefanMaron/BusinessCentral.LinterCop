codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyTable: Record MyTable;
        i: Integer;
        b: Boolean;
        d: Date;
        t: Time;
    begin
        d := [|Today()|];
        d := [|WorkDate()|];
        b := [|GuiAllowed()|];
        t := [|Time()|];
        i := [|MyTable.Count()|];
        b := [|MyTable.IsEmpty()|];

        MyTable.ReadIsolation([|IsolationLevel|]::UpdLock);
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}

xmlport 50100 MyXmlport
{
    trigger OnInitXmlPort()
    begin
        currXMLport.TextEncoding := [|TextEncoding|]::Windows;
    end;
}