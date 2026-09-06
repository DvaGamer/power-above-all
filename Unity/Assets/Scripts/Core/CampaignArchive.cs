using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace PowerAboveAll
{
    // Dosya işlemi yapmaz. Eski arşiv geçişi ve yeni arşiv doğrulaması tek yerde tutulur.
    public static class CampaignArchive
    {
        public const int CurrentVersion = 4;

        [Serializable] private sealed class Envelope
        {
            public int Version;
            public CampaignState State;
        }

        // DCS'nin zorunlu üye sözleşmesi v3 alan varlığını denetler; bütün State DTO'su çoğaltılmaz.
        [DataContract] private sealed class RequiredAccordEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredAccordState State = null;
        }

        [DataContract] private sealed class RequiredAccordState
        {
            [DataMember(IsRequired = true)] public string AccordRegionId = null;
            [DataMember(IsRequired = true)] public int AccordUntilWeek = 0;
        }

        [DataContract] private sealed class RequiredVictoryEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredVictoryState State = null;
        }

        [DataContract] private sealed class RequiredVictoryState
        {
            [DataMember(IsRequired = true)] public string PendingVictoryId = null;
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
                if (envelope != null && envelope.Version >= 3)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredAccordEnvelope)new DataContractJsonSerializer(typeof(RequiredAccordEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null || required.State.AccordRegionId == null)
                            throw new SerializationException("Missing required regional accord data.");
                    }
                if (envelope != null && envelope.Version >= 4)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredVictoryEnvelope)new DataContractJsonSerializer(typeof(RequiredVictoryEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null || required.State.PendingVictoryId == null)
                            throw new SerializationException("Missing required victory decision data.");
                    }
            }
            catch (Exception error) when (IsArchiveReadError(error))
            {
                throw new ArgumentException("Invalid campaign archive.", nameof(json), error);
            }
            if (envelope == null || envelope.State == null || envelope.Version < 1 || envelope.Version > CurrentVersion)
                throw new ArgumentException("Unsupported campaign archive.", nameof(json));
            var state = envelope.State;
            if (envelope.Version < 3)
            {
                if (!string.IsNullOrEmpty(state.AccordRegionId) || state.AccordUntilWeek != 0)
                    throw new ArgumentException("Invalid regional accord data in an older archive.", nameof(json));
                state.AccordRegionId = "";
                state.AccordUntilWeek = 0;
            }
            if (envelope.Version < 4)
            {
                if (!string.IsNullOrEmpty(state.PendingVictoryId))
                    throw new ArgumentException("Invalid victory decision data in an older archive.", nameof(json));
                state.PendingVictoryId = "";
            }
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

        private static bool IsArchiveReadError(Exception error)
        {
            // Mono'nun yansımalı DCS okuyucusu bozuk sayıyı XmlException içinde sarabilir.
            while (error is TargetInvocationException && error.InnerException != null) error = error.InnerException;
            return error is SerializationException || error is XmlException || error is FormatException || error is OverflowException;
        }
    }
}
