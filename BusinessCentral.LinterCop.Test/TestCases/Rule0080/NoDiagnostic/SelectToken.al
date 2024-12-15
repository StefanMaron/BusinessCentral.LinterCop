codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyJsonToken: JsonToken;
        Result: JsonToken;
    begin
        MyJsonToken.SelectToken([|'$.custom_attributes[?(@.attribute_code == ''activation_status'')].value'|], Result);
    end;
}