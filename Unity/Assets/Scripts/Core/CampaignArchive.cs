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
        public const int CurrentVersion = 9;

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

        [DataContract] private sealed class RequiredDumasEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredDumasState State = null;
        }

        [DataContract] private sealed class RequiredDumasState
        {
            [DataMember(IsRequired = true)] public int DumasForageDueWeek = 0;
            [DataMember(IsRequired = true)] public int DumasNextForageWeek = 0;
        }

        [DataContract] private sealed class RequiredArmyEstablishmentEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredArmyEstablishmentState State = null;
        }

        [DataContract] private sealed class RequiredArmyEstablishmentState
        {
            [DataMember(IsRequired = true)] public string ArmyPolicyId = null;
            [DataMember(IsRequired = true)] public int ArmyTargetTroops = 0;
            [DataMember(IsRequired = true)] public int ArmyReductionDueWeek = 0;
        }

        [DataContract] private sealed class RequiredOfficerCommissionEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredOfficerCommissionState State = null;
        }

        [DataContract] private sealed class RequiredOfficerCommissionState
        {
            [DataMember(IsRequired = true)] public bool DumasOfficerCommission = false;
            [DataMember(IsRequired = true)] public bool DumasExtraRecruitUsed = false;
        }

        [DataContract] private sealed class RequiredRegionalReformEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredRegionalReformState State = null;
        }

        [DataContract] private sealed class RequiredRegionalReformState
        {
            [DataMember(IsRequired = true)] public string ReformRegionId = null;
            [DataMember(IsRequired = true)] public string ReformModeId = null;
            [DataMember(IsRequired = true)] public int ReformStepsRemaining = 0;
        }

        [DataContract] private sealed class RequiredCorrespondenceEnvelope
        {
            [DataMember(IsRequired = true)] public RequiredCorrespondenceState State = null;
        }
        [DataContract] private sealed class RequiredCorrespondenceState
        {
            [DataMember(IsRequired = true)] public List<CorrespondenceDesk> Correspondence = null;
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
                if (envelope != null && envelope.Version >= 5)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredDumasEnvelope)new DataContractJsonSerializer(typeof(RequiredDumasEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null)
                            throw new SerializationException("Missing required Dumas initiative data.");
                    }
                if (envelope != null && envelope.Version >= 6)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredArmyEstablishmentEnvelope)new DataContractJsonSerializer(typeof(RequiredArmyEstablishmentEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null || required.State.ArmyPolicyId == null)
                            throw new SerializationException("Missing required army establishment data.");
                    }
                if (envelope != null && envelope.Version >= 7)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredOfficerCommissionEnvelope)new DataContractJsonSerializer(typeof(RequiredOfficerCommissionEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null)
                            throw new SerializationException("Missing required officer commission data.");
                    }
                if (envelope != null && envelope.Version >= 8)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredRegionalReformEnvelope)new DataContractJsonSerializer(typeof(RequiredRegionalReformEnvelope)).ReadObject(stream);
                        if (required == null || required.State == null || required.State.ReformRegionId == null || required.State.ReformModeId == null)
                            throw new SerializationException("Missing required regional reform data.");
                    }
                if (envelope != null && envelope.Version >= 9)
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var required = (RequiredCorrespondenceEnvelope)new DataContractJsonSerializer(typeof(RequiredCorrespondenceEnvelope)).ReadObject(stream);
                        if(required == null || required.State == null || required.State.Correspondence == null)
                            throw new SerializationException("Missing required correspondence data.");
                    }
            }
            catch (Exception error) when (IsArchiveReadError(error))
            {
                throw new ArgumentException("Invalid campaign archive.", nameof(json), error);
            }
            if (envelope == null || envelope.State == null || envelope.Version < 1 || envelope.Version > CurrentVersion)
                throw new ArgumentException("Unsupported campaign archive.", nameof(json));
            var state = envelope.State;
            if(envelope.Version < 9)
            {
                if(state.Correspondence != null && state.Correspondence.Count != 0)
                    throw new ArgumentException("Invalid correspondence in an older archive.", nameof(json));
                state.Correspondence = new List<CorrespondenceDesk>();
            }
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
            if (envelope.Version < 5)
            {
                if (state.DumasForageDueWeek != 0 || state.DumasNextForageWeek != 0)
                    throw new ArgumentException("Invalid Dumas initiative data in an older archive.", nameof(json));
                state.DumasForageDueWeek = state.DumasNextForageWeek = 0;
            }
            if (envelope.Version < 6)
            {
                if ((state.ArmyPolicyId != null && state.ArmyPolicyId != "campaign") ||
                    state.ArmyTargetTroops != 0 || state.ArmyReductionDueWeek != 0)
                    throw new ArgumentException("Invalid army establishment data in an older archive.", nameof(json));
                state.ArmyPolicyId = "campaign";
                state.ArmyTargetTroops = state.ArmyReductionDueWeek = 0;
            }
            if (envelope.Version < 7)
            {
                if (state.DumasOfficerCommission || state.DumasExtraRecruitUsed)
                    throw new ArgumentException("Invalid officer commission data in an older archive.", nameof(json));
                state.DumasOfficerCommission = state.DumasExtraRecruitUsed = false;
            }
            if (envelope.Version < 8)
            {
                if (!string.IsNullOrEmpty(state.ReformRegionId) || !string.IsNullOrEmpty(state.ReformModeId) || state.ReformStepsRemaining != 0)
                    throw new ArgumentException("Invalid regional reform data in an older archive.", nameof(json));
                state.ReformRegionId = state.ReformModeId = "";
                state.ReformStepsRemaining = 0;
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
