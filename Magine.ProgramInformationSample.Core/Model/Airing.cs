using System;
using System.Runtime.Serialization;

namespace Magine.ProgramInformationSample.Core.Model
{
    [DataContract]
    public struct Airing
    {
        [DataMember(Name = "channelId")]
        public string ChannelId { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "image")]
        public string ImageUrl { get; set; }
        [DataMember(Name = "start")]
        public DateTime Start { get; set; }
        [DataMember(Name = "stop")]
        public DateTime Stop { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}