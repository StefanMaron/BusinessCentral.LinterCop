codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        Customer: Record Customer;
        myText: Text;
    begin
        myText := Customer.TableCaption();
    end;
}