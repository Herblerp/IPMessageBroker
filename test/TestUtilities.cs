using MessageBroker;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    public class TestUtilities
    {
        public void SendAllMessagesOnce()
        {
            Log log = Log.Instance;
            Publisher publisher = Publisher.Instance;

            Serializer serializer = new Serializer();

            ErrorMessage errorMessage = new ErrorMessage()
            {
                header = new ErrorMessageHeader()
                {
                    sender = systeemNaam.Planning,
                    timestamp = DateTime.Now,
                    versie = "1"
                },
                body = new ErrorMessageBody()
                {
                    errorBericht = "Ik ben een errorbericht"
                }
            };
            publisher.NewMessage(serializer.SerializeMessage(errorMessage),"errorEx");

            EventMessage eventMessage = new EventMessage()
            {
                header = new EventMessageHeader()
                {
                    sender = systeemNaam.Planning,
                    timestamp = DateTime.Now,
                    versie = "1"
                },
                body = new EventMessageBody()
                {
                    adres = new EventAdres
                    {
                        land = "Belgie",
                        postcode = "1000",
                        provincie = "Vlaams-brabant",
                        stad = "Brussel",
                        straat = "industriekaai"
                    },
                    beschikbarePlaatsen = "100",
                    eindDatum = DateTime.Now,
                    eventUUID = Guid.NewGuid(),
                    isActief = true,
                    naam = "EHachB",
                    omschrijving = "Hacken afou",
                    organisatieUUID = Guid.NewGuid(),
                    prijs = 10,
                    startDatum = DateTime.Now
                }
            };
            publisher.NewMessage(serializer.SerializeMessage(eventMessage),"errorEx");

        }
    }
}
