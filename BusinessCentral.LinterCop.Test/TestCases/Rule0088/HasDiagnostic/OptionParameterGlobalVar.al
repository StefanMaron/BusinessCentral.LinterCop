codeunit 50100 MyCodeunit
{
    var
    [|Mode|]: Option "None","Allow deletion",Match;

    procedure MyProcedure()
    var
        ReservationManagement: Codeunit "Reservation Management PTE";
    begin
        ReservationManagement.SetItemTrackingHandling(Mode);
    end;
}

codeunit 50101 "Reservation Management PTE"
{
    procedure SetItemTrackingHandling(Mode: Option "None","Allow deletion",Match)
    begin
    end;
}