codeunit 50100 MyCodeunit
{
    trigger OnRun()
    var
        IsHandled: Boolean;
    begin
        OnCodeunitRun(IsHandled);
    end;

    [IntegrationEvent(false, false)]
    local procedure OnCodeunitRun(var IsHandled: Boolean)
    begin
    end;
}

codeunit 50101 MySubscriber
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, OnCodeunitRun, '', false, false)]
    local procedure OnCodeunitRun_MyCodeunit(var IsHandled: Boolean)
    begin
        if IsHandled then
            exit;

        [|IsHandled := MyProcedure();|]
    end;

    local procedure MyProcedure(): Boolean;
    begin

    end;
}