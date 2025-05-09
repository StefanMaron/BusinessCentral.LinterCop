interface MyInterface
{
    procedure MyProcedure() [|;|]
}

controladdin MyControladdin
{
    procedure MyProcedure() [|;|]
    event ControlAddInReady() [|;|]
}