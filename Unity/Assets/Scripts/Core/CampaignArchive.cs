using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PowerAboveAll
{
    // Dosya işlemi yapmaz. Eski arşiv geçişi ve yeni arşiv doğrulaması tek yerde tutulur.
    public static class CampaignArchive
    {
        public const int CurrentVersion = 2;

        [Serializable] private sealed class Envelope
        {
            public int Version;
            public CampaignState State;
        }

        public static string Serialize(CampaignState state, bool prettyPrint = true)
        {
            CampaignCore.Validate(state);
            var serializer = new DataContractJsonSerializer(typeof(Envelope));
            using (var stream = new MemoryStream())
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false, prettyPrint, "  "))
            {
                serializer.WriteObject(writer, new Envelope { Version = CurrentVersion, State = state });
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static CampaignState Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Campaign archive is empty.", nameof(json));
            Envelope envelope;
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    envelope = (Envelope)new DataContractJsonSerializer(typeof(Envelope)).ReadObject(stream);
            }
            catch (SerializationException error)
            {
                throw new ArgumentException("Invalid campaign archive.", nameof(json), error);
            }
            if (envelope == null || envelope.State == null || (envelope.Version != 1 && envelope.Version != CurrentVersion))
                throw new ArgumentException("Unsupported campaign archive.", nameof(json));
            var state = envelope.State;
            if (envelope.Version == 1)
            {
                CampaignCore.ValidateBase(state);
                if ((!string.IsNullOrEmpty(state.RoleId) && state.RoleId != "legacy") || state.NextMandateWeek != 0 ||
                    (state.Mandates != null && state.Mandates.Count != 0))
                    throw new ArgumentException("Invalid legacy role data.", nameof(json));
                state.RoleId = "legacy";
                state.Mandates = new List<MandateObligation>();
            }
            CampaignCore.Validate(state);
            return state;
        }
    }
}
