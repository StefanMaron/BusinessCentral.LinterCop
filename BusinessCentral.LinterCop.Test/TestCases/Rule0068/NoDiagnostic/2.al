codeunit 50100 MyCodeunit
{
    procedure MyProcedure(Param: Enum MyEnum)
    begin
        case Param of
            MyEnum::Zero:
                [|;|] // do nothing but does not fall in else
            MyEnum::One:
                Message('Message');
            else
                Error('Error');
        end;
    end;
}

enum 50100 MyEnum
{
    value(0; Zero) { }
    value(1; One) { }
    value(2; Two) { }
}