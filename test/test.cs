using MessageBroker;
using MessageBroker.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class test : IMessageHandler
    {
        public void HandleAankoopMessage(AankoopMessage aankoopMessage)
        {
            Console.WriteLine("AankoopUUID is: " + aankoopMessage.body.aankoopUUID);
        }

        public void HandleBadgeMessage(BadgeMessage badgeMessage)
        {
            Console.WriteLine("BadgeUUID is: " + badgeMessage.body.badgeUUID);
        }

        public void HandleBezoekerMessage(BezoekerMessage bezoekerMessage)
        {
            Console.WriteLine("BezoekerUUID is: " + bezoekerMessage.body.bezoekerUUID);
        }

        public void HandleCreditnotaMessage(CreditnotaMessage creditnotaMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleErrorMessage(ErrorMessage errorMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleEventMessage(EventMessage eventMessage)
        {
            Console.WriteLine("EventUUID is: " + eventMessage.body.eventUUID);
        }

        public void HandleFactuurMessage(FactuurMessage factuurMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleGroepRegistratieMessage(GroepRegistratieMessage groepResgistratieMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleInschrijvingsMessage(InschrijvingsMessage inschrijvingsMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleKalenderMessage(KalenderMessage kalenderMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleKeepAliveMessage(KeepAliveMessage keepAliveMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleLocatieMessage(LocatieMessage locatiemessage)
        {
            throw new NotImplementedException();
        }

        public void HandleOplaadMessage(OplaadMessage oplaadMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleOrganisatieMessage(OrganisatieMessage organisatieMessage)
        {
            throw new NotImplementedException();
        }

        public void HandlePingMessage(PingMessage pingMessage)
        {
            Console.WriteLine(pingMessage.header.timestamp);
            Console.WriteLine(pingMessage.header.versie);
            Console.WriteLine(pingMessage.header.type);
            Console.WriteLine(pingMessage.body.pingUUID);
        }

        public void HandleRegistratieMessage(RegistratieMessage registratieMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleReservatieMessage(ReservatieMessage reservatieMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleSessieMessage(SessieMessage sessieMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleTaakMessage(TaakMessage taakMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleVerkoopsItemMessage(VerkoopsItemMessage verkoopsItemMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleWerkMessage(WerkMessage werkMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleWerknemerMessage(WerknemerMessage werknemerMessage)
        {
            throw new NotImplementedException();
        }
    }
}
