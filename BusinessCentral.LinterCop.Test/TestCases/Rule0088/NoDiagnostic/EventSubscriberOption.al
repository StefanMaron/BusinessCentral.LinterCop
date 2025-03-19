codeunit 50100 MyCodeunit
{
    trigger OnRun()
    var
        OptionVariable: Option "", "test1";
    begin
        OnCodeunitRun(OptionVariable);
    end;

    [IntegrationEvent(false, false)]
    local procedure OnCodeunitRun(var OptionVariable: Option "", "test1")
    begin
    end;
}

codeunit 50101 MySubscriber
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, OnCodeunitRun, '', false, false)]
    local procedure OnCodeunitRun_MyCodeunit(var [|OptionVariable: Option "", "test1"|])
    begin
      
    end;
}