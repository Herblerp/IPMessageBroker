using MessageBroker.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBroker
{
    public interface IMessageHandler
    {
        void HandleAankoopMessage(AankoopMessage aankoopMessage);
        void HandleBadgeMessage(BadgeMessage badgeMessage);
        void HandleBezoekerMessage(BezoekerMessage bezoekerMessage);
        void HandleCreditnotaMessage(CreditnotaMessage creditnotaMessage);
        void HandleErrorMessage(ErrorMessage errorMessage);
        void HandleEventMessage(EventMessage eventMessage);
        void HandleFactuurMessage(FactuurMessage factuurMessage);
        void HandleGroepRegistratieMessage(GroepRegistratieMessage groepResgistratieMessage);
        void HandleInschrijvingsMessage(InschrijvingsMessage inschrijvingsMessage);
        void HandleKalenderMessage(KalenderMessage kalenderMessage);
        void HandleKeepAliveMessage(KeepAliveMessage keepAliveMessage);
        void HandleLocatieMessage(LocatieMessage locatiemessage);
        void HandleOplaadMessage(OplaadMessage oplaadMessage);
        void HandleOrganisatieMessage(OrganisatieMessage organisatieMessage);
        void HandlePingMessage(PingMessage pingMessage);
        void HandleRegistratieMessage(RegistratieMessage registratieMessage);
        void HandleReservatieMessage(ReservatieMessage reservatieMessage);
        void HandleSessieMessage(SessieMessage sessieMessage);
        void HandleTaakMessage(TaakMessage taakMessage);
        void HandleVerkoopsItemMessage(VerkoopsItemMessage verkoopsItemMessage);
        void HandleWerkMessage(WerkMessage werkMessage);
        void HandleWerknemerMessage(WerknemerMessage werknemerMessage);
    }
}
