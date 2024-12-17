codeunit 50100 MyCodeunit
{
    ObsoleteState = Pending;

    [IntegrationEvent(false, false)]
    local procedure OnBefore([|IsHandled|]: Boolean)
    begin
    end;
}