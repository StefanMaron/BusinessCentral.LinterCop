//TODO: Test with an codeunit from a dependency extension so the ."GetLocation().IsInSource" returns false
codeunit 50100 MyCodeunit
{
    var
        ModeGlobal: Option "None","Allow deletion",Match; // Should not raise a diagnostic

    procedure MyProcedure()
    var
        ReservationManagement: Codeunit "Reservation Management";
        ModeLocal: Option "None","Allow deletion",Match; // Should not raise a diagnostic
    begin
        ReservationManagement.SetItemTrackingHandling(ModeLocal);
        ReservationManagement.SetItemTrackingHandling(ModeGlobal);
    end;
}