codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        ReservationManagement: Codeunit "Reservation Management PTE";
        [|Mode|]: Option "None","Allow deletion",Match;
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