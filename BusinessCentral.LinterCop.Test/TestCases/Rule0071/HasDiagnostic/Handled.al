// tests that also the less used name "handled" is reported, see https://github.com/search?q=repo%3AStefanMaron%2FMSDyn365BC.Code.History+%22var+Handled%3A+Boolean%22&type=code
codeunit 50100 MyCodeunit
{
    trigger OnRun()
    var
        Handled: Boolean;
    begin
        OnCodeunitRun(Handled);
    end;

    [IntegrationEvent(false, false)]
    local procedure OnCodeunitRun(var Handled: Boolean)
    begin
    end;
}

codeunit 50101 MySubscriber
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, OnCodeunitRun, '', false, false)]
    local procedure OnCodeunitRun_MyCodeunit(var Handled: Boolean)
    begin
        [|Handled := false;|]
    end;
}