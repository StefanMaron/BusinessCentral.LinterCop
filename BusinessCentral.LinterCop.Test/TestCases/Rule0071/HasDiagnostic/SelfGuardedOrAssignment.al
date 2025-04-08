codeunit 50000 MyCodeunit
{
    [IntegrationEvent(false, false)]
    local procedure OnMyEvent(var IsHandled: Boolean)
    begin
    end;
}

codeunit 50001 MySubscriber
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, OnMyEvent, '', false, false)]
    local procedure OnMyEventSubscriber(var IsHandled: Boolean)
    begin
        [|IsHandled := IsHandled and (Random(2) = 1);|]
        [|IsHandled := (IsHandled or (Random(2) = 1)) and (Random(2) = 1);|]
    end;
}