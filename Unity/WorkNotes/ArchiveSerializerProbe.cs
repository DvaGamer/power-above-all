using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

[Serializable] public sealed class ProbeState
{
    public int Week;
    [OptionalField] public string RoleId;
    [OptionalField] public List<string> Mandates;
    [OptionalField] public int NextMandateWeek;
    public string NonSerializedProperty { get { return "must-not-appear"; } set {} }
}
[Serializable] public sealed class ProbeEnvelope { public int Version; public ProbeState State; }
public static class ArchiveSerializerProbe
{
    public static void Main()
    {
        var serializer = new DataContractJsonSerializer(typeof(ProbeEnvelope));
        using (var stream = new MemoryStream())
        {
            serializer.WriteObject(stream, new ProbeEnvelope { Version=2, State=new ProbeState { RoleId="crown", Mandates=new List<string>() } });
            Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
        }
        string[] inputs = {
            "{\"Version\":1,\"State\":{\"Week\":0}}",
            "{\"Version\":2,\"State\":{\"Week\":0,\"RoleId\":\"crown\",\"Mandates\":[]}}",
            "{\"Version\":2,\"State\":{\"Week\":0,\"RoleId\":\"crown\",\"Mandates\":null}}",
            "{\"Version\":2,\"State\":{\"Week\":0,\"RoleId\":\"crown\"}}"
        };
        foreach (string json in inputs)
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var result = (ProbeEnvelope)serializer.ReadObject(stream);
                    Console.WriteLine("v"+result.Version+" role="+result.State.RoleId+" list="+(result.State.Mandates==null?"NULL":result.State.Mandates.Count.ToString()));
                }
            }
            catch(Exception error) { Console.WriteLine(error.GetType().FullName+": "+error.Message); }
        }
    }
}
