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
        void HandleEventMessage(EventMessage eventMessage);
        void HandlePingMessage(PingMessage pingMessage);
    }
}
