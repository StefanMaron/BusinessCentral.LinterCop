table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {
            trigger OnValidate()
            var
                IsHandled: Boolean;
            begin
                OnBeforeCalculateNewValue([|Rec|], IsHandled);
                if IsHandled then
                    exit;
            end;
        }
    }

    [IntegrationEvent(false, false)]
    local procedure OnBeforeCalculateNewValue(var MyTable: Record MyTable; var IsHandled: Boolean)
    begin
    end;
}