using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Dka.Net5.TestingJSchema.Models
{
    public partial class FormV2_003 
    {
        [JsonProperty("form-section/0", Required = Required.Always)]
        [Required]
        [Display(Name = "Personal details")]
        public FormSection_0 FormSection_0 { get; set; } = new FormSection_0();
    
        private IDictionary<string, object> _additionalProperties = new Dictionary<string, object>();
    
        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    }
    
    public partial class FormSection_0 
    {
        [JsonProperty("form-question/0", Required = Required.Always)]
        [Required(AllowEmptyStrings = true)]
        public string FormQuestion_0 { get; set; }
    
        [JsonProperty("form-question/1", Required = Required.Always)]
        [Required(AllowEmptyStrings = true)]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FormSection_0FormQuestion_1 FormQuestion_1 { get; set; }
    
        private IDictionary<string, object> _additionalProperties = new Dictionary<string, object>();
    
        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    }
    
    public enum FormSection_0FormQuestion_1
    {
        [EnumMember(Value = @"Cat")]
        Cat = 0,
    
        [EnumMember(Value = @"Dog")]
        Dog = 1
    }    
}