codeunit 50100 MyCodeunit
{
    [IntegrationEvent(false, false)]
    [Obsolete('Lorem ipsum dolor sit amet diam no', '1.0')]
    local procedure OnBefore([|IsHandled|]: Boolean)
    begin
    end;
}